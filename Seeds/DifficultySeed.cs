using Microsoft.EntityFrameworkCore;
using Walks.API.Data;
using Walks.API.Models.Enums;
using Walks.API.Models.Entities;

namespace Walks.API.Seeds
{
    public static class DifficultySeed
    {
        private static readonly DifficultyLevel[] DifficultyNames =
        [
            DifficultyLevel.Easy,
            DifficultyLevel.Immidiate,
            DifficultyLevel.Advanced
        ];

        public static void Seed(WalksDbContext context)
        {
            context.Database.ExecuteSqlRaw(
                "UPDATE Difficulties SET Name = 'Easy' WHERE Name IN ('Easy','Beginner','Beginer');" +
                "UPDATE Difficulties SET Name = 'Immidiate' WHERE Name IN ('Medium','Intermediate','Immidiate');" +
                "UPDATE Difficulties SET Name = 'Advanced' WHERE Name IN ('Hard','Expert','Advanced');" +
                "UPDATE Difficulties SET Name = 'Advanced' WHERE Name NOT IN ('Easy','Immidiate','Advanced');");

            var existingNames = context.Difficulties
                .AsNoTracking()
                .Select(x => x.Name)
                .ToHashSet();

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
            await context.Database.ExecuteSqlRawAsync(
                "UPDATE Difficulties SET Name = 'Easy' WHERE Name IN ('Easy','Beginner','Beginer');" +
                "UPDATE Difficulties SET Name = 'Immidiate' WHERE Name IN ('Medium','Intermediate','Immidiate');" +
                "UPDATE Difficulties SET Name = 'Advanced' WHERE Name IN ('Hard','Expert','Advanced');" +
                "UPDATE Difficulties SET Name = 'Advanced' WHERE Name NOT IN ('Easy','Immidiate','Advanced');",
                cancellationToken);

            var existingNames = (await context.Difficulties
                .AsNoTracking()
                .Select(x => x.Name)
                .ToListAsync(cancellationToken))
                .ToHashSet();

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
