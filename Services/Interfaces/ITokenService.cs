using LiveWaitlistServer.Model;

namespace LiveWaitlistServer.Services
{
    public interface ITokenService
    {
        string GenerateToken(bool isAdmin, WaitlistHost waitlistHost);
    }
}