using System.Text.RegularExpressions;
using LiveWaitlistServer.Configuration;
using LiveWaitlistServer.Data;
using LiveWaitlistServer.Data.Interfaces;
using LiveWaitlistServer.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace LiveWaitlistServer.Hubs
{
    public class LiveWaitlistHub : Hub<ILiveWaitlistClient>
    {
        private readonly ILiveWaitlistManager _waitlistManager;
        private readonly IUserRepository _userRepository;
        private readonly WaitlistConfigOptions _waitlistConfigOptions;

        public LiveWaitlistHub(ILiveWaitlistManager waitlistManager, IUserRepository userRepository, IOptions<WaitlistConfigOptions> waitlistConfigOptions)
        {
            _waitlistManager = waitlistManager;
            _userRepository = userRepository;
            _waitlistConfigOptions = waitlistConfigOptions.Value;
        }

        private async Task<string> UpdateAndGetConnectionId(string userId)
        {
            var connectionId = _userRepository.GetConnectionIdByUserId(userId);

            if (string.IsNullOrWhiteSpace(connectionId) || connectionId != Context.ConnectionId)
            {
                await AddOrUpdateUserMapping(userId);
                connectionId = _userRepository.GetConnectionIdByUserId(userId);
            }

            return connectionId;
        }

        public async Task AddOrUpdateUserMapping(string userId)
        {
            var mappedConnectionId = _userRepository.GetConnectionIdByUserId(userId);
            var mappedUserId = _userRepository.GetUserIdByConnectionId(Context.ConnectionId);

            // Update the SignalR groups if the user ID or connection ID changes

            if (!string.IsNullOrWhiteSpace(mappedConnectionId) && mappedConnectionId != Context.ConnectionId) // Connection ID changed
            {
                var waitlistCode = _waitlistManager.GetWaitlistByUserId(userId);

                if (waitlistCode != null)
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, waitlistCode);
                    await Groups.AddToGroupAsync(Context.ConnectionId, waitlistCode);
                }
            }
            else if(!string.IsNullOrWhiteSpace(mappedUserId) && mappedUserId != userId) // User ID changed
            {
                var waitlistCode = _waitlistManager.GetWaitlistByUserId(mappedUserId);

                if (waitlistCode != null)
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, waitlistCode);
                    await Groups.AddToGroupAsync(Context.ConnectionId, waitlistCode);
                    _waitlistManager.UpdateUserIdIfExists(mappedUserId, userId);
                }
            }

            _userRepository.MapUserToConnection(userId, Context.ConnectionId);
        }

        public async Task AddToGroup(string userId, string waitlistCode)
        {
            var connectionId = await UpdateAndGetConnectionId(userId);
            await Groups.AddToGroupAsync(connectionId, waitlistCode);
        }

        public async Task RemoveFromGroup(string userId, string waitlistCode)
        {
            var connectionId = await UpdateAndGetConnectionId(userId);
            await Groups.RemoveFromGroupAsync(connectionId, waitlistCode);
        }
        
        public async Task AddToWaitlistQueue(string waitlistCode, string userId, string name, int partyNumber) 
        {  
            if (_waitlistManager.GetCurrentSize(waitlistCode) >= _waitlistConfigOptions.MaxUsersEnqueued)
            {
                await Clients.Caller.OnWaitlistFullError();
                return;    
            }

            if (!string.IsNullOrWhiteSpace(userId))
            {
                var connectionId = await UpdateAndGetConnectionId(userId);
                var user = new User(userId, name, partyNumber);

                if (_waitlistManager.EnqueueUser(waitlistCode, user))
                {
                    await Groups.AddToGroupAsync(connectionId, waitlistCode);
                    await Clients.OthersInGroup(waitlistCode).OnPartyAdded(user);
                }
            }
        }

        public async Task RemoveFromWaitlistQueue(string userId)
        {                
            var connectionId = await UpdateAndGetConnectionId(userId);
            var waitlistCode = _waitlistManager.GetWaitlistByUserId(userId);
            var userRemoved = _waitlistManager.DequeueUser(userId);

            if (userRemoved != null && waitlistCode != null) 
            {
                await Clients.OthersInGroup(waitlistCode).OnPartyRemoved(userRemoved);
                await Groups.RemoveFromGroupAsync(connectionId, waitlistCode);
            }
        }

        #region Admin Methods

        [Authorize (Roles = "Admin")]
        public async Task Admin_NotifyUser(string userId)
        {
            var connectionId = _userRepository.GetConnectionIdByUserId(userId);
            
            if (connectionId != null) 
            {
                _waitlistManager.NotifyUser(userId);
                await Clients.Client(connectionId).OnNotified();
            }
        }

        [Authorize (Roles = "Admin")]
        public async Task Admin_CompleteUser(string userId)
        {
            var connectionId = _userRepository.GetConnectionIdByUserId(userId);
            var waitlistCode = _waitlistManager.GetWaitlistByUserId(userId);
            var userRemoved = _waitlistManager.DequeueUser(userId);

            if (userRemoved != null && waitlistCode != null) 
            {
                await Clients.Client(connectionId).OnCompleted();
                await Clients.GroupExcept(waitlistCode, connectionId).OnPartyRemoved(userRemoved);
                await Groups.RemoveFromGroupAsync(connectionId, waitlistCode);
            }
        }

        [Authorize (Roles = "Admin")]
        public async Task Admin_RemoveFromWaitlistQueue(string userId)
        {                
            var connectionId = _userRepository.GetConnectionIdByUserId(userId);
            var waitlistCode = _waitlistManager.GetWaitlistByUserId(userId);
            var userRemoved = _waitlistManager.DequeueUser(userId);

            if (userRemoved != null && waitlistCode != null) 
            {                
                await Clients.Client(connectionId).OnRemoved();
                await Clients.GroupExcept(waitlistCode, connectionId).OnPartyRemoved(userRemoved);
                await Groups.RemoveFromGroupAsync(connectionId, waitlistCode);
            }
        }

        #endregion

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
    }
}