namespace OnlineVoting_API.DTOs
{
    public class ResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public bool IsActive { get; set; }
        public bool IsPublic { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public List<string> Options { get; set; }
    }
}
