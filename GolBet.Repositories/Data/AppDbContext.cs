using GolBet.Entities;
using GolBet.Entities.Common;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GolBet.Repositories.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Team> Teams => Set<Team>();

    public DbSet<Match> Matches => Set<Match>();

    public DbSet<Bet> Bets => Set<Bet>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        // Debe ser la primera línea.
        // Aquí Identity configura sus tablas.
        base.OnModelCreating(modelBuilder);

        // Los nombres de los equipos son únicos.
        modelBuilder.Entity<Team>()
            .Property(team => team.Name)
            .UseCollation(
                "SQL_Latin1_General_CP1_CI_AI");

        modelBuilder.Entity<Team>()
            .HasIndex(team => team.Name)
            .IsUnique();

        // Un partido tiene dos relaciones diferentes con Team.
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

        // Un partido con apuestas no puede eliminarse físicamente.
        modelBuilder.Entity<Bet>()
            .HasOne(bet => bet.Match)
            .WithMany(match => match.Bets)
            .HasForeignKey(bet => bet.MatchId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación entre una apuesta y su usuario.
        modelBuilder.Entity<Bet>()
            .HasOne(bet => bet.User)
            .WithMany(user => user.Bets)
            .HasForeignKey(bet => bet.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Un usuario no puede repetir la misma selección
        // en el mismo partido.
        modelBuilder.Entity<Bet>()
            .HasIndex(bet => new
            {
                bet.MatchId,
                bet.UserId,
                bet.Pick
            })
            .IsUnique();

        // Precisión del saldo de FutCoins.
        modelBuilder.Entity<AppUser>()
            .Property(user => user.Balance)
            .HasPrecision(12, 2);
    }

    public override Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        var utcNow = DateTime.UtcNow;

        foreach (var entry in ChangeTracker
            .Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedDate = utcNow;
                    break;

                case EntityState.Modified:
                    entry.Entity.ModifiedDate = utcNow;

                    // CreatedDate no cambia después de crear.
                    entry.Property(entity =>
                        entity.CreatedDate).IsModified = false;
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
