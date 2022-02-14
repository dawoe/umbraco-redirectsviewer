using Our.Umbraco.RedirectsViewer.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Extensions;

namespace Our.Umbraco.RedirectsViewer.Components
{
    internal class ServerVariableRegistrationComponent : INotificationHandler<ServerVariablesParsingNotification>
    {
        private readonly LinkGenerator _linkGenerator;

        public ServerVariableRegistrationComponent(LinkGenerator linkGenerator)
        {
            _linkGenerator = linkGenerator;
        }

        private void SetUpDictionaryForAngularPropertyEditor(IDictionary<string, object> e)
        {
          
            var urlDictionairy = new Dictionary<string, object>
            {
                {
                    "UserGroupApi",
                    _linkGenerator.GetUmbracoApiServiceBaseUrl<UserGroupsApiController>(c => c.GetUserGroups())
                },
                {
                    "RedirectsApi", _linkGenerator.GetUmbracoApiServiceBaseUrl<RedirectsApiController>(c =>
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
            SetUpDictionaryForAngularPropertyEditor(notification.ServerVariables);
        }
    }
}
