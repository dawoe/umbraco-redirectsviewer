using System.Collections.Generic;

namespace Our.Umbraco.RedirectsViewer.Models.Import.File
{
    public interface IRedirectsFile
    {
        string FileName { get; }

        void Load();


        List<RedirectItem> Redirects { get; }

    }
}
