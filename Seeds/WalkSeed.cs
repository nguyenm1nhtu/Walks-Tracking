using Microsoft.EntityFrameworkCore;
using Walks.API.Data;
using Walks.API.Models.Entities;

namespace Walks.API.Seeds
{
    public static class WalkSeed
    {
        private static readonly (string Name, string Description, double LengthInKm, string? WalkImageUrl, string DifficultyName, string RegionCode)[] Walks =
        [
            ("West Lake Morning Loop", "A short, easy walk around West Lake.", 5.2, "https://images.example.com/walks/west-lake-loop.jpg", "Easy", "HN"),
            ("Marble Mountains Trail", "A scenic medium route with stairs and viewpoints.", 8.4, "https://images.example.com/walks/marble-mountains.jpg", "Medium", "DN"),
            ("Fansipan Ridge Trek", "A long, challenging mountain trek.", 14.8, "https://images.example.com/walks/fansipan-ridge.jpg", "Hard", "SP"),
            ("Trang An Scenic Loop", "A longer expert walk through limestone valleys and rivers.", 10.7, "https://images.example.com/walks/trang-an-loop.jpg", "Expert", "NB")
        ];

        public static void Seed(WalksDbContext context)
        {
            var difficultyByName = context.Difficulties
                .AsNoTracking()
                .ToDictionary(x => x.Name, x => x.Id, StringComparer.OrdinalIgnoreCase);

            var regionByCode = context.Regions
                .AsNoTracking()
                .ToDictionary(x => x.Code, x => x.Id, StringComparer.OrdinalIgnoreCase);

            var existingWalkNames = context.Walks
                .AsNoTracking()
                .Select(x => x.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var missingWalks = new List<Walk>();

            foreach (var walk in Walks)
            {
                if (existingWalkNames.Contains(walk.Name))
                {
                    continue;
                }

                if (!difficultyByName.TryGetValue(walk.DifficultyName, out var difficultyId))
                {
                    continue;
                }

                if (!regionByCode.TryGetValue(walk.RegionCode, out var regionId))
                {
                    continue;
                }

                missingWalks.Add(new Walk
                {
                    Id = Guid.NewGuid(),
                    Name = walk.Name,
                    Description = walk.Description,
                    LengthInKm = walk.LengthInKm,
                    WalkImageUrl = walk.WalkImageUrl,
                    DifficultyId = difficultyId,
                    RegionId = regionId
                });
            }

            if (missingWalks.Count > 0)
            {
                context.Walks.AddRange(missingWalks);
            }
        }

        public static async Task SeedAsync(WalksDbContext context, CancellationToken cancellationToken)
        {
            var difficultyByName = (await context.Difficulties
                .AsNoTracking()
                .ToListAsync(cancellationToken))
                .ToDictionary(x => x.Name, x => x.Id, StringComparer.OrdinalIgnoreCase);

            var regionByCode = (await context.Regions
                .AsNoTracking()
                .ToListAsync(cancellationToken))
                .ToDictionary(x => x.Code, x => x.Id, StringComparer.OrdinalIgnoreCase);

            var existingWalkNames = (await context.Walks
                .AsNoTracking()
                .Select(x => x.Name)
                .ToListAsync(cancellationToken))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var missingWalks = new List<Walk>();

            foreach (var walk in Walks)
            {
                if (existingWalkNames.Contains(walk.Name))
                {
                    continue;
                }

                if (!difficultyByName.TryGetValue(walk.DifficultyName, out var difficultyId))
                {
                    continue;
                }

                if (!regionByCode.TryGetValue(walk.RegionCode, out var regionId))
                {
                    continue;
                }

                missingWalks.Add(new Walk
                {
                    Id = Guid.NewGuid(),
                    Name = walk.Name,
                    Description = walk.Description,
                    LengthInKm = walk.LengthInKm,
                    WalkImageUrl = walk.WalkImageUrl,
                    DifficultyId = difficultyId,
                    RegionId = regionId
                });
            }

            if (missingWalks.Count > 0)
            {
                await context.Walks.AddRangeAsync(missingWalks, cancellationToken);
            }
        }
    }
}
