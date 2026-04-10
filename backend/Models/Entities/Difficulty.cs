using Walks.API.Models.Enums;

namespace Walks.API.Models.Entities
{
    public class Difficulty
    {
        public Guid Id { get; set; }
        public DifficultyLevel Name { get; set; } = DifficultyLevel.Beginner;
    }
}
