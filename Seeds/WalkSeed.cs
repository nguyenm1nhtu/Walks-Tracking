using Microsoft.EntityFrameworkCore;
using Walks.API.Data;
using Walks.API.Models.Enums;
using Walks.API.Models.Entities;

namespace Walks.API.Seeds
{
    public static class WalkSeed
    {
        private static readonly (string Name, string Description, double LengthInKm, string? WalkImageUrl, DifficultyLevel DifficultyName, string RegionCode)[] Walks =
        [
            ("West Lake Morning Loop", "A short, easy walk around West Lake.", 5.2, "https://images.example.com/walks/west-lake-loop.jpg", DifficultyLevel.Beginner, "HN"),
            ("Marble Mountains Trail", "A scenic medium route with stairs and viewpoints.", 8.4, "https://images.example.com/walks/marble-mountains.jpg", DifficultyLevel.Immidiate, "DN"),
            ("Fansipan Ridge Trek", "A long, challenging mountain trek.", 14.8, "https://images.example.com/walks/fansipan-ridge.jpg", DifficultyLevel.Avanced, "SP"),
            ("Trang An Scenic Loop", "A longer expert walk through limestone valleys and rivers.", 10.7, "https://images.example.com/walks/trang-an-loop.jpg", DifficultyLevel.Avanced, "NB"),
            ("Old Quarter Heritage Walk", "A cultural walk through old streets and local food spots.", 4.1, "https://images.example.com/walks/old-quarter-heritage.jpg", DifficultyLevel.Beginner, "HN"),
            ("My Khe Coastline Walk", "A flat beachside path ideal for relaxed pacing.", 5.7, "https://images.example.com/walks/my-khe-coastline.jpg", DifficultyLevel.Beginner, "DN"),
            ("Son Tra Peninsula Climb", "A hill route with elevation and panoramic sea views.", 11.2, "https://images.example.com/walks/son-tra-peninsula.jpg", DifficultyLevel.Avanced, "DN"),
            ("Muong Hoa Valley Path", "Rolling valley trails through villages and rice fields.", 9.1, "https://images.example.com/walks/muong-hoa-valley.jpg", DifficultyLevel.Immidiate, "SP"),
            ("Tam Coc Riverside Trail", "A calm route along rivers and limestone mountains.", 7.4, "https://images.example.com/walks/tam-coc-riverside.jpg", DifficultyLevel.Beginner, "NB"),
            ("Bich Dong Temple Climb", "Temple stairs and hill climbs for a moderate challenge.", 8.9, "https://images.example.com/walks/bich-dong-temple.jpg", DifficultyLevel.Immidiate, "NB"),
            ("Saigon River Park Circuit", "Urban green spaces linked by a smooth river route.", 6.2, "https://images.example.com/walks/saigon-river-park.jpg", DifficultyLevel.Beginner, "HCM"),
            ("Cu Chi Forest Trail", "Forest route with mixed surfaces and longer distance.", 12.6, "https://images.example.com/walks/cu-chi-forest.jpg", DifficultyLevel.Avanced, "HCM"),
            ("Bai Chay Bayfront Walk", "A waterfront boardwalk route with mild distance.", 5.5, "https://images.example.com/walks/bai-chay-bayfront.jpg", DifficultyLevel.Beginner, "HL"),
            ("Long Beach Morning Path", "A long but gentle beach route suitable for beginners.", 7.0, "https://images.example.com/walks/long-beach-morning.jpg", DifficultyLevel.Beginner, "PQ"),
            ("Ham Ninh Hill Circuit", "A mixed coastal and hill circuit with moderate climbs.", 10.5, "https://images.example.com/walks/ham-ninh-hill.jpg", DifficultyLevel.Immidiate, "PQ"),
            ("Hanoi Citadel Night Walk", "An evening urban route around the historic citadel district.", 4.8, "https://images.example.com/walks/hanoi-citadel-night.jpg", DifficultyLevel.Beginner, "HN"),
            ("Hoan Kiem Lake Circuit", "A compact lake circuit through the city center landmarks.", 3.9, "https://images.example.com/walks/hoan-kiem-circuit.jpg", DifficultyLevel.Beginner, "HN"),
            ("Da Nang River Bridge Route", "A city route linking major river bridges with moderate distance.", 8.1, "https://images.example.com/walks/danang-river-bridge.jpg", DifficultyLevel.Immidiate, "DN"),
            ("Hai Van Pass Foot Trail", "A challenging mountain-side trail segment on Hai Van Pass.", 13.9, "https://images.example.com/walks/hai-van-pass-foot.jpg", DifficultyLevel.Avanced, "DN"),
            ("O Quy Ho Mountain Approach", "A high-altitude climb with long uphill sections and valley views.", 15.2, "https://images.example.com/walks/o-quy-ho-approach.jpg", DifficultyLevel.Avanced, "SP"),
            ("Sapa Bamboo Forest Walk", "A forest route through bamboo groves and hillside villages.", 9.6, "https://images.example.com/walks/sapa-bamboo-forest.jpg", DifficultyLevel.Immidiate, "SP"),
            ("Van Long Wetland Path", "A flat scenic route near wetlands and limestone formations.", 6.4, "https://images.example.com/walks/van-long-wetland.jpg", DifficultyLevel.Beginner, "NB"),
            ("Hoa Lu Ancient Capital Trek", "A route between temple complexes with moderate climbs.", 8.7, "https://images.example.com/walks/hoa-lu-ancient-capital.jpg", DifficultyLevel.Immidiate, "NB"),
            ("Thu Duc Greenbelt Walk", "A relaxed suburban greenbelt walk with paved trails.", 5.6, "https://images.example.com/walks/thu-duc-greenbelt.jpg", DifficultyLevel.Beginner, "HCM"),
            ("Can Gio Mangrove Expedition", "A long route through mangrove zones and rugged coastal terrain.", 16.1, "https://images.example.com/walks/can-gio-mangrove.jpg", DifficultyLevel.Avanced, "HCM"),
            ("Sun World Cliffside Route", "A coastal cliffside route with stairs and panoramic viewpoints.", 9.4, "https://images.example.com/walks/sun-world-cliffside.jpg", DifficultyLevel.Immidiate, "HL"),
            ("Bai Tu Long Coastal Trek", "A challenging coastal trek with long exposed segments.", 14.3, "https://images.example.com/walks/bai-tu-long-coastal.jpg", DifficultyLevel.Avanced, "HL"),
            ("Duong Dong Harbor Walk", "A morning harbor walk with easy terrain and sea breeze.", 5.1, "https://images.example.com/walks/duong-dong-harbor.jpg", DifficultyLevel.Beginner, "PQ"),
            ("Ganh Dau Shoreline Trek", "A moderate shoreline trek across mixed beach and rock terrain.", 9.0, "https://images.example.com/walks/ganh-dau-shoreline.jpg", DifficultyLevel.Immidiate, "PQ"),
            ("Yen So Urban Park Loop", "A large park loop with smooth paths and open lake views.", 6.6, "https://images.example.com/walks/yen-so-urban-park.jpg", DifficultyLevel.Beginner, "HN"),
            ("Non Nuoc Heritage Steps", "A stair-focused heritage route through marble hill passages.", 7.8, "https://images.example.com/walks/non-nuoc-heritage-steps.jpg", DifficultyLevel.Immidiate, "DN"),
            ("Ta Van Valley Ascent", "A steep ascent from valley floor to ridge villages.", 12.9, "https://images.example.com/walks/ta-van-valley-ascent.jpg", DifficultyLevel.Avanced, "SP"),
            ("Trang An Cave Connector", "A connector route between cave access points and valley paths.", 8.3, "https://images.example.com/walks/trang-an-cave-connector.jpg", DifficultyLevel.Immidiate, "NB"),
            ("District 1 Historical Route", "A central-city historical walk through iconic architecture.", 4.7, "https://images.example.com/walks/district-1-historical.jpg", DifficultyLevel.Beginner, "HCM"),
            ("Ham Rong Bay Ridge Walk", "A high ridge route above the bay with sustained climbs.", 13.5, "https://images.example.com/walks/ham-rong-bay-ridge.jpg", DifficultyLevel.Avanced, "HL"),
            ("Imperial City Riverside Walk", "A gentle route along Perfume River and old citadel walls.", 5.3, "https://images.example.com/walks/imperial-city-riverside.jpg", DifficultyLevel.Beginner, "HUE"),
            ("Ngu Binh Hill Climb", "A moderate climb to viewpoints across Hue's river plain.", 9.2, "https://images.example.com/walks/ngu-binh-hill-climb.jpg", DifficultyLevel.Immidiate, "HUE"),
            ("Lang Co Bay Ridge Route", "A long ridge route with exposed sections and sea panoramas.", 13.1, "https://images.example.com/walks/lang-co-bay-ridge.jpg", DifficultyLevel.Avanced, "HUE"),
            ("Xuan Huong Lake Loop", "A beginner-friendly loop around Da Lat's iconic central lake.", 4.9, "https://images.example.com/walks/xuan-huong-lake-loop.jpg", DifficultyLevel.Beginner, "DL"),
            ("Lang Biang Summit Trek", "A challenging trek with steep gradients to Lang Biang summit.", 15.4, "https://images.example.com/walks/lang-biang-summit-trek.jpg", DifficultyLevel.Avanced, "DL"),
            ("Da Lat Pine Forest Route", "A cool-weather forest walk through pine-covered hills.", 8.6, "https://images.example.com/walks/da-lat-pine-forest.jpg", DifficultyLevel.Immidiate, "DL"),
            ("Tran Phu Coastal Walk", "A flat seaside walk along Nha Trang's main beach boulevard.", 6.1, "https://images.example.com/walks/tran-phu-coastal.jpg", DifficultyLevel.Beginner, "NT"),
            ("Hon Chong Cliff Trail", "A scenic coastal trail over rocky outcrops and viewpoints.", 9.5, "https://images.example.com/walks/hon-chong-cliff-trail.jpg", DifficultyLevel.Immidiate, "NT"),
            ("Vung Tau Lighthouse Ascent", "A stair-and-road ascent to the lighthouse with bay views.", 8.0, "https://images.example.com/walks/vung-tau-lighthouse-ascent.jpg", DifficultyLevel.Immidiate, "VT"),
            ("Back Beach Sunrise Loop", "A relaxed morning loop along Back Beach promenade.", 5.8, "https://images.example.com/walks/back-beach-sunrise-loop.jpg", DifficultyLevel.Beginner, "VT"),
            ("Eo Gio Windy Trail", "A rugged coastal route on windy cliffs near Quy Nhon.", 12.7, "https://images.example.com/walks/eo-gio-windy-trail.jpg", DifficultyLevel.Avanced, "QN"),
            ("Ky Co Coast Path", "A moderate coast path combining sand, stairs, and rocky edges.", 9.3, "https://images.example.com/walks/ky-co-coast-path.jpg", DifficultyLevel.Immidiate, "QN"),
            ("Ninh Kieu Riverside Walk", "A beginner riverside walk across Can Tho's night market area.", 5.4, "https://images.example.com/walks/ninh-kieu-riverside.jpg", DifficultyLevel.Beginner, "CT"),
            ("Ma Pi Leng Ridge Trek", "An advanced mountain route with dramatic canyon viewpoints.", 16.8, "https://images.example.com/walks/ma-pi-leng-ridge-trek.jpg", DifficultyLevel.Avanced, "HG"),
            ("Dray Nur Waterfall Route", "A mixed-terrain route connecting forest tracks to waterfall points.", 10.2, "https://images.example.com/walks/dray-nur-waterfall-route.jpg", DifficultyLevel.Immidiate, "BMT")
        ];

        public static void Seed(WalksDbContext context)
        {
            var difficultyByName = context.Difficulties
                .AsNoTracking()
                .GroupBy(x => x.Name)
                .ToDictionary(group => group.Key, group => group.First().Id);

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
                .GroupBy(x => x.Name)
                .ToDictionary(group => group.Key, group => group.First().Id);

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
