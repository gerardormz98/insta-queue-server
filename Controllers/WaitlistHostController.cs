using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LiveWaitlistServer.Configuration;
using LiveWaitlistServer.Data;
using LiveWaitlistServer.Hubs;
using LiveWaitlistServer.Model;
using LiveWaitlistServer.Model.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace LiveWaitlistServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WaitlistHostController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IWaitlistHostRepository _waitlistHostRepository;
        private readonly ILiveWaitlistManager _waitlistManager;
        private readonly IHubContext<LiveWaitlistHub> _liveWaitlistHub;
        private readonly WaitlistConfigOptions _waitlistConfigOptions;

        public WaitlistHostController(IMapper mapper, IWaitlistHostRepository waitlistHostRepository, ILiveWaitlistManager waitlistManager, IHubContext<LiveWaitlistHub> liveWaitlistHub, IOptions<WaitlistConfigOptions> waitlistConfigOptions)
        {
            _mapper = mapper;
            _waitlistHostRepository = waitlistHostRepository;
            _waitlistManager = waitlistManager;
            _liveWaitlistHub = liveWaitlistHub;
            _waitlistConfigOptions = waitlistConfigOptions.Value;
        }

        [HttpGet]
        [Route("{waitlistCode}")]
        public ActionResult<WaitlistHostResult> Get(string waitlistCode)
        {
            var waitlistHost = _waitlistHostRepository.Get(waitlistCode);

            if (waitlistHost == null)
                return NotFound();

            return Ok(_mapper.Map<WaitlistHostResult>(waitlistHost));
        }

        [HttpGet]
        [Route("{waitlistCode}/queue")]
        public ActionResult<List<User>> GetQueue(string waitlistCode)
        {
            var waitlistQueue = _waitlistManager.GetCurrentList(waitlistCode);

            if (waitlistQueue == null)
                return NotFound();

            return Ok(waitlistQueue);
        }

        [HttpPost]
        public ActionResult<WaitlistHostResult> CreateHost(WaitlistHostCreateRequest hostRequest)
        {
            int hostsCount = _waitlistHostRepository.GetCount();

            if (hostsCount >= _waitlistConfigOptions.MaxHosts)
            {
                return Forbid();
            }

            var newHost = _waitlistHostRepository.Create(hostRequest);
            _waitlistManager.CreateWaitlist(newHost.WaitlistCode);

            return Ok(_mapper.Map<WaitlistHostResult>(newHost));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        [Route("{waitlistCode}")]
        public ActionResult<WaitlistHostResult> UpdateHost(string waitlistCode, WaitlistHostUpdateRequest hostRequest)
        {
            var updatedHost = _waitlistHostRepository.Update(waitlistCode, hostRequest);

            if (updatedHost == null)
                return BadRequest();

            return Ok(_mapper.Map<WaitlistHostResult>(updatedHost));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        [Route("{waitlistCode}")]
        public async Task<ActionResult<WaitlistHostResult>> DeleteHost(string waitlistCode)
        {
            var deletedHost = _waitlistHostRepository.Delete(waitlistCode);

            if (deletedHost == null)
                return BadRequest();

            _waitlistManager.DeleteWaitlist(waitlistCode);
            await _liveWaitlistHub.Clients.Group(waitlistCode).SendAsync("OnWaitlistClosed");

            return Ok(_mapper.Map<WaitlistHostResult>(deletedHost));
        }
    }
}