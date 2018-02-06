namespace Our.Umbraco.RedirectsViewer.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;

    using AutoMapper;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Configuration;
    using global::Umbraco.Core.Logging;
    using global::Umbraco.Core.Services;
    using global::Umbraco.Web;
    using global::Umbraco.Web.Editors;
    using global::Umbraco.Web.Models.ContentEditing;
    using global::Umbraco.Web.WebApi;

    using Our.Umbraco.RedirectsViewer.Models;

    /// <summary>
    /// The redirects api controller.
    /// </summary>
    public class RedirectsApiController : BackOfficeNotificationsController
    {
        /// <summary>
        /// The redirect url service.
        /// </summary>
        private readonly IRedirectUrlService redirectUrlService;

        /// <summary>
        /// The mapper.
        /// </summary>
        private readonly IMappingEngine mapper;

        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedirectsApiController"/> class.
        /// </summary>
        public RedirectsApiController()
        {
            this.redirectUrlService = this.Services.RedirectUrlService;
            this.mapper = Mapper.Engine;
            this.logger = this.Logger;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedirectsApiController"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="redirectUrlService">
        /// The redirect url service.
        /// </param>
        /// <param name="mapper">
        /// The mapper.
        /// </param>
        public RedirectsApiController(
            UmbracoContext context,
            IRedirectUrlService redirectUrlService,
            IMappingEngine mapper,
            ILogger logger) : base(context)
        {
            this.redirectUrlService = redirectUrlService;
            this.mapper = mapper;
            this.logger = logger;
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

            var redirects = this.mapper.Map<IEnumerable<ContentRedirectUrl>>(this.redirectUrlService.GetContentRedirectUrls(contentKey)).ToArray();

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
                this.redirectUrlService.Delete(id);

                return this.Request.CreateNotificationSuccessResponse("Redirect is deleted");
            }
            catch (Exception e)
            {
                this.logger.Error(this.GetType(), "Error deleting redirect", e);
                return this.Request.CreateNotificationValidationErrorResponse("Unexpected error deleting redirect");
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
                this.redirectUrlService.Register(redirect.Url, redirect.ContentKey);
                return this.Request.CreateNotificationSuccessResponse("Redirect is created");
            }
            catch (Exception ex)
            {
                this.logger.Error(this.GetType(), "Error creating redirect", ex);
                return this.Request.CreateNotificationValidationErrorResponse("Unexpected error creating redirect");
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
            return UmbracoConfig.For.UmbracoSettings().WebRouting.DisableRedirectUrlTracking;
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
            if (redirect.Url.StartsWith("http://"))
            {
                return "Url should be without domain";
            }

            if (redirect.Url.Contains("."))
            {
                return "Url should be without .";
            }

            if (redirect.Url.Contains(" "))
            {
                return "Url should be without spaces";
            }


            // make sure we have a valid url
            redirect.Url = redirect.Url.EnsureStartsWith("/").TrimEnd("/");

            return string.Empty;
        }
    }
}
