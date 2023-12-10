using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiveWaitlistServer.Data;
using LiveWaitlistServer.Model.DTO;
using LiveWaitlistServer.Services;
using LiveWaitlistServer.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiveWaitlistServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class LoginController : ControllerBase
    {
        private readonly IWaitlistHostRepository _waitlistHostRepository;
        private readonly ITokenService _tokenService;
        private readonly IEncryptionService _encryptionService;

        public LoginController(IWaitlistHostRepository waitlistHostRepository, ITokenService tokenService, IEncryptionService encryptionService)
        {
            _waitlistHostRepository = waitlistHostRepository;
            _tokenService = tokenService;
            _encryptionService = encryptionService;
        }

        [HttpPost]
        [Route("token")]
        public ActionResult<AdminLoginResult> CreateAdminToken([FromBody] AdminLoginRequest request)
        {
            var host = _waitlistHostRepository.Get(request.WaitlistCode);

            if (host == null)
                return Unauthorized();

            if (_encryptionService.Decrypt(host.Password) == request.Password)
            {
                var token = _tokenService.GenerateToken(true, host);
                var result = new AdminLoginResult()
                {
                    Token = token
                };

                return Ok(result);
            }

            return Unauthorized();
        }
    }
}