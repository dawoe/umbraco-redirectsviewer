namespace Our.Umbraco.RedirectsViewer.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Xml.Linq;

    using AutoMapper;

    using global::Umbraco.Core.Services;
    using global::Umbraco.Web;
    using global::Umbraco.Web.Editors;

    using Our.Umbraco.RedirectsViewer.Models;

    /// <summary>
    /// User groups api controller
    /// </summary>
    public class UserGroupsApiController : BackOfficeNotificationsController
    {
        /// <summary>
        /// The user service.
        /// </summary>
        private readonly IUserService userService;

        /// <summary>
        /// The mapper.
        /// </summary>
        private readonly IMappingEngine mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserGroupsApiController"/> class.
        /// </summary>      
        public UserGroupsApiController()
        {
            this.userService = this.Services.UserService;
            this.mapper = Mapper.Engine;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserGroupsApiController"/> class.
        /// </summary>
        /// <param name="umbracoContext">
        /// The umbraco Context.
        /// </param>
        /// <param name="userService">
        /// The user service.
        /// </param>
        /// <param name="mapper">
        /// The mapper.
        /// </param>
        internal UserGroupsApiController(UmbracoContext umbracoContext, IUserService userService, IMappingEngine mapper)
            : base(umbracoContext)
        {
            this.userService = userService;
            this.mapper = mapper;
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
            // get all user groups
            var allUserTypes = this.userService.GetAllUserGroups().OrderBy(x => x.Name).ToList();
          
            // remove admin group
            allUserTypes.RemoveAll(x => x.Alias == "admin");

            var model = new List<UserGroupDisplay>();

            if (allUserTypes.Any())
            {
                model = this.mapper.Map<IEnumerable<UserGroupDisplay>>(allUserTypes).ToList();
            }

            return this.Request.CreateResponse(HttpStatusCode.OK, model);
        }
    }
}
