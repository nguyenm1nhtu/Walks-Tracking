using Microsoft.EntityFrameworkCore;
using Walks.API.Data;
using Walks.API.Models.Entities;

namespace Walks.API.Seeds
{
    public static class RegionSeed
    {
        private static readonly (string Name, string Code, string? RegionImageUrl)[] Regions =
        [
            ("Hanoi", "HN", "https://images.example.com/regions/hanoi.jpg"),
            ("Da Nang", "DN", "https://images.example.com/regions/danang.jpg"),
            ("Sa Pa", "SP", "https://images.example.com/regions/sapa.jpg"),
            ("Ninh Binh", "NB", "https://images.example.com/regions/ninh-binh.jpg")
        ];

        public static void Seed(WalksDbContext context)
        {
            var existingCodes = context.Regions
                .AsNoTracking()
                .Select(x => x.Code)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var missingRegions = Regions
                .Where(region => !existingCodes.Contains(region.Code))
                .Select(region => new Region
                {
                    Id = Guid.NewGuid(),
                    Name = region.Name,
                    Code = region.Code,
                    RegionImageUrl = region.RegionImageUrl
                })
                .ToList();

            if (missingRegions.Count > 0)
            {
                context.Regions.AddRange(missingRegions);
            }
        }

        public static async Task SeedAsync(WalksDbContext context, CancellationToken cancellationToken)
        {
            var existingCodes = (await context.Regions
                .AsNoTracking()
                .Select(x => x.Code)
                .ToListAsync(cancellationToken))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var missingRegions = Regions
                .Where(region => !existingCodes.Contains(region.Code))
                .Select(region => new Region
                {
                    Id = Guid.NewGuid(),
                    Name = region.Name,
                    Code = region.Code,
                    RegionImageUrl = region.RegionImageUrl
                })
                .ToList();

            if (missingRegions.Count > 0)
            {
                await context.Regions.AddRangeAsync(missingRegions, cancellationToken);
            }
        }
    }
}
