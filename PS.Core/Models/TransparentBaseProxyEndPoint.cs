using System.Net;
using System.Threading.Tasks;
using PS.Core.EventArguments;

namespace PS.Core.Models
{
    public abstract class TransparentBaseProxyEndPoint : ProxyEndPoint
    {   
        /// <summary>
        /// The hostname of the generic certificate to negotiate SSL.
        /// This will be only used when Sever Name Indication (SNI) is not supported by client, 
        /// or when it does not indicate any host name.
        /// </summary>
        public abstract string GenericCertificateName { get; set; }

        protected TransparentBaseProxyEndPoint(IPAddress ipAddress, int port, bool decryptSsl) : base(ipAddress, port, decryptSsl)
        {
        }

        internal abstract Task InvokeBeforeSslAuthenticate(ProxyServer proxyServer,
            BeforeSslAuthenticateEventArgs connectArgs, ExceptionHandler? exceptionFunc);
    }
}
