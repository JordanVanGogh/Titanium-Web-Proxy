using System;
using Titanium.Web.Proxy.Http;
using Titanium.Web.Proxy.Shared;

namespace Titanium.Web.Proxy.Helpers
{
    internal static class SshHelper
    {
        internal static bool IsSshConnectRequest(ConnectRequest connectRequest)
        {
            if (connectRequest == null) throw new ArgumentNullException(nameof(connectRequest));

            return connectRequest.RequestUri?.Port == ProxyConstants.SshPort;
        }
    }
}
