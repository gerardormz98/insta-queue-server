using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiveWaitlistServer.Model;

namespace LiveWaitlistServer.Hubs
{
    public interface ILiveWaitlistClient
    {
        Task OnNotified();
        Task OnCompleted();
        Task OnRemoved();
        Task OnPartyAdded(User user);
        Task OnPartyRemoved(User user);
        Task OnWaitlistClosed();

        // Errors
        Task OnWaitlistFullError();
    }
}