using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Configuration;
using Umbraco.Core.Configuration.UmbracoSettings;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;

namespace Our.Umbraco.RedirectsViewer.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using AutoMapper;

    using global::Umbraco.Core.Services;
    using global::Umbraco.Web;
    using global::Umbraco.Web.Editors;

    using Models;

    /// <summary>
    /// User groups api controller
    /// </summary>
    public class UserGroupsApiController : BackOfficeNotificationsController
    {
        /// <summary>
        /// The user service.
        /// </summary>
        private readonly IUserService _userService;

        /// <summary>
        /// The mapper.
        /// </summary>
        private readonly IMappingEngine _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserGroupsApiController"/> class.
        /// </summary>      
        public UserGroupsApiController(IUmbracoSettingsSection umbracoSettings,
            IGlobalSettings globalSettings,
            IUmbracoContextAccessor umbracoContextAccessor,
            ISqlContext sqlContext,
            ServiceContext services,
            AppCaches appCaches,
            IProfilingLogger logger,
            IRuntimeState runtimeState,
            UmbracoHelper umbracoHelper, 
            IMappingEngine mapper,IUserService userService) : base(globalSettings, umbracoContextAccessor, sqlContext, services, appCaches, logger, runtimeState, umbracoHelper)
        {
            _userService = userService;
            _mapper = mapper;
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
            var allUserTypes = this._userService.GetAllUserGroups().OrderBy(x => x.Name).ToList();
          
            // remove admin group
            allUserTypes.RemoveAll(x => x.Alias == "admin");

            var model = new List<UserGroupDisplay>();

            if (allUserTypes.Any())
            {
                model = this._mapper.Map<IEnumerable<UserGroupDisplay>>(allUserTypes).ToList();
            }

            return this.Request.CreateResponse(HttpStatusCode.OK, model);
        }
    }
}
