using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiveWaitlistServer.Model.DTO
{
    public class WaitlistHostResult
    {
        public string WaitlistCode { get; set; } = "";
        public string HostName { get; set; } = "";
        public string HostDescription { get; set; } = "";
        public int Size { get; set; }
    }
}