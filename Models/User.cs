using System.ComponentModel.DataAnnotations;

namespace OnlineVoting_API.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Poll> CreatedPolls { get; set; } = new List<Poll>();
        public ICollection<Vote> Votes { get; set; } = new List<Vote>();
    }
}