using Our.Umbraco.RedirectsViewer.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace Our.Umbraco.RedirectsViewer.Components
{
    internal class ServerVariableRegistrationComponent : INotificationHandler<ServerVariablesParsingNotification>
    {
        public ServerVariableRegistrationComponent(urlhe)
        {
        }

        private static void SetUpDictionaryForAngularPropertyEditor(Dictionary<string, object> e)
        {
            var urlHelper = new UrlHelper(new RequestContext(new HttpContextWrapper(HttpContext.Current), new RouteData()));

            var urlDictionairy = new Dictionary<string, object>
            {
                {
                    "UserGroupApi",
                    urlHelper.GetUmbracoApiServiceBaseUrl<UserGroupsApiController>(c => c.GetUserGroups())
                },
                {
                    "RedirectsApi", urlHelper.GetUmbracoApiServiceBaseUrl<RedirectsApiController>(c =>
                        c.GetRedirectsForContent(Guid.Empty, string.Empty))
                }
            };


            if (!e.Keys.Contains("Our.Umbraco.RedirectsViewer"))
            {
                e.Add("Our.Umbraco.RedirectsViewer", urlDictionairy);
            }
        }

        public void Handle(ServerVariablesParsingNotification notification)
        {
            if (HttpContext.Current == null)
            {
                return;
            }

            SetUpDictionaryForAngularPropertyEditor(e);
        }
    }
}
