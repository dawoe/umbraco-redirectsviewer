using System;
using System.Collections.Generic;
using System.Linq;
using Our.Umbraco.RedirectsViewer.Models.Import;
using Skybrud.Umbraco.Redirects.Extensions;
using Skybrud.Umbraco.Redirects.Import.Csv;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace Skybrud.Umbraco.Redirects.Models.Import.File
{
    public class CsvRedirectsFile : IRedirectsFile
    {
        public CsvRedirectsFile()
        {
            Seperator = CsvSeparator.Comma;
        }

        private readonly IRedirectPublishedContentFinder contentFinder;

        public CsvRedirectsFile(IRedirectPublishedContentFinder contentFinder)
        {
            this.contentFinder = contentFinder;
        }

        public string FileName { get; set; }

        public CsvSeparator Seperator { get; set; }

        public CsvFile File { get; private set; }

        public List<RedirectItem> Redirects { get; private set; }


        /// <summary>
        /// Loads, parses and validates redirects
        /// </summary>
        public void Load()
        {
            File = CsvFile.Load(FileName, Seperator);

            File.Columns.AddColumn("Status");
            File.Columns.AddColumn("ErrorMessage");

            Redirects = File.Rows.Select(Parse).ToList();

        }

        /// <summary>
        /// Validates using task chains.
        /// </summary>
       

       

        /// <summary>
        /// This is where a CsvRow gets parsed into a RedirectItem. The aim here is not to validate but 
        /// to get everything into a nicely typed model. It's not pretty mainly because of old skool 
        /// null checks.
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private RedirectItem Parse(CsvRow row)
        {
            var redirectItem = new RedirectItem();



            var sourceUrlRaw = row.Cells[0] == null ? null : row.Cells[0].Value.Replace("\"",string.Empty).Trim();

            var sourceUrl = sourceUrlRaw.ToUri();

            if (sourceUrl != null)
            {
                var lastSlash = sourceUrl.AbsolutePath.LastIndexOf('/');
                var sourceUrlNoTrailingSlash = (lastSlash > 0) ? sourceUrl.AbsolutePath.Substring(0, lastSlash) : sourceUrl.AbsolutePath;

                redirectItem.Url = sourceUrlNoTrailingSlash;
            }

            var destinationRaw = row.Cells[1] == null ? null : row.Cells[1].Value.Replace("\"", string.Empty).Trim();
            IPublishedContent content;
            if (int.TryParse(destinationRaw,out int id))
            {
                content = contentFinder.Find(id);
            }
            else if (Guid.TryParse(destinationRaw,out Guid guid))
            {
                content = contentFinder.Find(guid);
            }
            else
            {
                content = contentFinder.Find(destinationRaw);
            }
            
                redirectItem.Content = content;
            return redirectItem;
        }
    }
}