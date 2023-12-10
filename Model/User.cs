namespace LiveWaitlistServer.Model
{
    public class User
    {
        public User(string userId, string name, int partyNumber)
        {
            UserId = userId;
            Name = name;
            PartySize = partyNumber;
        }

        public string UserId { get; set; }
        public string Name { get; set; }
        public int PartySize { get; set; }
        public int CurrentPosition { get; set; }
        public DateTime CheckInTime { get; set; }
        public int NotifyCount { get; set; }
    }
}