using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS.Core
{
    public static class ProxyAuthenticationSchemeList
    {
        public static string[] Basic => new string[] { nameof(Basic).ToLower() };
        public static string[] NTLM => new string[] { nameof(NTLM).ToLower() };
        public static string[] Kerberos => new string[] { nameof(Kerberos).ToLower() };
        public static string[] Negotiate => new string[] { nameof(Negotiate).ToLower() };
    }
}
