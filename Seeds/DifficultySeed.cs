using Microsoft.EntityFrameworkCore;
using Walks.API.Data;
using Walks.API.Models.Enums;
using Walks.API.Models.Entities;

namespace Walks.API.Seeds
{
    public static class DifficultySeed
    {
        private const string NormalizeDifficultySql =
            "UPDATE Difficulties SET Name = 'Beginner' WHERE Name IN ('Easy','Beginner','Beginer');" +
            "UPDATE Difficulties SET Name = 'Immidiate' WHERE Name IN ('Medium','Intermediate','Immidiate');" +
            "UPDATE Difficulties SET Name = 'Avanced' WHERE Name IN ('Hard','Expert','Advanced','Avanced');" +
            "UPDATE Difficulties SET Name = 'Avanced' WHERE Name NOT IN ('Beginner','Immidiate','Avanced');";

        private static readonly DifficultyLevel[] DifficultyNames =
        [
            DifficultyLevel.Beginner,
            DifficultyLevel.Immidiate,
            DifficultyLevel.Avanced
        ];

        public static void Seed(WalksDbContext context)
        {
            context.Database.ExecuteSqlRaw(NormalizeDifficultySql);
            EnsureExactlyThreeDifficulties(context);
        }

        public static async Task SeedAsync(WalksDbContext context, CancellationToken cancellationToken)
        {
            await context.Database.ExecuteSqlRawAsync(NormalizeDifficultySql, cancellationToken);
            await EnsureExactlyThreeDifficultiesAsync(context, cancellationToken);
        }

        private static void EnsureExactlyThreeDifficulties(WalksDbContext context)
        {
            var difficulties = context.Difficulties.ToList();

            var canonicalByName = difficulties
                .GroupBy(x => x.Name)
                .ToDictionary(group => group.Key, group => group.OrderBy(x => x.Id).First());

            foreach (var name in DifficultyNames)
            {
                if (canonicalByName.ContainsKey(name))
                {
                    continue;
                }

                var difficulty = new Difficulty
                {
                    Id = Guid.NewGuid(),
                    Name = name
                };

                context.Difficulties.Add(difficulty);
                canonicalByName[name] = difficulty;
            }

            var duplicates = difficulties
                .GroupBy(x => x.Name)
                .SelectMany(group => group
                    .OrderBy(x => x.Id)
                    .Skip(1)
                    .Select(duplicate => new
                    {
                        Duplicate = duplicate,
                        CanonicalId = canonicalByName[group.Key].Id
                    }))
                .ToList();

            if (duplicates.Count == 0)
            {
                return;
            }

            var duplicateToCanonical = duplicates
                .ToDictionary(x => x.Duplicate.Id, x => x.CanonicalId);

            var duplicateIds = duplicateToCanonical.Keys.ToList();

            var walksToUpdate = context.Walks
                .Where(walk => duplicateIds.Contains(walk.DifficultyId))
                .ToList();

            foreach (var walk in walksToUpdate)
            {
                walk.DifficultyId = duplicateToCanonical[walk.DifficultyId];
            }

            context.Difficulties.RemoveRange(duplicates.Select(x => x.Duplicate));
        }

        private static async Task EnsureExactlyThreeDifficultiesAsync(WalksDbContext context, CancellationToken cancellationToken)
        {
            var difficulties = await context.Difficulties.ToListAsync(cancellationToken);

            var canonicalByName = difficulties
                .GroupBy(x => x.Name)
                .ToDictionary(group => group.Key, group => group.OrderBy(x => x.Id).First());

            foreach (var name in DifficultyNames)
            {
                if (canonicalByName.ContainsKey(name))
                {
                    continue;
                }

                var difficulty = new Difficulty
                {
                    Id = Guid.NewGuid(),
                    Name = name
                };

                await context.Difficulties.AddAsync(difficulty, cancellationToken);
                canonicalByName[name] = difficulty;
            }

            var duplicates = difficulties
                .GroupBy(x => x.Name)
                .SelectMany(group => group
                    .OrderBy(x => x.Id)
                    .Skip(1)
                    .Select(duplicate => new
                    {
                        Duplicate = duplicate,
                        CanonicalId = canonicalByName[group.Key].Id
                    }))
                .ToList();

            if (duplicates.Count == 0)
            {
                return;
            }

            var duplicateToCanonical = duplicates
                .ToDictionary(x => x.Duplicate.Id, x => x.CanonicalId);

            var duplicateIds = duplicateToCanonical.Keys.ToList();

            var walksToUpdate = await context.Walks
                .Where(walk => duplicateIds.Contains(walk.DifficultyId))
                .ToListAsync(cancellationToken);

            foreach (var walk in walksToUpdate)
            {
                walk.DifficultyId = duplicateToCanonical[walk.DifficultyId];
            }

            context.Difficulties.RemoveRange(duplicates.Select(x => x.Duplicate));
        }
    }
}
