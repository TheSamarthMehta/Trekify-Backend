using Microsoft.EntityFrameworkCore;
using Trekify.API.Models;

namespace Trekify.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Trek> Treks { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User entity configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Password).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        // Trek entity configuration
        modelBuilder.Entity<Trek>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TrekName).HasMaxLength(255);
            entity.Property(e => e.State).HasMaxLength(100);
            entity.Property(e => e.TrekType).HasMaxLength(100);
            entity.Property(e => e.DifficultyLevel).HasMaxLength(50);
            entity.Property(e => e.Season).HasMaxLength(100);
            entity.Property(e => e.Duration).HasMaxLength(100);
            entity.Property(e => e.Distance).HasMaxLength(100);
            entity.Property(e => e.MaxAltitude).HasMaxLength(100);
            entity.Property(e => e.TrekDescription).HasMaxLength(2000);
            entity.Property(e => e.Image).HasMaxLength(500);
            entity.Property(e => e.AgeGroup).HasMaxLength(50);
            entity.Property(e => e.GuideNeeded).HasMaxLength(20);
            entity.Property(e => e.SnowTrek).HasMaxLength(10);
            entity.Property(e => e.RecommendedGear).HasMaxLength(1000);
        });
    }
}