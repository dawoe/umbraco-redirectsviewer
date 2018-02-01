namespace Our.Umbraco.RedirectsViewer.Controllers
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;

    using AutoMapper;

    using global::Umbraco.Core.Services;
    using global::Umbraco.Web.Editors;

    using Our.Umbraco.RedirectsViewer.Models;

    /// <summary>
    /// User groups api controller
    /// </summary>
    internal class UserGroupsApiController : BackOfficeNotificationsController
    {
        /// <summary>
        /// The user service.
        /// </summary>
        private readonly IUserService userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserGroupsApiController"/> class.
        /// </summary>      
        public UserGroupsApiController()
        {
            this.userService = this.Services.UserService;
        }

        /// <summary>
        /// The get user groups.
        /// </summary>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [HttpGet]
        public HttpResponseMessage GetUserGroups()
        {
            return this.Request.CreateResponse(HttpStatusCode.OK, Mapper.Map<IEnumerable<UserGroupDisplay>>(this.userService.GetAllUserTypes()));
        }
    }
}
