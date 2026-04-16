namespace OnlineVoting_API.DTOs
{
    public class PollsDto
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public bool IsPublic { get; set; }

        public int? MultiChoiceLimit { get; set; }
        public bool IsAnonymous { get; set; }

        public List<string> Options { get; set; }
    }
}
