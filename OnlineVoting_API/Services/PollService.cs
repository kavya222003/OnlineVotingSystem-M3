using Microsoft.EntityFrameworkCore;
using OnlineVoting_API.Data;
using OnlineVoting_API.DTOs;
using OnlineVoting_API.Models;

public class PollService
{
    private readonly AppDbContext _context;

    public PollService(AppDbContext context)
    {
        _context = context;
    }

  
    public async Task<int> CreatePoll(PollsDto dto, int userId)
    {
        var poll = new Poll
        {
            Title = dto.Title,
            Description = dto.Description,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            IsPublic = dto.IsPublic,
            MultiChoiceLimit = dto.MultiChoiceLimit,
            CreatedById = userId
        };

        _context.Polls.Add(poll);
        await _context.SaveChangesAsync();

        var options = dto.Options.Select((opt, index) => new PollOption
        {
            PollId = poll.Id,
            OptionText = opt,
            DisplayOrder = index
        }).ToList();

        _context.PollOptions.AddRange(options);
        await _context.SaveChangesAsync();

        return poll.Id;
    }

   
    public async Task<ResponseDto?> GetPoll(int pollId)
    {
        var poll = await _context.Polls
            .Include(p => p.Options)
            .FirstOrDefaultAsync(p => p.Id == pollId);

        if (poll == null) return null;

        return new ResponseDto
        {
            Id = poll.Id,
            Title = poll.Title,
            Description = poll.Description,
            IsPublic = poll.IsPublic,
            StartDate = poll.StartDate,
            EndDate = poll.EndDate,

           
            IsActive = DateTime.UtcNow >= poll.StartDate &&
                       DateTime.UtcNow <= poll.EndDate,

            Options = poll.Options
                .OrderBy(o => o.DisplayOrder)
                .Select(o => o.OptionText)
                .ToList()
        };
    }
}