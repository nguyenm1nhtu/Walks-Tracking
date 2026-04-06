using System.ComponentModel.DataAnnotations;

namespace Walks.API.Models.DTOs
{
    public class UpdateRegionRequestDto
    {
        [Required]
        [MinLength(1, ErrorMessage = "Code has to be a minimum of 1 character")]
        [MaxLength(10, ErrorMessage = "Code has to be a maximum of 10 characters")]
        public string Code { get; set; } = string.Empty;

        [Required]
        [MaxLength(100, ErrorMessage = "Name has to be a maximum of 100 characters")]
        public string Name { get; set; } = string.Empty;

        public string? RegionImageUrl { get; set; }
    }
}
