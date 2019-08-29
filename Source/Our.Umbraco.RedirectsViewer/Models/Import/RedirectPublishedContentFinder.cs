using System;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Umbraco.Web.PublishedCache;

namespace Our.Umbraco.RedirectsViewer.Models.Import
{
    /// <summary>
    /// This class only really exists so I can test the looking up of Umbraco nodes without loads of mocking 
    /// out the Umbraco Context which is a faff
    /// </summary>
    public class RedirectPublishedContentFinder : IRedirectPublishedContentFinder
    {
        private IPublishedContentCache  publishedCache { get; }

        public RedirectPublishedContentFinder(IPublishedContentCache publishedCache)
        {
            this.publishedCache = publishedCache;
        }

        public IPublishedContent Find(string url)
        {
            return publishedCache.GetByRoute(false,  new Uri(url).LocalPath.TrimEnd('/'), false);
        }
        public IPublishedContent Find(int id)
        {
            return publishedCache.GetById(false, id);
        }
        public IPublishedContent Find(Guid id)
        {
            return publishedCache.GetById(false, id);
        }
    }
}