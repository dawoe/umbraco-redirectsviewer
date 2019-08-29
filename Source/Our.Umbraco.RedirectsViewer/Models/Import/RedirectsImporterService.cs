using System;
using System.Collections.Generic;
using System.Linq;
using Our.Umbraco.RedirectsViewer.Hubs;
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
        public virtual ImporterResponse Import(IRedirectsFile file,string clientId)
        {
            var response = new ImporterResponse();

            file.Load();
            
            response.File = file;
            response.ImportedItems = file.Redirects;
            var domains = this._domainService.GetAll(true).Where(x => x.RootContentId.HasValue).ToList();
            List<Tuple<int, string, string>> list = new List<Tuple<int, string, string>>();
            int redirects = 0;
            foreach (var redirect in file.Redirects)
            {
                if (domains.Any())
                {
                    try
                    {
                        var status = _redirectService.AddRedirect(redirect.Content, domains, redirect.Url);
                        list.Add(new Tuple<int, string, string>(status, redirect.Url, redirect.Target));

                        redirects++;
                    }
                    catch (Exception e)
                    {
                        list.Add(new Tuple<int, string, string>(3, redirect.Url, redirect.Target));
                    }
                    var hubClient = new HubClientService(clientId);
                    hubClient.SendUpdate(new
                    {
                        Message = list,
                        Count = redirects, 
                        Total = file.Redirects.Count
                    });
                   
                }
            }
            response.StatusImportItems = list.OrderBy(e=>e.Item1).ToList();

            RaiseEvent(response);

            return response;
        }
    }
}