using System;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.RedirectsViewer.Models.Import
{
    public interface IRedirectPublishedContentFinder
    {
        IPublishedContent Find(string url);
        IPublishedContent Find(int url);
        IPublishedContent Find(Guid url);

        
    }
}