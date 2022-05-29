using System;
using PS.Core.Network.Tcp;

namespace PS.Core.EventArguments
{
    /// <summary>
    ///     The base event arguments
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public abstract class ProxyEventArgsBase : EventArgs
    {
        private readonly TcpClientConnection clientConnection;
        internal readonly ProxyServer Server;
        public object ClientUserData
        {
            get => clientConnection.ClientUserData;
            set => clientConnection.ClientUserData = value;
        }

        internal ProxyEventArgsBase(ProxyServer server, TcpClientConnection clientConnection)
        {
            this.clientConnection = clientConnection;
            this.Server = server;
        }
    }
}
