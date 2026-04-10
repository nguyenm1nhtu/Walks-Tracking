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
            ("Ninh Binh", "NB", "https://images.example.com/regions/ninh-binh.jpg"),
            ("Ho Chi Minh City", "HCM", "https://images.example.com/regions/ho-chi-minh.jpg"),
            ("Ha Long", "HL", "https://images.example.com/regions/ha-long.jpg"),
            ("Phu Quoc", "PQ", "https://images.example.com/regions/phu-quoc.jpg"),
            ("Hue", "HUE", "https://images.example.com/regions/hue.jpg"),
            ("Da Lat", "DL", "https://images.example.com/regions/da-lat.jpg"),
            ("Nha Trang", "NT", "https://images.example.com/regions/nha-trang.jpg"),
            ("Vung Tau", "VT", "https://images.example.com/regions/vung-tau.jpg"),
            ("Quy Nhon", "QN", "https://images.example.com/regions/quy-nhon.jpg"),
            ("Can Tho", "CT", "https://images.example.com/regions/can-tho.jpg"),
            ("Ha Giang", "HG", "https://images.example.com/regions/ha-giang.jpg"),
            ("Buon Ma Thuot", "BMT", "https://images.example.com/regions/buon-ma-thuot.jpg")
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
