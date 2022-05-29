using PS.Core.Network.Tcp;

namespace PS.Core.EventArguments
{
    public class EmptyProxyEventArgs : ProxyEventArgsBase
    {
        internal EmptyProxyEventArgs(ProxyServer server, TcpClientConnection clientConnection) : base(server, clientConnection)
        {
        }
    }
}
