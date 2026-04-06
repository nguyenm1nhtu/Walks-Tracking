using Microsoft.EntityFrameworkCore;
using Walks.API.Models.Entities;
using Walks.API.Models.Enums;

namespace Walks.API.Data
{
    public class WalksDbContext : DbContext
    {
        public WalksDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
        }

        public DbSet<Difficulty> Difficulties { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Walk> Walks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Difficulty>()
                .Property(difficulty => difficulty.Name)
                .HasConversion<string>()
                .HasDefaultValue(DifficultyLevel.Easy)
                .HasMaxLength(20);

            base.OnModelCreating(modelBuilder);
        }
    }
}
