

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.ContentEditing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common;
using Our.Umbraco.RedirectsViewer.Models;
using Umbraco.Cms.Core.Actions;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Extensions;
using Umbraco.Extensions;
using HttpRequestExtensions = Umbraco.Extensions.HttpRequestExtensions;
using StringExtensions = Umbraco.Extensions.StringExtensions;

namespace Our.Umbraco.RedirectsViewer.Controllers
{

    /// <summary>
    /// The redirects api controller.
    /// </summary>
    public class RedirectsApiController : BackOfficeNotificationsController
    {
        /// <summary>
        /// The redirect url service.
        /// </summary>
        private readonly IRedirectUrlService _redirectUrlService;

        private readonly UmbracoMapper _mapper;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// The localized text service.
        /// </summary>
        private readonly ILocalizedTextService _localizedTextService;

        /// <summary>
        /// The content service.
        /// </summary>
        private readonly IContentService _contentService;

        /// <summary>
        /// The domain service.
        /// </summary>
        private readonly IDomainService _domainService;
        private readonly IOptionsMonitor<WebRoutingSettings> _webRoutingSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedirectsApiController"/> class.
        /// </summary>
        public RedirectsApiController(IRedirectUrlService redirectUrlService,
                                      IUmbracoContextFactory umbracoContextAccessor, 
                                      IOptionsMonitor<WebRoutingSettings> webRoutingSettings,
                                      ISqlContext sqlContext, 
                                      ServiceContext services, 
                                      AppCaches appCaches, 
                                      ILogger<RedirectsApiController> logger, 
                                      IRuntimeState runtimeState, 
                                      UmbracoHelper umbracoHelper,UmbracoMapper mapper) : base()
        {
            _redirectUrlService = redirectUrlService;
            _webRoutingSettings = webRoutingSettings;

            _mapper = mapper;
            _logger = logger;
            _localizedTextService = services.TextService;
            _contentService = services.ContentService;
            _domainService = services.DomainService;
        }

        /// <summary>
        /// Gets the redirects for the content
        /// </summary>
        /// <param name="contentKey">
        /// The content key.
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [HttpGet]
        public HttpResponseMessage GetRedirectsForContent(Guid contentKey, string culture = "")
        {
            if (this.IsUrlTrackingDisabled())
            {
                return new HttpResponseMessage(HttpStatusCode.Conflict);
            }

            var redirects = _mapper.MapEnumerable<IRedirectUrl, ContentRedirectUrl>(_redirectUrlService.GetContentRedirectUrls(contentKey).ToList());

            if (!string.IsNullOrEmpty(culture))
            {
                redirects = redirects.Where(x => StringExtensions.InvariantEquals(x.Culture, culture)).ToList();
            }          
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(redirects))
            };
        }

        /// <summary>
        /// Deletes a redirect
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [HttpPost]
        [HttpDelete]
        public IActionResult  DeleteRedirect(Guid id)
        {
            if (this.IsUrlTrackingDisabled())
            {
                return Conflict();
            }

            try
            {
                this._redirectUrlService.Delete(id);

                return Ok(this._localizedTextService.Localize("redirectsviewer","deleteSuccess"));
            }
            catch (Exception e)
            {
                this._logger.LogError( "Error deleting redirect", e);
                return ValidationProblem(this._localizedTextService.Localize("redirectsviewer","deleteError"));
            }           
        }

        /// <summary>
        /// Creates a redirect
        /// </summary>
        /// <param name="redirect">
        /// The redirect.
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [HttpPost]
        public IActionResult CreateRedirect(RedirectSave redirect)
        {
            if (this.IsUrlTrackingDisabled())
            {
                return Conflict();
            }

            if (redirect.ContentKey == Guid.Empty || string.IsNullOrEmpty(redirect.Url))
            {
                return BadRequest();
            }

            var urlError = this.ValidateUrl(redirect);

            if (!string.IsNullOrEmpty(urlError))
            {
                return ValidationProblem(urlError);
            }

            try
            {
                // check if we there is a domain configured for this node in umbraco
                var rootNode = string.Empty;
                var rootNodeUrl = string.Empty;

                // get the content item
                var content = this._contentService.GetById(redirect.ContentKey);

                if (content == null)
                {
                    throw new Exception("Content does not exist");
                }

                // set default culture (if any)
                if (string.IsNullOrEmpty(redirect.Culture) && content.CultureInfos.Count > 0)
                {
                    redirect.Culture = content.CultureInfos[0].Culture;
                }

                // get all the domains that have a root content id set
                var domains = this._domainService.GetAll(true).Where(x => x.RootContentId.HasValue).ToList();
                
                if (domains.Any())
                {
                    // get all the ids in the path
                    var pathIds = content.Path.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                    if (pathIds.Any())
                    {
                        // find a domain that is in the path of the item
                        IDomain assignedDomain = null;

                        if (!string.IsNullOrEmpty(redirect.Culture))
                        {
                            assignedDomain = domains.FirstOrDefault(x => StringExtensions.InvariantEquals(x.LanguageIsoCode,redirect.Culture) && pathIds.Contains(x.RootContentId.Value.ToString()));
                        }
                        else
                        {
                            assignedDomain = domains.FirstOrDefault(x => pathIds.Contains(x.RootContentId.Value.ToString()));
                        }

                        if (assignedDomain != null)
                        {
                            // get the root content node
                            rootNode = assignedDomain.RootContentId.Value.ToString();
                            rootNodeUrl = assignedDomain.DomainName;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(rootNode))
                {
                    // remove language prefix
                    if (!string.IsNullOrEmpty(rootNodeUrl) && rootNodeUrl != "/" && redirect.Url.StartsWith(rootNodeUrl))
                    {
                        redirect.Url = redirect.Url.Substring(rootNodeUrl.Length);
                    }

                    if (string.IsNullOrEmpty(redirect.Url))
                    {
                        throw new Exception("Cannot redirect root node");
                    }

                    // prefix the url with the root content node
                    redirect.Url = rootNode + redirect.Url;
                }

                // check if there is already a redirect with the url
                long total;
                var redirects = this._redirectUrlService.GetAllRedirectUrls(0, int.MaxValue, out total);

                if (redirects.Any(x => x.Url == redirect.Url && x.Culture == redirect.Culture))
                {
                    return ValidationProblem(this._localizedTextService.Localize("redirectsviewer","urlExistsError"));
                }

                if (!string.IsNullOrEmpty(redirect.Culture))
                {
                    this._redirectUrlService.Register(redirect.Url, redirect.ContentKey, redirect.Culture.ToLower());
                }
                else
                {
                    this._redirectUrlService.Register(redirect.Url, redirect.ContentKey, string.Empty);
                }

                if (!string.IsNullOrEmpty(redirect.Culture))
                {
                    this._redirectUrlService.Register(redirect.Url, redirect.ContentKey, redirect.Culture.ToLower());
                }
                else
                {
                    this._redirectUrlService.Register(redirect.Url, redirect.ContentKey);
                }

                return Ok(this._localizedTextService.Localize("redirectsviewer","createSuccess"));
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error creating redirect", ex);
                return ValidationProblem(this._localizedTextService.Localize("redirectsviewer","createError"));
            }
        }

        /// <summary>
        /// Checks if  url tracking disabled.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsUrlTrackingDisabled()
        {
            return _webRoutingSettings.CurrentValue.DisableRedirectUrlTracking;
        }

        /// <summary>
        /// Validates the url when creating a redirect
        /// </summary>
        /// <param name="redirect">
        /// The redirect.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string ValidateUrl(RedirectSave redirect)
        {
            if (redirect.Url.StartsWith("http://") || redirect.Url.StartsWith("https://"))
            {
                return this._localizedTextService.Localize("redirectsviewer","urlRelativeError");
            }

            if (redirect.Url.Contains("."))
            {
                return this._localizedTextService.Localize("redirectsviewer","urlNoDotsError");
            }

            if (redirect.Url.Contains(" "))
            {
                return this._localizedTextService.Localize("redirectsviewer","urlNoSpacesError");
            }


            // make sure we have a valid url
            redirect.Url = redirect.Url.ToLower().EnsureStartsWith("/").TrimEnd("/");

            return string.Empty;
        }
    }
}
