using System;

namespace Our.Umbraco.RedirectsViewer.Models.Events
{
    public class RedirectDeletedArgs : EventArgs
    {
        public Guid Id { get; }

        public RedirectDeletedArgs(Guid id)
        {
            Id = id;
        }
    }
}