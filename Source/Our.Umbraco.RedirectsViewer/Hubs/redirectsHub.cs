using System;
using Microsoft.AspNet.SignalR;

namespace Our.Umbraco.RedirectsViewer.Hubs
{
    public class RedirectsHub : Hub
    {
        public string GetTime()
        {
            return DateTime.Now.ToString();
        }
    }
}
