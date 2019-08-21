using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Our.Umbraco.RedirectsViewer.Models.Import;
using Our.Umbraco.RedirectsViewer.Services;
using Skybrud.Umbraco.Redirects.Import.Csv;
using Skybrud.Umbraco.Redirects.Models.Import;
using Skybrud.Umbraco.Redirects.Models.Import.File;
using Umbraco.Core.Cache;
using Umbraco.Core.Configuration.UmbracoSettings;
using Umbraco.Core.Mapping;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence;

namespace Our.Umbraco.RedirectsViewer.Controllers
{
    using System;
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
        private readonly IRedirectUrlService _redirectUrlService;

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

        private readonly RedirectService _redirectService;

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
                                      RedirectService redirectService,
                                      ServiceContext services, 
                                      AppCaches appCaches, 
                                      IProfilingLogger logger, 
                                      IRuntimeState runtimeState, 
                                      UmbracoHelper umbracoHelper,UmbracoMapper mapper) : base(globalSettings, umbracoContextAccessor, sqlContext, services, appCaches, logger, runtimeState, umbracoHelper)
        {
            _redirectUrlService = this.Services.RedirectUrlService;

            _umbracoSettings = umbracoSettings;
            _mapper = mapper;
            _logger = logger;
            _localizedTextService = services.TextService;
            _contentService = services.ContentService;
            _domainService = services.DomainService;
            _redirectService = redirectService;
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
        public HttpResponseMessage GetRedirectsForContent(Guid contentKey)
        {
            if (this.IsUrlTrackingDisabled())
            {
                return new HttpResponseMessage(HttpStatusCode.Conflict);
            }

            var redirects = _mapper.MapEnumerable<IRedirectUrl, ContentRedirectUrl>(_redirectUrlService.GetContentRedirectUrls(contentKey).ToArray());

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
                var domains = this._domainService.GetAll(true).Where(x => x.RootContentId.HasValue).ToList();
                var status = 0;
                if (domains.Any())
                {
                    // get the content item
                    var content = this._contentService.GetById(redirect.ContentKey);
                    status = _redirectService.AddRedirect(content,domains,redirect.Url);
                }

                if (status == 1)
                {
                    return this.Request.CreateNotificationValidationErrorResponse(this._localizedTextService.Localize("redirectsviewer/urlExistsError"));
                }

                if (status == 0)
                {
                    return this.Request.CreateNotificationSuccessResponse(this._localizedTextService.Localize("redirectsviewer/createSuccess"));
                }
               
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
        private const string FileUploadPath = "~/App_Data/TEMP/FileUploads/";
        private const string FileName = "redirects{0}.csv";

        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> Import()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.UnsupportedMediaType,
                    Content = new StringContent("File must be a valid CSV or Excel file")
                });
            }

            var uploadFolder = HttpContext.Current.Server.MapPath(FileUploadPath);
            Directory.CreateDirectory(uploadFolder);
            var provider = new CustomMultipartFormDataStreamProvider(uploadFolder);

            var result = await Request.Content.ReadAsMultipartAsync(provider);

            var file = result.FileData[0];
            var path = file.LocalFileName;
            var ext = path.Substring(path.LastIndexOf('.')).ToLower();

            if (ext != ".csv" && ext != ".xlst")
            {
                throw new HttpResponseException(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.UnsupportedMediaType,
                    Content = new StringContent("File must be a valid CSV or Excel file")
                });
            }

            var fileNameAndPath = HttpContext.Current.Server.MapPath(FileUploadPath + string.Format(FileName, DateTime.Now.Ticks));

            System.IO.File.Copy(file.LocalFileName, fileNameAndPath, true);
            
            var importer = new RedirectsImporterService(_redirectService,_domainService);
            
            IRedirectsFile redirectsFile;

            switch (ext)
            {   
                default:
                    var csvFile = new CsvRedirectsFile(new RedirectPublishedContentFinder(UmbracoContext.ContentCache))
                        {
                            FileName = fileNameAndPath,
                            Seperator = CsvSeparator.Comma
                        };

                    redirectsFile = csvFile;

                    break;
            }
                
            var response = importer.Import(redirectsFile);

            using (var ms = new MemoryStream())
            {
                using (var outputFile = new FileStream(response.File.FileName, FileMode.Open, FileAccess.Read))
                {
                    byte[] bytes = new byte[outputFile.Length];
                    outputFile.Read(bytes, 0, (int)outputFile.Length);
                    ms.Write(bytes, 0, (int)outputFile.Length);

                    HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
                    httpResponseMessage.Content = new ByteArrayContent(bytes.ToArray());
                    httpResponseMessage.Content.Headers.Add("x-filename", "redirects.csv");
                    httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                    httpResponseMessage.Content.Headers.ContentDisposition.FileName = "redirects.csv";
                    httpResponseMessage.StatusCode = HttpStatusCode.OK;

                    return httpResponseMessage;
                }
            }
        }

        public class CustomMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
        {
            public CustomMultipartFormDataStreamProvider(string path) : base(path) { }

            public override string GetLocalFileName(HttpContentHeaders headers)
            {
                return headers.ContentDisposition.FileName.Replace("\"", string.Empty);
            }
        }
    }
}
