using LiveWaitlistServer.Services.Interfaces;
using Sqids;

namespace LiveWaitlistServer.Services
{
    public class WaitlistCodeService : IWaitlistCodeService
    {
        public WaitlistCodeService() { }

        public string GenerateCode()
        {
            var sqidEncoder = new SqidsEncoder<int>(new()
            {
                Alphabet = "LRPDACUYKBVOFMIZSHWGXJETNQ",
                MinLength = 6
            });

            var code = sqidEncoder.Encode(new Random().Next(9000000));

            return code;
        }
    }
}