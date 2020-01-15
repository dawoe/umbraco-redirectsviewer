using System;

namespace Our.Umbraco.RedirectsViewer.Models.Events
{
    public class RedirectAdded:EventArgs
    {
        public string Url { get; }
        public Guid ContentKey { get; }
        public string Culture { get; }

        public RedirectAdded(string url, Guid contentKey, string culture = null)
        {
            Url = url;
            ContentKey = contentKey;
            Culture = culture;
        }
    }
}