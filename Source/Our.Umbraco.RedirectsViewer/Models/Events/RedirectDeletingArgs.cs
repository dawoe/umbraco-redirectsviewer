using System;

namespace Our.Umbraco.RedirectsViewer.Models.Events
{
    public class RedirectDeletingArgs:RedirectDeletedArgs
    {
        public RedirectDeletingArgs(Guid id, string culture) : base(id, culture)
        {
        }
    }
}