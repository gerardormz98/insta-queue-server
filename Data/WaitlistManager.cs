using LiveWaitlist.Model;

namespace LiveWaitlist.Data
{
    public class WaitlistManager : IWaitlistManager
    {
        LinkedList<User> userList = new LinkedList<User>();
        Dictionary<Guid, LinkedListNode<User>> userDictionary = new Dictionary<Guid, LinkedListNode<User>>();

        public int GetCurrentSize() 
        {
            return userList.Count;
        }

        public User? DequeueUser(Guid userId)
        {
            if (userDictionary.TryGetValue(userId, out var node))
            {
                var nextNode = node.Next;

                // Remove from memory
                userList.Remove(node);
                userDictionary.Remove(userId);

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
            if (!userDictionary.ContainsKey(user.Id))
            {
                user.CheckInTime = DateTime.UtcNow;
                user.CurrentPosition = userList.Count + 1;
                var node = userList.AddLast(user);
                userDictionary.Add(user.Id, node);
            }

            return user.CurrentPosition;
        }
    }
}