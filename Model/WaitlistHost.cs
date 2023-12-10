namespace LiveWaitlistServer.Model
{
    public class WaitlistHost
    {
        public string WaitlistCode { get; set; } = "";
        public string HostName { get; set; } = "";
        public string HostDescription { get; set; } = "";
        public int Size { get; set; }
        public string Password { get; set; } = string.Empty;
    }
}