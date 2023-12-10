using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiveWaitlistServer.Configuration
{
    public class AesOptions
    {
        public const string KeyName = "Aes";

        public string Key { get; set; } = string.Empty;
    }
}