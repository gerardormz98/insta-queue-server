using LiveWaitlistServer.Model;
using LiveWaitlistServer.Model.DTO;
using LiveWaitlistServer.Services.Interfaces;
using Sqids;

namespace LiveWaitlistServer.Data
{
    public class WaitlistHostRepositoryInMemory : IWaitlistHostRepository
    {
        private readonly ILiveWaitlistManager _waitlistManager;
        private readonly IWaitlistCodeService _waitlistCodeService;
        private readonly IEncryptionService _encryptionService;

        // TODO: Get this from DB
        private List<WaitlistHost> WaitlistHosts { get; set; } = new List<WaitlistHost>() 
        {
            new WaitlistHost() {
                WaitlistCode = "AAAAAA",
                HostName = "Pizza Palace",
                HostDescription = "We're the best pizza restaurant in the world!",
                Password = "psx9Ei3Y7uLhR4TEEBnhcQ==" //123456a
            },
            new WaitlistHost() {
                WaitlistCode = "BBBBBB",
                HostName = "Papa's Taquería",
                HostDescription = "We have the best tacos of the city!",
                Password = "pHGo8JWlSxMOPWQHnJ+PTg==" //a654321
            }
        };

        public WaitlistHostRepositoryInMemory(ILiveWaitlistManager waitlistManager, IWaitlistCodeService waitlistCodeService, IEncryptionService encryptionService)
        {
            _waitlistManager = waitlistManager;
            _waitlistCodeService = waitlistCodeService;
            _encryptionService = encryptionService;
        }

        public int GetCount()
        {
            return WaitlistHosts.Count;
        }

        public WaitlistHost? Get(string code) 
        {
            var host = WaitlistHosts.FirstOrDefault(h => h.WaitlistCode == code);

            if (host != null)
                host.Size = _waitlistManager.GetCurrentSize(host.WaitlistCode);

            return host;
        }
        
        public WaitlistHost Create(WaitlistHostCreateRequest hostRequest)
        {
            string code;

            do
                code = _waitlistCodeService.GenerateCode();
            while (WaitlistHosts.Any(h => h.WaitlistCode == code));
            
            var encryptedPassword = _encryptionService.Encrypt(hostRequest.Password);

            var waitlistHost = new WaitlistHost() 
            {
                WaitlistCode = code,
                HostName = hostRequest.HostName,
                HostDescription = hostRequest.HostDescription,
                Password = encryptedPassword
            };

            WaitlistHosts.Add(waitlistHost);
            return waitlistHost;
        }

        public WaitlistHost? Update(string waitlistCode, WaitlistHostUpdateRequest hostRequest)
        {
            var waitlist = Get(waitlistCode);

            if (waitlist != null)
            {
                waitlist.HostName = hostRequest.HostName;
                waitlist.HostDescription = hostRequest.HostDescription;
            }

            return waitlist;
        }

        public WaitlistHost? Delete(string waitlistCode)
        {
            var waitlist = Get(waitlistCode);

            if (waitlist != null)
            {
                WaitlistHosts.Remove(waitlist);
            }

            return waitlist;
        }
    }
}