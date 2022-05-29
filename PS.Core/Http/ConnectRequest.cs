using PS.Core.Models;
using PS.Core.StreamExtended;

namespace PS.Core.Http
{
    /// <summary>
    /// The tcp tunnel Connect request.
    /// </summary>
    public class ConnectRequest : Request
    {
        internal ConnectRequest(ByteString authority)
        {
            Method = "CONNECT";
            Authority = authority;
        }

        public TunnelType TunnelType { get; internal set; }

        public ClientHelloInfo? ClientHelloInfo { get; set; }
    }
}
