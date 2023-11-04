using LiveWaitlist.Data;
using Microsoft.AspNetCore.Mvc;

namespace LiveWaitlist.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WaitlistController : ControllerBase
    {
        private readonly IWaitlistManager _waitlistManager;

        public WaitlistController(IWaitlistManager waitlistManager)
        {
            _waitlistManager = waitlistManager;
        }

        [HttpGet]
        [Route("size")]
        public int GetCurrentSize()
        {
            return _waitlistManager.GetCurrentSize();
        }
    }
}