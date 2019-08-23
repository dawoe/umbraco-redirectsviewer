using System.Collections.Generic;
using Our.Umbraco.RedirectsViewer.Models.Import;

namespace Skybrud.Umbraco.Redirects.Models.Import.File
{
    public interface IRedirectsFile
    {
        string FileName { get; }

        void Load();


        List<RedirectItem> Redirects { get; }

    }
}
