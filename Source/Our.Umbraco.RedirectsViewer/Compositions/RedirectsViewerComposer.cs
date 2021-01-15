using Umbraco.Core.Composing;
using Umbraco.Core;
using Our.Umbraco.RedirectsViewer.Components;
using Umbraco.Web;

namespace Our.Umbraco.RedirectsViewer.Compositions
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class RedirectsViewerComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Components().Append<ServerVariableRegistrationComponent>();

            composition.ContentApps().Append<RedirectsContentAppFactory>();
        }
    }
}
