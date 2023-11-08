namespace LiveWaitlist.Model
{
    public class User
    {
        public User(string name, int partyNumber, string connectionId)
        {
            Name = name;
            PartyNumber = partyNumber;
            ConnectionId = connectionId;
        }

        public string Name { get; set; }
        public int PartyNumber { get; set; }
        public int CurrentPosition { get; set; }
        public DateTime CheckInTime { get; set; }
        public string ConnectionId { get; set; }
    }
}