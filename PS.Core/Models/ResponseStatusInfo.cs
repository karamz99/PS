using System;

namespace PS.Core.Helpers
{
    struct ResponseStatusInfo
    {
        public Version Version { get; set; }

        public int StatusCode { get; set; }

        public string Description { get; set; }
    }
}
