#if NET451
using System;
using System.Threading.Tasks;

namespace PS.Core
{
    class Net45Compatibility
    {
        public static byte[] EmptyArray = new byte[0];

        public static Task CompletedTask = Task.FromResult<object>(null);
    }
}
#endif
