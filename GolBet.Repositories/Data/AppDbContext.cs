using GolBet.Entities;
using GolBet.Entities.Common;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace GolBet.Repositories.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Team> Teams => Set<Team>();

    public DbSet<Match> Matches => Set<Match>();

    public DbSet<Bet> Bets => Set<Bet>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Team names must be unique and case-insensitive.
        modelBuilder.Entity<Team>()
            .Property(team => team.Name)
            .UseCollation("SQL_Latin1_General_CP1_CI_AI");

        modelBuilder.Entity<Team>()
            .HasIndex(team => team.Name)
            .IsUnique();

        // Match has two relationships with Team.
        modelBuilder.Entity<Match>()
            .HasOne(match => match.HomeTeam)
            .WithMany()
            .HasForeignKey(match => match.HomeTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Match>()
            .HasOne(match => match.AwayTeam)
            .WithMany()
            .HasForeignKey(match => match.AwayTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        // A match with bets cannot be physically deleted.
        modelBuilder.Entity<Bet>()
            .HasOne(bet => bet.Match)
            .WithMany(match => match.Bets)
            .OnDelete(DeleteBehavior.Restrict);
    }

    public override Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        var utcNow = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedDate = utcNow;
                    break;

                case EntityState.Modified:
                    entry.Entity.ModifiedDate = utcNow;

                    // CreatedDate must not change after creation.
                    entry.Property(entity => entity.CreatedDate)
                        .IsModified = false;
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
