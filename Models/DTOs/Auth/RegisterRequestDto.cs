using System.ComponentModel.DataAnnotations;

namespace Walks.API.Models.DTOs
{
    public class RegisterRequestDto
    {
        [Required]
        [EmailAddress]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        public string[]? Roles { get; set; }
    }
}
