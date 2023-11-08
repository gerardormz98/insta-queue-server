using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiveWaitlist.Model;

namespace LiveWaitlist.Data
{
    public interface IWaitlistManager
    {
        /// <summary>
        /// Get the current length of the queue.
        /// </summary>
        /// <returns>Current queue size.</returns>
        int GetCurrentSize();

        /// <summary>
        /// Adds user to the waitlist. 
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Current position in the queue.</returns>
        int EnqueueUser(User user);

        /// <summary>
        /// Removes user from the waitlist.
        /// </summary>
        /// <param name="userId"></param>
        User? DequeueUser(string connectionId);
    }
}