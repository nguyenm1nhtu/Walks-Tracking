using System.ComponentModel.DataAnnotations;

namespace Walks.API.Models.DTOs
{
    public class UpdateRegionRequestDto
    {
        [Required]
        [MaxLength(100, ErrorMessage = "Name has to be a maximum of 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MinLength(2, ErrorMessage = "Code has to be a minimum of 2 characters")]
        [MaxLength(5, ErrorMessage = "Code has to be a maximum of 5 characters")]
        public string Code { get; set; } = string.Empty;
        
        public string? RegionImageUrl { get; set; }
    }
}
