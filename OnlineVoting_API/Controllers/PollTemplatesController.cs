using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineVoting_API.Data;

namespace OnlineVoting_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PollTemplatesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PollTemplatesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetTemplates()
        {
            var templates = await _context.PollTemplates.ToListAsync();
            return Ok(templates);
        }
    }

}