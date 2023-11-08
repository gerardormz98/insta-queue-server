using LiveWaitlist.Model;

namespace LiveWaitlist.Data
{
    public class WaitlistManager : IWaitlistManager
    {
        LinkedList<User> userList = new LinkedList<User>();
        Dictionary<string, LinkedListNode<User>> userDictionary = new Dictionary<string, LinkedListNode<User>>();

        public int GetCurrentSize() 
        {
            return userList.Count;
        }

        public User? GetUserByConnectionId(string connectionId)
        {
            if (userDictionary.TryGetValue(connectionId, out var node))
            {
                return node.Value;
            }

            return null;
        }

        public User? DequeueUser(string connectionId)
        {
            if (userDictionary.TryGetValue(connectionId, out var node))
            {
                var nextNode = node.Next;

                // Remove from memory
                userList.Remove(node);
                userDictionary.Remove(connectionId);

                // Update current position for the rest of the queue
                while (nextNode != null)
                {
                    nextNode.Value.CurrentPosition--;
                    nextNode = nextNode.Next;
                }

                return node.Value;
            }

            return null;
        }

        public int EnqueueUser(User user)
        {
            if (!userDictionary.ContainsKey(user.ConnectionId))
            {
                user.CheckInTime = DateTime.UtcNow;
                user.CurrentPosition = userList.Count + 1;
                var node = userList.AddLast(user);
                userDictionary.Add(user.ConnectionId, node);
            }

            return user.CurrentPosition;
        }
    }
}