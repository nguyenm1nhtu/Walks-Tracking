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
    [Authorize]
    public class RegionController : ControllerBase
    {
        private readonly IRegionRepository _regionRepository;
        private readonly IMapper _mapper;

        public RegionController(IRegionRepository regionRepository, IMapper mapper)
        {
            _regionRepository = regionRepository;
            _mapper = mapper;
        }

        // GET: api/Region
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RegionDto>>> GetAll(
            [FromQuery] string? filterOn, 
            [FromQuery] string? filterQuery, 
            [FromQuery] string? sortBy, 
            [FromQuery] bool? isAscending, 
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 10)
        {
            var regions = await _regionRepository.GetAllAsync(
                filterOn, 
                filterQuery, 
                sortBy, 
                isAscending, 
                pageNumber, 
                pageSize);

            return Ok(_mapper.Map<List<RegionDto>>(regions));
        }

        // GET: api/Region/5
        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<RegionDto>> GetById([FromRoute]Guid id)
        {
            var region = await _regionRepository.GetByIdAsync(id);

            if (region == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<RegionDto>(region));
        }

        // POST: api/Region
        [HttpPost]
        public async Task<ActionResult<RegionDto>> Create([FromBody] AddRegionDto newRegion)
        {
            var region = _mapper.Map<Region>(newRegion);

            region = await _regionRepository.CreateAsync(region);

            var regionDto = _mapper.Map<RegionDto>(region);

            return CreatedAtAction(nameof(GetById), new { id = regionDto.Id }, regionDto);
        }

        // PUT: api/Region/5
        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> Update([FromRoute]Guid id, [FromBody] UpdateRegionRequestDto updateRegion)
        {
            var region = _mapper.Map<Region>(updateRegion);

            region = await _regionRepository.UpdateAsync(id, region);

            if (region == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<RegionDto>(region));
        }

        // DELETE: api/Region/5
        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute]Guid id)
        {
            var region = await _regionRepository.DeleteAsync(id);
            if (region == null)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
