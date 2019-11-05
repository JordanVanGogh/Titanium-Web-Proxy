using System;
using System.Net;

namespace Titanium.Web.Proxy.EventArguments
{
    public class ClientConnectionCountChangedEventArgs : EventArgs
    {
        public Guid ConnectionId { get; set; }

        public int UpdatedCount { get; set; }

        public EndPoint LocalEndPoint { get; set; }

        public EndPoint RemoteEndPoint { get; set; }
    }
}
