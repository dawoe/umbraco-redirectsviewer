using System;
using System.Collections.Generic;
using System.Linq;
using Our.Umbraco.RedirectsViewer.Services;
using Skybrud.Umbraco.Redirects.Models.Import.File;
using Umbraco.Core.Services;
using Umbraco.Core.Services.Implement;

namespace Our.Umbraco.RedirectsViewer.Models.Import
{
    public class ImportedEventArgs : EventArgs
    {
        public ImportedEventArgs(ImporterResponse response)
        {
            Response = response;
        }

        public ImporterResponse Response { get; private set; }
    }

    /// <summary>
    /// Umbraco style service that handles the imports and raises events
    /// </summary>
    public class RedirectsImporterService
    {
        RedirectService _redirectService;
        IDomainService _domainService;

        public delegate void ImportedHandler(Object sender, ImportedEventArgs e);

        public RedirectsImporterService(RedirectService redirectService, IDomainService domainService)
        {
            _redirectService = redirectService;
            _domainService = domainService;
        }

        public event ImportedHandler Imported;

        public void RaiseEvent(ImporterResponse response)
        {
            var handler = Imported;

            if (handler != null)
            {
                var args = new ImportedEventArgs(response);

                handler(this, args);
            }
        }

        /// <summary>
        /// Imports a redirect file into the Skybrud redirect table
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public virtual ImporterResponse Import(IRedirectsFile file)
        {
            var response = new ImporterResponse();

            file.Load();
            
            response.File = file;
            response.ImportedItems = file.Redirects;
            var domains = this._domainService.GetAll(true).Where(x => x.RootContentId.HasValue).ToList();
            Dictionary<int, Dictionary<string, string>> List = new Dictionary<int, Dictionary<string, string>>();
            foreach (var redirect in file.Redirects)
            {
                if (domains.Any())
                {
                    var status = _redirectService.AddRedirect(redirect.Content, domains, redirect.Url);
                   
                    if (!List.ContainsKey(status))
                    {
                        var urlDictionary = new Dictionary<string,string>();
                        urlDictionary.Add(redirect.Url,redirect.Target);
                        List.Add(status,urlDictionary);
                    }
                    else
                    {
                        List[status].Add(redirect.Url,redirect.Target);
                    }
                   
                }
            }

            response.StatusImportItems = List;

            RaiseEvent(response);

            return response;
        }
    }
}