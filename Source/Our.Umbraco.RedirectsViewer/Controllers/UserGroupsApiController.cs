using System;
using Newtonsoft.Json;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Configuration;
using Umbraco.Core.Configuration.UmbracoSettings;
using Umbraco.Core.Logging;
using Umbraco.Core.Models.Membership;
using Umbraco.Core.Persistence;

namespace Our.Umbraco.RedirectsViewer.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;


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

        private readonly IKeyValueService _keyValueService;

        private Guid _key = new Guid("4cf3ae6f-dbde-42de-be3f-6e8f86f55381");

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
            IUserService userService,IKeyValueService keyValueService) : base(globalSettings, umbracoContextAccessor, sqlContext, services, appCaches, logger, runtimeState, umbracoHelper)
        {
            _userService = userService;
            _keyValueService = keyValueService;
           
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
                model = Map(allUserTypes);
            }

            return Request.CreateResponse(HttpStatusCode.OK, model);
        }

        [HttpPost]
        public HttpResponseMessage SaveConfig(IEnumerable<RedirectSettings> settings)
        {
            try
            {
                _keyValueService.SetValue("redirectSettings_" + _key, JsonConvert.SerializeObject(settings));

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError,ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetConfig()
        {

            var settings = _keyValueService.GetValue("redirectSettings_" + _key);

            IEnumerable<RedirectSettings> model;
            
            if (settings != null)
            {
                model = JsonConvert.DeserializeObject<List<RedirectSettings>>(settings);
            }
            else
            {
                model = CreateEmptySettings();
            }


            return Request.CreateResponse(HttpStatusCode.OK, model);
        }

        private List<RedirectSettings> CreateEmptySettings()
        {
            List<RedirectSettings> settings = new List<RedirectSettings>();

            RedirectSettings createSettings = new RedirectSettings("createAllowed");

            RedirectSettings deleteSettings = new RedirectSettings("deleteAllowed");

            settings.Add(createSettings);
            settings.Add(deleteSettings);

            return settings;
        }

        private List<UserGroupDisplay> Map(List<IUserGroup> allUserTypes)
        {
            List<UserGroupDisplay>  model = new List<UserGroupDisplay>();

            foreach (var item in allUserTypes)
            {
                UserGroupDisplay ug = new UserGroupDisplay {Name = item.Name, Alias = item.Alias};
                model.Add(ug);
            }

            return model;
        }
    }
}
