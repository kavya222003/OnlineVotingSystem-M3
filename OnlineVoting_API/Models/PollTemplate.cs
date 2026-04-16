namespace OnlineVoting_API.Models
{
    public class PollTemplate
    {
        public int Id { get; set; }
        public string TemplateName { get; set; } = string.Empty;
        public string DefaultQuestion { get; set; } = string.Empty;

        // Store as JSON string
        public string DefaultOptions { get; set; } = string.Empty;
    }
}