using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Walks.API.Data;
using Walks.API.Models.DTOs;

namespace Walks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DifficultyController : ControllerBase
    {
        private readonly WalksDbContext _context;

        public DifficultyController(WalksDbContext context)
        {
            _context = context;
        }

        // GET: api/Difficulty
        [HttpGet]
        [Authorize(Roles = "Reader, Writer")]
        public async Task<ActionResult<IEnumerable<DifficultyDto>>> GetAll()
        {
            var difficulties = await _context.Difficulties
                .AsNoTracking()
                .OrderBy(difficulty => difficulty.Name)
                .Select(difficulty => new DifficultyDto
                {
                    Id = difficulty.Id,
                    Name = difficulty.Name
                })
                .ToListAsync();

            return Ok(difficulties);
        }
    }
}
