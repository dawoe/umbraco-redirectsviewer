using System;
using System.Collections.Generic;
using Umbraco.Core.Models;
using Umbraco.Core.Models.ContentEditing;
using Umbraco.Core.Models.Membership;

namespace Our.Umbraco.RedirectsViewer.Components
{
    internal class RedirectsContentAppFactory : IContentAppFactory
    {
        public ContentApp GetContentAppFor(object source, IEnumerable<IReadOnlyUserGroup> userGroups)
        {
            var contentBase = source as IContentBase;

            if (contentBase == null)
            {
                return null;
            }

            if (contentBase.Id == 0)
            {
                return null;
            }
            

            return new ContentApp
            {
                Alias = "redirects",
                Icon = "icon-trafic",
                Name = "Redirects",
                View = "/App_Plugins/RedirectsViewer/views/editor.html",                
            };
        }
    }
}
