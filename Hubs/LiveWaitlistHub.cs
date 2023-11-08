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

        public async Task AddToWaitlist(string name, int partyNumber) 
        {
            string connectionId = Context.ConnectionId;
            var user = new User(name, partyNumber, connectionId);
            _waitlistManager.EnqueueUser(user);

            await Clients.Others.SendAsync(HubActions.PARTY_ADDED, user);
            await Clients.Caller.SendAsync(HubActions.USER_ADDED, user);
        }

        public async Task RemoveFromWaitlist(string connectionId)
        {
            var userRemoved = _waitlistManager.DequeueUser(connectionId);

            if (userRemoved != null) 
            {
                await Clients.Others.SendAsync(HubActions.PARTY_REMOVED, userRemoved);
                await Clients.Caller.SendAsync(HubActions.USER_REMOVED);
            }
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            return RemoveFromWaitlist(Context.ConnectionId);
        }
    }
}