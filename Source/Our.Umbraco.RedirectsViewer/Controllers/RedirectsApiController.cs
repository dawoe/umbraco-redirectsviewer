using Our.Umbraco.RedirectsViewer.Services;
using Umbraco.Core.Cache;
using Umbraco.Core.Configuration.UmbracoSettings;
using Umbraco.Core.Mapping;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence;

namespace Our.Umbraco.RedirectsViewer.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using global::Umbraco.Core;
    using global::Umbraco.Core.Configuration;
    using global::Umbraco.Core.Logging;
    using global::Umbraco.Core.Services;
    using global::Umbraco.Web;
    using global::Umbraco.Web.Editors;
    using global::Umbraco.Web.Models.ContentEditing;
    using global::Umbraco.Web.WebApi;

    using Models;

    /// <summary>
    /// The redirects api controller.
    /// </summary>
    public class RedirectsApiController : BackOfficeNotificationsController
    {
        /// <summary>
        /// The redirect url service.
        /// </summary>
        private readonly IOurRedirectsService _redirectUrlService;

        private readonly IUmbracoSettingsSection _umbracoSettings;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="RedirectsApiController"/> class.
        /// </summary>
        public RedirectsApiController(IUmbracoSettingsSection umbracoSettings, 
                                      IGlobalSettings globalSettings, 
                                      IUmbracoContextAccessor umbracoContextAccessor, 
                                      ISqlContext sqlContext, 
                                      ServiceContext services, 
                                      AppCaches appCaches, 
                                      IProfilingLogger logger, 
                                      IRuntimeState runtimeState, 
                                      UmbracoHelper umbracoHelper,UmbracoMapper mapper,IOurRedirectsService ourRedirectsService) : base(globalSettings, umbracoContextAccessor, sqlContext, services, appCaches, logger, runtimeState, umbracoHelper)
        {
            _redirectUrlService = ourRedirectsService;

            _umbracoSettings = umbracoSettings;
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

            var redirects = new List<ContentRedirectUrl>();

            if (!string.IsNullOrEmpty(culture))
            {
                redirects = _mapper.MapEnumerable<IRedirectUrl, ContentRedirectUrl>(_redirectUrlService.GetContentRedirectUrls(contentKey).Where(x => x.Culture.InvariantEquals(culture)).ToArray());
            }
            else
            {
                redirects = _mapper.MapEnumerable<IRedirectUrl, ContentRedirectUrl>(_redirectUrlService.GetContentRedirectUrls(contentKey).ToArray());
            }

            return this.Request.CreateResponse(HttpStatusCode.OK, redirects);
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
        public HttpResponseMessage DeleteRedirect(Guid id)
        {
            if (this.IsUrlTrackingDisabled())
            {
                return new HttpResponseMessage(HttpStatusCode.Conflict);
            }

            try
            {
                this._redirectUrlService.Delete(id);

                return this.Request.CreateNotificationSuccessResponse(this._localizedTextService.Localize("redirectsviewer/deleteSuccess"));
            }
            catch (Exception e)
            {
                this._logger.Error(this.GetType(), "Error deleting redirect", e);
                return this.Request.CreateNotificationValidationErrorResponse(this._localizedTextService.Localize("redirectsviewer/deleteError"));
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
        public HttpResponseMessage CreateRedirect(RedirectSave redirect)
        {
            if (this.IsUrlTrackingDisabled())
            {
                return new HttpResponseMessage(HttpStatusCode.Conflict);
            }

            if (redirect.ContentKey == Guid.Empty || string.IsNullOrEmpty(redirect.Url))
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            var urlError = this.ValidateUrl(redirect);

            if (!string.IsNullOrEmpty(urlError))
            {
                return this.Request.CreateNotificationValidationErrorResponse(urlError);
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
                            assignedDomain = domains.FirstOrDefault(x => x.LanguageIsoCode.InvariantEquals(redirect.Culture) && pathIds.Contains(x.RootContentId.Value.ToString()));
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

                if (redirects.Any(x => x.Url == redirect.Url))
                {
                    return this.Request.CreateNotificationValidationErrorResponse(this._localizedTextService.Localize("redirectsviewer/urlExistsError"));
                }

                if (!string.IsNullOrEmpty(redirect.Culture))
                {
                    this._redirectUrlService.Register(redirect.Url, redirect.ContentKey, redirect.Culture.ToLower());
                }
                else
                {
                    this._redirectUrlService.Register(redirect.Url, redirect.ContentKey);
                }

                return this.Request.CreateNotificationSuccessResponse(this._localizedTextService.Localize("redirectsviewer/createSuccess"));
            }
            catch (Exception ex)
            {
                this._logger.Error(this.GetType(), "Error creating redirect", ex);
                return this.Request.CreateNotificationValidationErrorResponse(this._localizedTextService.Localize("redirectsviewer/createError"));
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
            return _umbracoSettings.WebRouting.DisableRedirectUrlTracking;
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
                return this._localizedTextService.Localize("redirectsviewer/urlRelativeError");
            }

            if (redirect.Url.Contains("."))
            {
                return this._localizedTextService.Localize("redirectsviewer/urlNoDotsError");
            }

            if (redirect.Url.Contains(" "))
            {
                return this._localizedTextService.Localize("redirectsviewer/urlNoSpacesError");
            }


            // make sure we have a valid url
            redirect.Url = redirect.Url.ToLower().EnsureStartsWith("/").TrimEnd("/");

            return string.Empty;
        }
    }
}
