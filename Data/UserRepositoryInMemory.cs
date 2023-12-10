using LiveWaitlistServer.Data.Interfaces;

namespace LiveWaitlistServer.Data
{
    public class UserRepositoryInMemory : IUserRepository
    {
        // TODO: Change this to DB
        private Dictionary<string, string> usersToConnectionId = new Dictionary<string, string>();
        private Dictionary<string, string> connectionIdToUser = new Dictionary<string, string>();

        public string GetConnectionIdByUserId(string userId)
        {
            return usersToConnectionId.GetValueOrDefault(userId, "");
        }

        public string GetUserIdByConnectionId(string userId)
        {
            return connectionIdToUser.GetValueOrDefault(userId, "");
        }

        public void MapUserToConnection(string userId, string connectionId)
        {
            if (usersToConnectionId.ContainsKey(userId)) // Existing user
            {
                var oldConnectionId = usersToConnectionId[userId];
                usersToConnectionId[userId] = connectionId;

                connectionIdToUser.Remove(oldConnectionId);
                connectionIdToUser[connectionId] = userId;
            }
            else if (connectionIdToUser.ContainsKey(connectionId)) // Existing connection, user was changed
            {
                var oldUserId = connectionIdToUser[connectionId];
                connectionIdToUser[connectionId] = userId;

                usersToConnectionId.Remove(oldUserId);
                usersToConnectionId[userId] = connectionId;
            }
            else // New connection and user
            {
                usersToConnectionId.Add(userId, connectionId);
                connectionIdToUser.Add(connectionId, userId);
            }
        }
    }
}