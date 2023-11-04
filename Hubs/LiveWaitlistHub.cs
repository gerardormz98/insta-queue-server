using LiveWaitlist.Data;
using LiveWaitlist.Model;
using Microsoft.AspNetCore.SignalR;

namespace LiveWaitlist.Hubs
{
    public class LiveWaitlistHub : Hub
    {
        private readonly IWaitlistManager _waitlistManager;

        public LiveWaitlistHub(IWaitlistManager waitlistManager)
        {
            _waitlistManager = waitlistManager;
        }

        public async void AddToWaitlist(string name, int partyNumber) 
        {
            string connectionId = Context.ConnectionId;
            var user = new User(name, partyNumber, connectionId);
            _waitlistManager.EnqueueUser(user);

            await Clients.Others.SendAsync(HubActions.PARTY_ADDED, user);
            await Clients.Caller.SendAsync(HubActions.USER_ADDED, user);
        }

        public async void RemoveFromWaitlist(string userId)
        {
            Guid userIdGuid = Guid.Parse(userId);
            var userRemoved = _waitlistManager.DequeueUser(userIdGuid);

            await Clients.Others.SendAsync(HubActions.PARTY_REMOVED, userRemoved);
            await Clients.Caller.SendAsync(HubActions.USER_REMOVED);
        }
    }
}