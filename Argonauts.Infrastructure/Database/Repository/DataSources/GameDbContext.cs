using Argonauts.Infrastructure.Database.Entity;
using Microsoft.EntityFrameworkCore;

namespace Argonauts.Infrastructure.Database.Repository.DataSources;

/// <summary>
/// 
/// </summary>
/// <param name="options"></param>
public class GameDbContext(DbContextOptions<GameDbContext> options) : DbContext(options)
{
    /// <summary>
    /// 
    /// </summary>
    public DbSet<Galaxy> Galaxies { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public DbSet<Star> Stars { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public DbSet<Player> Players { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public DbSet<Spaceship> Spaceships { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public DbSet<SpaceshipCondition> SpaceshipConditions { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public DbSet<Balance> Balances { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public DbSet<SpaceshipStarVisit> SpaceshipStarVisits { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public DbSet<Quest> Quests { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // === Galaxy ===
        modelBuilder.Entity<Galaxy>(entity =>
        {
            entity.HasKey(g => g.Version);
            entity.HasMany(g => g.Stars)
                  .WithOne(s => s.Galaxy)
                  .HasForeignKey(s => s.GalaxyVersion)
                  .IsRequired()
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // === Star ===
        modelBuilder.Entity<Star>(entity =>
        {
            entity.HasKey(s => new { s.GalaxyVersion, s.Radius, s.AngleMilliradians });
            entity.HasIndex(s => new { s.GalaxyVersion, s.Radius, s.AngleMilliradians }).IsUnique();
            entity.Property(s => s.Type).HasMaxLength(50).IsRequired().HasDefaultValue("-");
        });

        // === Player ===
        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(p => p.Id);

            entity.HasOne(p => p.Spaceship)
              .WithOne(s => s.Owner)
              .HasForeignKey<Spaceship>(s => s.OwnerId)
              .IsRequired(false)
              .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(p => p.Login);
        });

        // === Spaceship ===
        modelBuilder.Entity<Spaceship>(entity =>
        {
            entity.HasKey(sp => sp.OwnerId);

            entity.HasOne(sp => sp.Owner)
                .WithOne(p => p.Spaceship)
                .HasForeignKey<Spaceship>(sp => sp.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(sp => sp.VisitedStars)
                .WithOne(v => v.Spaceship)
                .HasForeignKey(v => v.SpaceshipId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Balance>(entity =>
        {
            entity.HasKey(b => b.OwnerId);

            entity.HasOne(b => b.Spaceship)
                .WithOne(sp => sp.Balance)
                .HasForeignKey<Balance>(b => b.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        modelBuilder.Entity<SpaceshipCondition>(entity =>
        {
            entity.HasKey(sc => sc.OwnerId);

            entity.HasOne(sc => sc.Spaceship)
                .WithOne(sp => sp.SpaceshipCondition)
                .HasForeignKey<SpaceshipCondition>(sc => sc.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // === M-T-M: Spaceship - Star ===
        modelBuilder.Entity<SpaceshipStarVisit>(entity =>
        {
            entity.HasKey(v => new
            {
                v.SpaceshipId,
                v.StarGalaxyVersion,
                v.StarRadius,
                v.StarAngleMilliradians
            });

            entity.HasOne(v => v.Spaceship)
                .WithMany(s => s.VisitedStars)
                .HasForeignKey(v => v.SpaceshipId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(v => v.Star)
                .WithMany(s => s.VisitedByShips)
                .HasForeignKey(v => new
                {
                    v.StarGalaxyVersion,
                    v.StarRadius,
                    v.StarAngleMilliradians
                })
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(v => v.VisitedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Quest>(entity =>
        {
            entity.HasKey(q => q.OwnerId);

            entity.HasOne(q => q.Spaceship)
                .WithOne(sp => sp.Quest)
                .HasForeignKey<Quest>(q => q.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}