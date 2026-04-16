namespace OnlineVoting_API.DTOs
{
    public class VotesDto
    {
        public int PollId { get; set; }
        public List<int> OptionIds { get; set; } = new List<int>();
    }
}
