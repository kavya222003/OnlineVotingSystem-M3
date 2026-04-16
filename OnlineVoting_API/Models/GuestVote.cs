namespace OnlineVoting_API.Models
{
    public class GuestVote
    {
        public int Id { get; set; }
        public int PollId { get; set; }
        public string IpAddress { get; set; }
        public string Fingerprint { get; set; }
        public DateTime VotedAt { get; set; }
    }
}
