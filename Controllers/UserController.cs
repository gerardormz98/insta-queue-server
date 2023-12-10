using LiveWaitlistServer.Data;
using Microsoft.AspNetCore.Mvc;

namespace LiveWaitlistServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILiveWaitlistManager _waitlistManager;

        public UserController(ILiveWaitlistManager waitlistManager)
        {
            _waitlistManager = waitlistManager;
        }

        [HttpGet]
        [Route("{userId}/currentWaitlist")]
        public ActionResult<string> GetCurrentWaitlist(string userId)
        {
            var waitlistCode = _waitlistManager.GetWaitlistByUserId(userId);
            return Ok(waitlistCode);
        }
    }
}