using System;
using System.IO;
using System.Net;
#if NETCOREAPP2_1
using System.Net.Security;
#endif
using System.Net.Sockets;
using System.Threading.Tasks;
using Titanium.Web.Proxy.Extensions;

namespace Titanium.Web.Proxy.Network.Tcp
{
    /// <summary>
    ///     An object that holds TcpConnection to a particular server and port
    /// </summary>
    internal class TcpClientConnection : IDisposable
    {
        internal TcpClientConnection(ProxyServer proxyServer, TcpClient tcpClient, int readTimeout, int writeTimeout)
        {
            this.tcpClient = tcpClient;
            this.proxyServer = proxyServer;
            this.proxyServer.UpdateClientConnectionCount(true, Id, LocalEndPoint, RemoteEndPoint);
            this.ReadTimeout = readTimeout;
            this.WriteTimeout = writeTimeout;
        }

        private ProxyServer proxyServer { get; }

        public Guid Id { get; } = Guid.NewGuid();

        public int ReadTimeout { get; }

        public int WriteTimeout { get; }

        public EndPoint LocalEndPoint => tcpClient?.Client?.LocalEndPoint;

        public EndPoint RemoteEndPoint => tcpClient?.Client?.RemoteEndPoint;

        internal SslApplicationProtocol NegotiatedApplicationProtocol { get; set; }

        private readonly TcpClient tcpClient;

        public Stream GetStream()
        {
            var ns = tcpClient.GetStream();
            ns.ReadTimeout = this.ReadTimeout;
            ns.WriteTimeout = this.WriteTimeout;
            return ns;
        }

        /// <summary>
        ///     Dispose.
        /// </summary>
        public void Dispose()
        {
            Guid id = Guid.Empty;
            try
            {
                id = Id;
            }
            catch { }

            EndPoint localEndPoint = null;
            try
            {
                localEndPoint = LocalEndPoint;
            }
            catch { }

            EndPoint remoteEndPoint = null;
            try
            {
                remoteEndPoint = RemoteEndPoint;
            }
            catch { }

            Task.Run(async () =>
            {
                //delay calling tcp connection close()
                //so that client have enough time to call close first.
                //This way we can push tcp Time_Wait to client side when possible.
                try
                {
                    await Task.Delay(1000);
                }
                catch { }

                try
                {
                    proxyServer.UpdateClientConnectionCount(false, id, localEndPoint, remoteEndPoint);
                }
                catch
                {
                    proxyServer.UpdateClientConnectionCount(false, id);
                }
                tcpClient.CloseSocket();
            });
           
        }
    }
}
