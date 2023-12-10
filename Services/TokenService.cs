using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LiveWaitlistServer.Configuration;
using LiveWaitlistServer.Model;
using LiveWaitlistServer.Services;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LiveWaitlistServer.Shared.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtOptions _jwtOptions;

        public TokenService(IOptions<JwtOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
        }

        public string GenerateToken(bool isAdmin, WaitlistHost waitlistHost)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtOptions.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("waitlist_code", waitlistHost.WaitlistCode),
                    new Claim(ClaimTypes.Role, isAdmin ? "Admin" : "User")
                }),
                Expires = DateTime.UtcNow.AddHours(12),
                Issuer = _jwtOptions.Issuer,
                Audience = _jwtOptions.Audience,
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }   
    }
}