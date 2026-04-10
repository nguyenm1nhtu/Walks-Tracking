using System.ComponentModel.DataAnnotations;
using Walks.API.Validation;

namespace Walks.API.Models.DTOs
{
    public class AddWalkDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0, 50)]
        public double LengthInKm { get; set; } = 0;
        public string? WalkImageUrl { get; set; } 

        [Required]
        [NotEmptyGuid]
        public Guid DifficultyId { get; set; }

        [Required]
        [NotEmptyGuid]
        public Guid RegionId { get; set; }
    }
}
