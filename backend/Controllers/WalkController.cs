using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Walks.API.Models.DTOs;
using Walks.API.Models.Entities;
using Walks.API.Repositories;
using AutoMapper;

namespace Walks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalkController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IWalkRepository _walkRepository;
        public WalkController(IMapper mapper, IWalkRepository walkRepository)
        {
            _walkRepository = walkRepository;
            _mapper = mapper;
        }

    // GET: api/Walk?filterOn=Name&filterQuery=Track&sortBy=LengthInKm&isAscending=false&pageNumber=1&pageSize=10
    [HttpGet]
    [Authorize(Roles="Reader, Writer")]
    public async Task<ActionResult<List<WalkDto>>> GetAll(
        [FromQuery] string? filterOn, 
        [FromQuery] string? filterQuery, 
        [FromQuery] string? sortBy, 
        [FromQuery] bool isAscending, 
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10)
        {
            var walks = await _walkRepository.GetAllAsync(
                filterOn, 
                filterQuery, 
                sortBy, 
                isAscending, 
                pageNumber, 
                pageSize);

            return Ok(_mapper.Map<List<WalkDto>>(walks));
        } 

    // GET: api/Walk/5
    [HttpGet("{id:Guid}")]
    [Authorize(Roles="Reader, Writer")]
    public async Task<ActionResult<WalkDto>> GetById([FromRoute]Guid id)
        {
            var walk = await _walkRepository.GetByIdAsync(id);

            if (walk == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<WalkDto>(walk));
        }   
    
    // POST: api/Walk
    [HttpPost]
    [Authorize(Roles="Writer")]
    public async Task<ActionResult<WalkDto>> Create([FromBody] AddWalkDto newWalk)
        {
            var walk =_mapper.Map<Walk>(newWalk);

            walk = await _walkRepository.CreateAsync(walk);

            return Ok(_mapper.Map<WalkDto>(walk));
        } 

    // PUT: api/Walk/5
    [HttpPut("{id:Guid}")]
    [Authorize(Roles="Writer")]
    public async Task<ActionResult<WalkDto>> Update([FromRoute] Guid id, [FromBody] UpdateWalkRequestDto updatedWalk)
        {
            var walk = _mapper.Map<Walk>(updatedWalk);

            walk = await _walkRepository.UpdateAsync(id, walk);

            if (walk == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<WalkDto>(walk));
        }

    // DELETE: api/Walk/5
    [HttpDelete("{id:Guid}")]
    [Authorize(Roles="Writer")]
    public async Task<ActionResult<WalkDto>> Delete([FromRoute] Guid id)
        {
            var walk = await _walkRepository.DeleteAsync(id);

            if (walk == null)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
    
}
