using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using OnlineVoting_API.DTOs;

namespace OnlineVoting_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PollsController : ControllerBase
    {
        private readonly PollService _service;

        public PollsController(PollService service)
        {
            _service = service;
        }

        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(PollsDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var pollId = await _service.CreatePoll(dto, userId);

            return Ok(new { pollId });
        }

        
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var poll = await _service.GetPoll(id);

            if (poll == null) return NotFound();

            return Ok(poll);
        }
    }
}
