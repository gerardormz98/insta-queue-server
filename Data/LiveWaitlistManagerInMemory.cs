using LiveWaitlistServer.Model;

namespace LiveWaitlistServer.Data
{
    public class LiveWaitlistManagerInMemory : ILiveWaitlistManager
    {
        Dictionary<string, List<User>> waitlistData = new Dictionary<string, List<User>>();
        Dictionary<string, string> userWaitlist = new Dictionary<string, string>();

        public LiveWaitlistManagerInMemory()
        {
            // TODO: Get this from DB
            waitlistData.Add("AAAAAA", new List<User>());
            waitlistData.Add("BBBBBB", new List<User>());
        }

        public void CreateWaitlist(string waitlistCode)
        {
            waitlistData.Add(waitlistCode, new List<User>());
        }

        public int GetCurrentSize(string waitlistCode) 
        {
            if (!waitlistData.ContainsKey(waitlistCode))
                throw new ApplicationException("The waitlist code doesn't exist.");

            return waitlistData[waitlistCode].Count;
        }

        public User? DequeueUser(string userId)
        {
            var waitlistCode = GetWaitlistByUserId(userId);

            if (string.IsNullOrWhiteSpace(waitlistCode))
                return null;

            if (waitlistData.TryGetValue(waitlistCode, out var userList))
            {
                var user = userList.FirstOrDefault(u => u.UserId == userId);

                if (user != null)
                    userList.Remove(user);

                userWaitlist.Remove(userId);

                // Update current position for the rest of the list
                for (int i = 0; i < userList.Count; i++)
                {
                    userList[i].CurrentPosition = i + 1;
                }

                return user;
            }

            return null;
        }

        public bool EnqueueUser(string waitlistCode, User user)
        {
            if (GetWaitlistByUserId(user.UserId) != null)
                return false;

            if (!waitlistData.ContainsKey(waitlistCode))
                return false;

            userWaitlist.Add(user.UserId, waitlistCode);

            var userList = waitlistData[waitlistCode];
            user.CheckInTime = DateTime.UtcNow;
            user.CurrentPosition = userList.Count + 1;
            userList.Add(user);

            return true;
        }

        public string? GetWaitlistByUserId(string userId)
        {
            if (userWaitlist.ContainsKey(userId))
                return userWaitlist[userId];

            return null;
        }

        public List<User>? GetCurrentList(string waitlistCode)
        {
            if (waitlistData.TryGetValue(waitlistCode, out var userList))
                return userList;

            return null;
        }

        public void UpdateUserIdIfExists(string oldUserId, string newUserId)
        {
            if (userWaitlist.ContainsKey(oldUserId))
            {
                var waitlistCode = userWaitlist[oldUserId];
                userWaitlist.Remove(oldUserId);
                userWaitlist.Add(newUserId, waitlistCode);

                if (waitlistData.ContainsKey(waitlistCode))
                {
                    var user = waitlistData[waitlistCode].FirstOrDefault(u => u.UserId == oldUserId);

                    if (user != null)
                        user.UserId = newUserId;
                }
            }
        }

        public void NotifyUser(string userId)
        {
            var waitlistCode = GetWaitlistByUserId(userId);

            if (string.IsNullOrWhiteSpace(waitlistCode))
                return;

            if (waitlistData.TryGetValue(waitlistCode, out var userList))
            {
                var user = userList.FirstOrDefault(u => u.UserId == userId);

                if (user != null)
                    user.NotifyCount++;
            }
        }

        public void DeleteWaitlist(string waitlistCode)
        {
            if (waitlistData.ContainsKey(waitlistCode))
            {
                var usersInWaitlist = GetCurrentList(waitlistCode);
                waitlistData.Remove(waitlistCode);

                if (usersInWaitlist != null) 
                {
                    foreach(var user in usersInWaitlist)
                        userWaitlist.Remove(user.UserId);
                }
            }
        }
    }
}