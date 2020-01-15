using System;

namespace Our.Umbraco.RedirectsViewer.Models.Events
{
    public class RedirectAddedArgs:EventArgs
    {
        public string Url { get; }
        public Guid ContentKey { get; }
        public string Culture { get; }

        public RedirectAddedArgs(string url, Guid contentKey, string culture = null)
        {
            Url = url;
            ContentKey = contentKey;
            Culture = culture;
        }
    }
}