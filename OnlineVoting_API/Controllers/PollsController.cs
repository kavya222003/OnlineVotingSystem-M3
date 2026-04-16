using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineVoting_API.Data;
using OnlineVoting_API.DTOs;
using OnlineVoting_API.Services;
using System.Security.Claims;

namespace OnlineVoting_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PollsController : ControllerBase
    {
        private readonly PollService _service;
        private readonly AppDbContext _context;

        public PollsController(PollService service, AppDbContext context)
        {
            _service = service;
            _context = context;
        }

        // ✅ CREATE POLL
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(PollsDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var pollId = await _service.CreatePoll(dto, userId);

            return Ok(new { pollId });
        }

        // ✅ GET POLL
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var poll = await _service.GetPoll(id);

            if (poll == null)
                return NotFound();

            return Ok(poll);
        }

        // ✅ GET USER VOTE (Module 3 - Use Case 1)
        [Authorize]
        [HttpGet("{id}/my-vote")]
        public async Task<IActionResult> GetMyVote(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var votes = await _context.Votes
                .Where(v => v.PollId == id && v.UserId == userId)
                .Select(v => v.OptionId)
                .ToListAsync();

            if (!votes.Any())
                return Ok(null);

            return Ok(votes);
        }

        // ✅ SHAREABLE LINK (Module 3 - Use Case 2)
        [HttpGet("share/{token}")]
        public async Task<IActionResult> GetByShareToken(Guid token)
        {
            var poll = await _context.Polls
                .Include(p => p.Options)
                .FirstOrDefaultAsync(p => p.ShareToken == token);

            if (poll == null)
                return NotFound();

            return Ok(new
            {
                poll.Id,
                poll.Title,
                poll.Description,
                poll.IsPublic,
                poll.StartDate,
                poll.EndDate,
                IsActive = DateTime.UtcNow >= poll.StartDate && DateTime.UtcNow <= poll.EndDate,
                Options = poll.Options.Select(o => new
                {
                    o.Id,
                    o.OptionText
                })
            });
        }
    }
}