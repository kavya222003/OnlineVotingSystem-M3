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

        // ✅ AUTHENTICATED USER VOTE
        public async Task<Guid> Vote(int pollId, int userId, List<int> optionIds)
        {
            var poll = await _context.Polls
                .Include(p => p.Options)
                .FirstOrDefaultAsync(p => p.Id == pollId);

            if (poll == null)
                throw new Exception("Poll not found");

            var now = DateTime.UtcNow;
            if (now < poll.StartDate || now > poll.EndDate)
                throw new Exception("Poll is not active");

            // ✅ Validate options
            var validOptionIds = poll.Options.Select(o => o.Id).ToList();

            if (optionIds.Any(id => !validOptionIds.Contains(id)))
                throw new Exception("Invalid option selected");

            // ✅ MultiChoice / SingleChoice validation
            if (poll.MultiChoiceLimit.HasValue)
            {
                if (optionIds.Count == 0)
                    throw new Exception("Select at least one option");

                if (optionIds.Count > poll.MultiChoiceLimit.Value)
                    throw new Exception($"Max {poll.MultiChoiceLimit} options allowed");
            }
            else
            {
                if (optionIds.Count != 1)
                    throw new Exception("Only one option allowed");
            }

            // ✅ Prevent duplicate voting
            var alreadyVoted = await _context.Votes
                .AnyAsync(v => v.UserId == userId && v.PollId == pollId);

            if (alreadyVoted)
                throw new Exception("User already voted");

            var confirmationId = Guid.NewGuid();

            var votes = optionIds.Select(optionId => new Vote
            {
                PollId = pollId,
                UserId = poll.IsAnonymous ? null : userId,
                OptionId = optionId,
                VotedAt = DateTime.UtcNow,
                ConfirmationId = confirmationId
            }).ToList();

            _context.Votes.AddRange(votes);
            await _context.SaveChangesAsync();

            return confirmationId;
        }

        // ✅ GUEST VOTE (IP + Fingerprint)
        public async Task<Guid> GuestVote(int pollId, List<int> optionIds, string ip, string fingerprint)
        {
            var poll = await _context.Polls
                .Include(p => p.Options)
                .FirstOrDefaultAsync(p => p.Id == pollId);

            if (poll == null)
                throw new Exception("Poll not found");

            var now = DateTime.UtcNow;
            if (now < poll.StartDate || now > poll.EndDate)
                throw new Exception("Poll is not active");

            // ✅ Prevent duplicate guest voting
            var alreadyVoted = await _context.GuestVotes
                .AnyAsync(g => g.PollId == pollId &&
                               g.IpAddress == ip &&
                               g.Fingerprint == fingerprint);

            if (alreadyVoted)
                throw new Exception("You have already voted from this device");

            // ✅ Validate options
            var validOptionIds = poll.Options.Select(o => o.Id).ToList();

            if (optionIds.Any(id => !validOptionIds.Contains(id)))
                throw new Exception("Invalid option selected");

            // ✅ MultiChoice logic
            if (poll.MultiChoiceLimit.HasValue)
            {
                if (optionIds.Count == 0)
                    throw new Exception("Select at least one option");

                if (optionIds.Count > poll.MultiChoiceLimit.Value)
                    throw new Exception($"Max {poll.MultiChoiceLimit} options allowed");
            }
            else
            {
                if (optionIds.Count != 1)
                    throw new Exception("Only one option allowed");
            }

            var confirmationId = Guid.NewGuid();

            // ✅ Save guest tracking
            var guestVote = new GuestVote
            {
                PollId = pollId,
                IpAddress = ip,
                Fingerprint = fingerprint,
                VotedAt = DateTime.UtcNow
            };

            _context.GuestVotes.Add(guestVote);

            // ✅ Save votes
            var votes = optionIds.Select(optionId => new Vote
            {
                PollId = pollId,
                UserId = null,
                OptionId = optionId,
                VotedAt = DateTime.UtcNow,
                ConfirmationId = confirmationId
            }).ToList();

            _context.Votes.AddRange(votes);

            await _context.SaveChangesAsync();

            return confirmationId;
        }

        // ✅ CHECK USER VOTED
        public async Task<bool> HasVoted(int pollId, int userId)
        {
            return await _context.Votes
                .AnyAsync(v => v.PollId == pollId && v.UserId == userId);
        }

        // ✅ CONFIRMATION RECEIPT
        public async Task<object> GetConfirmation(Guid confirmationId)
        {
            var votes = await _context.Votes
                .Include(v => v.Poll)
                .Include(v => v.Option)
                .Where(v => v.ConfirmationId == confirmationId)
                .ToListAsync();

            if (!votes.Any())
                throw new Exception("Invalid confirmation ID");

            return new
            {
                ConfirmationId = confirmationId,
                PollTitle = votes.First().Poll.Title,
                Options = votes.Select(v => v.Option.OptionText).ToList(),
                VotedAt = votes.First().VotedAt
            };
        }
    }
}