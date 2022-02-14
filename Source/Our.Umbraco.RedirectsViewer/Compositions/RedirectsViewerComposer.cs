using Umbraco.Core;
using Our.Umbraco.RedirectsViewer.Components;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;

namespace Our.Umbraco.RedirectsViewer.Compositions
{
    public class RedirectsViewerComposer : IComposer
    {
        public void Compose(IUmbracoBuilder composition)
        {
            composition.AddNotificationHandler<ServerVariablesParsingNotification,ServerVariableRegistrationComponent>();

            composition.ContentApps().Append<RedirectsContentAppFactory>();
        }
    }
}
