using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineVoting_API.Services;
using System.Security.Claims;

namespace OnlineVoting_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VotesController : ControllerBase
    {
        private readonly VoteService _voteService;

        public VotesController(VoteService voteService)
        {
            _voteService = voteService;
        }

        // ✅ AUTH USER VOTE
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Vote(int pollId, List<int> optionIds)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var confirmationId = await _voteService.Vote(pollId, userId, optionIds);

            return Ok(new
            {
                message = "Vote submitted",
                confirmationId
            });
        }

        // ✅ GUEST VOTE
        [HttpPost("guest")]
        public async Task<IActionResult> GuestVote(int pollId, List<int> optionIds)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var fingerprint = Request.Headers["X-Fingerprint"].ToString();

            if (string.IsNullOrEmpty(fingerprint))
                return BadRequest("Fingerprint missing");

            var confirmationId = await _voteService.GuestVote(pollId, optionIds, ip, fingerprint);

            return Ok(new
            {
                message = "Vote submitted",
                confirmationId
            });
        }

        // ✅ HAS VOTED
        [Authorize]
        [HttpGet("has-voted/{pollId}")]
        public async Task<IActionResult> HasVoted(int pollId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var result = await _voteService.HasVoted(pollId, userId);

            return Ok(result);
        }

        // ✅ CONFIRMATION RECEIPT
        [HttpGet("confirm/{confirmationId}")]
        public async Task<IActionResult> GetConfirmation(Guid confirmationId)
        {
            var result = await _voteService.GetConfirmation(confirmationId);

            return Ok(result);
        }
    }
}