using System;
using PS.Core.Extensions;
using PS.Core.Models;

namespace PS.Core.Http
{
    public class KnownHeader
    {
        internal ByteString String8;

        public string String;

        private KnownHeader(string str)
        {
            String8 = (ByteString)str;
            String = str;
        }

        public override string ToString()
        {
            return String;
        }

        internal bool Equals(ReadOnlySpan<char> value)
        {
            return String.AsSpan().EqualsIgnoreCase(value);
        }

        internal bool Equals(string? value)
        {
            return String.EqualsIgnoreCase(value);
        }

        public static implicit operator KnownHeader(string str) => new KnownHeader(str);
    }
}
