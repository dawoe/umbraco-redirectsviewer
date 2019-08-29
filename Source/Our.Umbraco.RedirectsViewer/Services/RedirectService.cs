using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Services;

namespace Our.Umbraco.RedirectsViewer.Services
{
    public class RedirectService
    {
        private readonly IRedirectUrlService _redirectUrlService;


        public RedirectService(IRedirectUrlService redirectUrlService)
        {
            _redirectUrlService = redirectUrlService;
        }

        public int AddRedirect(IContent content, List<IDomain> domains, string url)
        {
            if (content == null)
            {
                throw new Exception("Content does not exist");
            }

            return AddRedirect(domains, url, content.Path, content.Key);
        }
        public int AddRedirect(IPublishedContent content, List<IDomain> domains, string url)
        {
            if (content == null)
            {
                return 1;
            }

            return AddRedirect(domains, url, content.Path, content.Key);
        }

        public int AddRedirect(List<IDomain> domains, string url,string path, Guid key)
        {
            var redirectUrl = url;
            var rootNode = string.Empty;

            // get all the domains that have a root content id set
          

            if (!string.IsNullOrEmpty(rootNode))
            {
                // prefix the url with the root content node
                redirectUrl = rootNode + url;
            }

            // check if there is already a redirect with the url
            long total;
            var redirects = this._redirectUrlService.GetAllRedirectUrls(0, int.MaxValue, out total);
         

            // get all the ids in the path
            var pathIds = path.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            if (pathIds.Any())
            {
                // find a domain that is in the path of the item
                var assignedDomain = domains.FirstOrDefault(x => pathIds.Contains(x.RootContentId.Value.ToString()));

                if (assignedDomain != null)
                {
                    // get the root content node
                    rootNode = assignedDomain.RootContentId.Value.ToString();
                }
            }

            if (!string.IsNullOrEmpty(rootNode))
            {
                redirectUrl = rootNode + url;
            }
            if (redirects.Any(x => x.Url == redirectUrl))
            {
                return 2;
            }
            this._redirectUrlService.Register(redirectUrl, key);
            return 0;
        
        }
    }
}