using Microsoft.EntityFrameworkCore;
using OnlineVoting_API.Data;
using OnlineVoting_API.Models;

namespace OnlineVoting_API.Services
{
    public class VoteService
    {
        private readonly AppDbContext _context;

        public VoteService(AppDbContext context)
        {
            _context = context;
        }

        public async Task Vote(int pollId, int userId, List<int> optionIds)
        {
            
            var poll = await _context.Polls
                .Include(p => p.Options)
                .FirstOrDefaultAsync(p => p.Id == pollId);

            if (poll == null)
                throw new Exception("Poll not found");

            var now = DateTime.UtcNow;
            if (now < poll.StartDate || now > poll.EndDate)
                throw new Exception("Poll is not active");

          
            var validOptionIds = poll.Options.Select(o => o.Id).ToList();

            if (optionIds.Any(id => !validOptionIds.Contains(id)))
                throw new Exception("Invalid option selected");

            if (poll.MultiChoiceLimit.HasValue)
            {
                if (optionIds.Count == 0)
                    throw new Exception("Select at least one option");

                if (optionIds.Count > poll.MultiChoiceLimit.Value)
                    throw new Exception($"You can select max {poll.MultiChoiceLimit} options");
            }
            else
            {
            
                if (optionIds.Count != 1)
                    throw new Exception("Only one option allowed");
            }

            var alreadyVoted = await _context.Votes
                .AnyAsync(v => v.UserId == userId && v.PollId == pollId);

            if (alreadyVoted)
                throw new Exception("User already voted");

         
            var votes = optionIds.Select(optionId => new Vote
            {
                PollId = pollId,
                UserId = userId,
                OptionId = optionId,
                VotedAt = DateTime.UtcNow
            }).ToList();

            _context.Votes.AddRange(votes);

            await _context.SaveChangesAsync();
        }
    }
}