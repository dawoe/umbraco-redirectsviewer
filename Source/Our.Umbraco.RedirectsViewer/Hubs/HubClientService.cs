using System;
using Microsoft.AspNet.SignalR;

namespace Our.Umbraco.RedirectsViewer.Hubs
{
    public class HubClientService
    {
        private readonly IHubContext hubContext;
        private readonly string clientId;

        public HubClientService(string clientId)
        {
            hubContext = GlobalHost.ConnectionManager.GetHubContext<RedirectsHub>();
            this.clientId = clientId;
        }

        public void SendMessage<TObject>(TObject item)
        {
            if (hubContext != null)
            {
                if (!string.IsNullOrWhiteSpace(clientId))
                {
                    var client = hubContext.Clients.Client(clientId);
                    if (client != null)
                    {
                        client.Add(item);
                        return;
                    }
                }
                hubContext.Clients.All.Add(item);
            }
        }

        public void SendUpdate(Object message)
        {
            if (hubContext != null)
            {
                if (!string.IsNullOrWhiteSpace(clientId))
                {
                    var client = hubContext.Clients.Client(clientId);
                    if (client != null)
                    {
                        client.Update(message);
                        return;
                    }
                }

                hubContext.Clients.All.Update(message);
            }
        }
    }
}
