namespace Our.Umbraco.RedirectsViewer.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;

    using AutoMapper;

    using global::Umbraco.Web.Editors;
    using global::Umbraco.Web.Models.ContentEditing;

    /// <summary>
    /// The redirects api controller.
    /// </summary>
    public class RedirectsApiController : BackOfficeNotificationsController
    {
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
            var redirectService = this.Services.RedirectUrlService;

            var redirects = Mapper.Map<IEnumerable<ContentRedirectUrl>>(redirectService.GetContentRedirectUrls(contentKey)).ToArray();

            return this.Request.CreateResponse(HttpStatusCode.OK, redirects);
        }
    }
}
