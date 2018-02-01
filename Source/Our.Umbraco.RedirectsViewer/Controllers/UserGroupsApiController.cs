namespace Our.Umbraco.RedirectsViewer.Controllers
{
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;

    using global::Umbraco.Core.Services;
    using global::Umbraco.Web.Editors;

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
        /// <param name="userService">
        /// The user service.
        /// </param>
        public UserGroupsApiController(IUserService userService)
        {
            this.userService = userService;
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
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }
    }
}
