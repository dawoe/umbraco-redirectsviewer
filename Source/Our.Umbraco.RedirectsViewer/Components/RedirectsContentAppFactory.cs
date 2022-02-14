using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.ContentEditing;
using Umbraco.Cms.Core.Models.Membership;

namespace Our.Umbraco.RedirectsViewer.Components
{
    internal class RedirectsContentAppFactory : IContentAppFactory
    {
        private readonly IOptionsMonitor<WebRoutingSettings> _webRoutingSettings;

        public RedirectsContentAppFactory( IOptionsMonitor<WebRoutingSettings> webRoutingSettings)
        {
            _webRoutingSettings = webRoutingSettings;
        }

        public ContentApp GetContentAppFor(object source, IEnumerable<IReadOnlyUserGroup> userGroups)
        {
            if(this.IsUrlTrackingDisabled()) 
            {
                return null;
            }
            
            var contentBase = source as IContentBase;                      

            if (contentBase == null)
            {
                return null;
            }

            if (contentBase.Id == 0)
            {
                return null;
            }

            if (contentBase is IContent)
            {
                return new ContentApp
                {
                    Alias = "redirects",
                    Icon = "icon-trafic",
                    Name = "Redirects",
                    View = "/App_Plugins/RedirectsViewer/views/editor.html",
                };
            }

            return null;
        }

        private bool IsUrlTrackingDisabled()
        {
            return _webRoutingSettings.CurrentValue.DisableRedirectUrlTracking;
        }
    }
}
