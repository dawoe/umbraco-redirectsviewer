using System;
using AutoMapper;
using Our.Umbraco.RedirectsViewer.Mapping;
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Our.Umbraco.RedirectsViewer.Compositions
{
    public class UmbracoStartup:IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Register<Profile, UserGroupProfile>();
        }
    }
}
