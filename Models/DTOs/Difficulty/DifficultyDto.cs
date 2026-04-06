using System.ComponentModel.DataAnnotations;
using Walks.API.Models.Enums;

namespace Walks.API.Models.DTOs
{
    public class DifficultyDto
    {
        public Guid Id { get; set; }

        [Required]
        [EnumDataType(typeof(DifficultyLevel), ErrorMessage = "Difficulty must be one of: Beginner, Immidiate, Avanced.")]
        public DifficultyLevel Name { get; set; } = DifficultyLevel.Beginner;
    }
}
