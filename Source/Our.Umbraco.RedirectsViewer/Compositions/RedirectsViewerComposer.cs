using Umbraco.Core.Composing;
using Umbraco.Core;
using Our.Umbraco.RedirectsViewer.Components;

namespace Our.Umbraco.RedirectsViewer.Compositions
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    internal class RedirectsViewerComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Components().Append<ServerVariableRegistrationComponent>();
        }
    }
}
