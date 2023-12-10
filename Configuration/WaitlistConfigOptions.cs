using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiveWaitlistServer.Configuration
{
    public class WaitlistConfigOptions
    {
        public const string KeyName = "WaitlistConfig";

        public int MaxHosts { get; set; }
        public int MaxUsersEnqueued { get; set; }
    }
}