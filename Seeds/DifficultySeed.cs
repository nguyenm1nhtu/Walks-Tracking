using Microsoft.EntityFrameworkCore;
using Walks.API.Data;
using Walks.API.Models.Entities;

namespace Walks.API.Seeds
{
    public static class DifficultySeed
    {
        private static readonly string[] DifficultyNames = ["Easy", "Medium", "Hard", "Expert"];

        public static void Seed(WalksDbContext context)
        {
            var existingNames = context.Difficulties
                .AsNoTracking()
                .Select(x => x.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var missingDifficulties = DifficultyNames
                .Where(name => !existingNames.Contains(name))
                .Select(name => new Difficulty
                {
                    Id = Guid.NewGuid(),
                    Name = name
                })
                .ToList();

            if (missingDifficulties.Count > 0)
            {
                context.Difficulties.AddRange(missingDifficulties);
            }
        }

        public static async Task SeedAsync(WalksDbContext context, CancellationToken cancellationToken)
        {
            var existingNames = (await context.Difficulties
                .AsNoTracking()
                .Select(x => x.Name)
                .ToListAsync(cancellationToken))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var missingDifficulties = DifficultyNames
                .Where(name => !existingNames.Contains(name))
                .Select(name => new Difficulty
                {
                    Id = Guid.NewGuid(),
                    Name = name
                })
                .ToList();

            if (missingDifficulties.Count > 0)
            {
                await context.Difficulties.AddRangeAsync(missingDifficulties, cancellationToken);
            }
        }
    }
}
