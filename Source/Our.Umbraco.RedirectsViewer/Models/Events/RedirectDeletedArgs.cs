using System;

namespace Our.Umbraco.RedirectsViewer.Models.Events
{
    public class RedirectDeletedArgs : EventArgs
    {
        public Guid Id { get; }
        public string Culture { get; }

        public RedirectDeletedArgs(Guid id,string culture)
        {
            Id = id;
            Culture = culture;
        }
    }
}