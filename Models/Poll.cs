using System.ComponentModel.DataAnnotations;

namespace OnlineVoting_API.Models
{
    public class Poll
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int CreatedByUserId { get; set; }

        public bool IsPublic { get; set; } = true;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public User CreatedBy { get; set; } = null!;

        public ICollection<PollOption> Options { get; set; } = new List<PollOption>();
        public ICollection<Vote> Votes { get; set; } = new List<Vote>();
    }
}