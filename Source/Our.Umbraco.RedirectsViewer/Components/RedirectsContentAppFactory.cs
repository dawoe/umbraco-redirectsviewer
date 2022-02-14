using System;
using System.Collections.Generic;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.ContentEditing;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Core.Configuration.UmbracoSettings;
using Umbraco.Core.Models;
using Umbraco.Core.Models.ContentEditing;
using Umbraco.Core.Models.Membership;

namespace Our.Umbraco.RedirectsViewer.Components
{
    internal class RedirectsContentAppFactory : IContentAppFactory
    {
        private readonly IUmbracoSettingsSection _umbracoSettings;

        public RedirectsContentAppFactory(IUmbracoSettingsSection umbracoSettings)
        {
            this._umbracoSettings = umbracoSettings;
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
            return _umbracoSettings.WebRouting.DisableRedirectUrlTracking;
        }
    }
}
