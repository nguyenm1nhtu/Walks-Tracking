using System.ComponentModel.DataAnnotations;

namespace Walks.API.Models.DTOs
{
    public class ImageUploadRequestDto
    {
        [Required]
        public IFormFile File { get; set; } = null!;

        [Required]
        public string FileName { get; set; } = string.Empty;

        public string? FileDescription { get; set; }
    }
}
