using System.ComponentModel.DataAnnotations;

namespace OnlineVoting_API.Models
{
    public class Vote
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        public int PollId { get; set; }
        public int OptionId { get; set; }

        public DateTime VotedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = null!;
        public Poll Poll { get; set; } = null!;
        public PollOption Option { get; set; } = null!;
    }
}