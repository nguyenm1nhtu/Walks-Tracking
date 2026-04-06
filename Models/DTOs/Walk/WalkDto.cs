using Walks.API.Models.Entities;

namespace Walks.API.Models.DTOs
{
    public class WalkDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double LengthInKm { get; set; } = 0;
        public string? WalkImageUrl { get; set; } 

        public RegionDto Region { get; set; } = null!;
        public DifficultyDto Difficulty { get; set; } = null!;
    }
}
