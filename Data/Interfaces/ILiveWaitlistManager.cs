using LiveWaitlistServer.Model;

namespace LiveWaitlistServer.Data
{
    public interface ILiveWaitlistManager
    {
        /// <summary>
        /// Get the current length of the queue.
        /// </summary>
        /// <returns>Current queue size.</returns>
        int GetCurrentSize(string waitlistCode);

        /// <summary>
        /// Initializes a new waitlist so that users can enqueue.
        /// </summary>
        /// <param name="waitlistHost"></param>
        void CreateWaitlist(string waitlistCode);

        /// <summary>
        /// Adds user to the waitlist. 
        /// </summary>
        /// <param name="waitlistCode"></param>
        /// <param name="user"></param>
        /// <returns>If operation was successful or not</returns>
        bool EnqueueUser(string waitlistCode, User user);

        /// <summary>
        /// Removes user from the waitlist.
        /// </summary>
        /// <param name="userId"></param>
        User? DequeueUser(string userId);

        /// <summary>
        /// Gets the waitlist code in which the user is enqueued.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>Waitlist code</returns>
        string? GetWaitlistByUserId(string userId);

        /// <summary>
        /// Gets the current waitlist user data.
        /// </summary>
        /// <param name="waitlistCode"></param>
        /// <returns>List of users enqueued</returns>
        List<User>? GetCurrentList(string waitlistCode);

        /// <summary>
        /// Updates user ID in the user-waitlist mapping
        /// </summary>
        /// <param name="oldUserId">Old user ID</param>
        /// <param name="newUserId">New user ID</param>
        void UpdateUserIdIfExists(string oldUserId, string newUserId);

        /// <summary>
        /// Increases the notify count of a specific user
        /// </summary>
        /// <param name="userId"></param>
        void NotifyUser(string userId);

        /// <summary>
        /// Deletes waitlist and all related users
        /// </summary>
        /// <param name="waitlistCode"></param>
        void DeleteWaitlist(string waitlistCode);
    }
}