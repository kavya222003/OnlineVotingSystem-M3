using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineVoting_API.DTOs;
using OnlineVoting_API.Services;
using System.Security.Claims;

namespace OnlineVoting_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VotesController : ControllerBase
    {
        private readonly VoteService _voteService;

        public VotesController(VoteService voteService)
        {
            _voteService = voteService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Vote(VotesDto dto)
        {
            try
            {
                
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                await _voteService.Vote(dto.PollId, userId, dto.OptionIds);

                return Ok(new { message = "Vote submitted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}