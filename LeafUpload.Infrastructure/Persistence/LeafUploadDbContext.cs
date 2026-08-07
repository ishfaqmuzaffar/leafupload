using LeafUpload.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace LeafUpload.Infrastructure.Persistence
{
    public class LeafUploadDbContext : DbContext
    {
        public LeafUploadDbContext(DbContextOptions<LeafUploadDbContext> options) : base(options)
        {
        }

        public DbSet<Farmer> Farmers => Set<Farmer>();
        public DbSet<Farm> Farms => Set<Farm>();
        public DbSet<Advisory> Advisories => Set<Advisory>();
        public DbSet<DeviceToken> DeviceTokens => Set<DeviceToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Farmer>()
                .HasIndex(f => f.Username)
                .IsUnique();

            modelBuilder.Entity<Farm>()
                .HasOne<Farmer>()
                .WithMany()
                .HasForeignKey(f => f.FarmerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Advisory>()
                .HasOne<Farm>()
                .WithMany()
                .HasForeignKey(a => a.FarmId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DeviceToken>()
                .HasIndex(t => t.Token)
                .IsUnique();

            modelBuilder.Entity<DeviceToken>()
                .HasOne<Farmer>()
                .WithMany()
                .HasForeignKey(t => t.FarmerId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
