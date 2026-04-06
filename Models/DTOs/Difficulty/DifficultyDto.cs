using Walks.API.Models.Enums;

namespace Walks.API.Models.DTOs
{
    public class DifficultyDto
    {
        public Guid Id { get; set; }
        public DifficultyLevel Name { get; set; } = DifficultyLevel.Easy;
    }
}
