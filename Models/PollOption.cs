using System.ComponentModel.DataAnnotations;

namespace OnlineVoting_API.Models
{
    public class PollOption
    {
        [Key]
        public int Id { get; set; }

        public int PollId { get; set; }

        [Required]
        public string OptionText { get; set; } = string.Empty;

        public int DisplayOrder { get; set; }

        public Poll Poll { get; set; } = null!;

        public ICollection<Vote> Votes { get; set; } = new List<Vote>();
    }
}