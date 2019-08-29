using System;
using System.Collections.Generic;
using System.Linq;
using Our.Umbraco.RedirectsViewer.Models.Import.File;

namespace Our.Umbraco.RedirectsViewer.Models.Import
{
    public class ImporterResponse
    {
        public ImporterResponse()
        {
            ImportedItems = Enumerable.Empty<RedirectItem>();
        }

        public IRedirectsFile File { get; set; }

        public IEnumerable<RedirectItem> ImportedItems { get; set;  }
        public List<Tuple<int, string, string>> StatusImportItems { get; set; }
    }
}