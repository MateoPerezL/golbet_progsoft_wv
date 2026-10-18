using GolBet.Entities;
using GolBet.Entities.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GolBet.Repositories.Data;

public static class DbSeeder
{
    public const string AdminRole = "Admin";

    public const string BettorRole = "Apostador";

    public static async Task SeedAsync(
        AppDbContext context,
        RoleManager<IdentityRole> roleManager,
        UserManager<AppUser> userManager)
    {
        await context.Database.MigrateAsync();

        await SeedDomainAsync(context);

        await SeedIdentityAsync(
            roleManager,
            userManager);
    }

    private static async Task SeedDomainAsync(
        AppDbContext context)
    {
        if (await context.Teams.AnyAsync())
        {
            return;
        }

        var teams = new List<Team>
        {
            new()
            {
                Name = "Atlético Nacional",
                City = "Medellín",
                CrestUrl =
                    "https://placehold.co/80x80/006633/ffffff?text=NAC"
            },

            new( )
            {
                Name = "Independiente Medellín",
                City = "Medellín",
                CrestUrl =
                    "https://placehold.co/80x80/cc0000/ffffff?text=DIM"
            },

            new( )
            {
                Name = "Millonarios",
                City = "Bogotá",
                CrestUrl =
                    "https://placehold.co/80x80/003399/ffffff?text=MIL"
            },

            new( )
            {
                Name = "Independiente Santa Fe",
                City = "Bogotá",
                CrestUrl =
                    "https://placehold.co/80x80/cc0000/ffffff?text=SFE"
            },

            new( )
            {
                Name = "América de Cali",
                City = "Cali",
                CrestUrl =
                    "https://placehold.co/80x80/e60000/ffffff?text=AME"
            },

            new( )
            {
                Name = "Deportivo Cali",
                City = "Cali",
                CrestUrl =
                    "https://placehold.co/80x80/00794d/ffffff?text=CAL"
            },

            new( )
            {
                Name = "Junior de Barranquilla",
                City = "Barranquilla",
                CrestUrl =
                    "https://placehold.co/80x80/d40026/ffffff?text=JUN"
            },

            new( )
            {
                Name = "Once Caldas",
                City = "Manizales",
                CrestUrl =
                    "https://placehold.co/80x80/ffffff/000000?text=ONC"
            }
        };

        context.Teams.AddRange(teams);

        await context.SaveChangesAsync();

        var nacional = teams.First(
            team => team.Name == "Atlético Nacional");

        var medellin = teams.First(
            team => team.Name == "Independiente Medellín");

        var millonarios = teams.First(
            team => team.Name == "Millonarios");

        var santaFe = teams.First(
            team => team.Name == "Independiente Santa Fe");

        var matches = new List<Match>
        {
            new()
            {
                HomeTeamId = nacional.Id,
                AwayTeamId = medellin.Id,
                Date = DateTime.UtcNow.AddDays(3),
                HomeOdds = 1.85m,
                DrawOdds = 3.20m,
                AwayOdds = 3.90m,
                Status = MatchStatus.Scheduled
            },

            new()
            {
                HomeTeamId = millonarios.Id,
                AwayTeamId = santaFe.Id,
                Date = DateTime.UtcNow.AddDays(5),
                HomeOdds = 2.10m,
                DrawOdds = 3.10m,
                AwayOdds = 3.25m,
                Status = MatchStatus.Scheduled
            }
        };

        context.Matches.AddRange(matches);

        await context.SaveChangesAsync();
    }

    private static async Task SeedIdentityAsync(
        RoleManager<IdentityRole> roleManager,
        UserManager<AppUser> userManager)
    {
        var roles = new[]
        {
            AdminRole,
            BettorRole
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var roleResult = await roleManager
                    .CreateAsync(new IdentityRole(role));

                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(
                        "; ",
                        roleResult.Errors.Select(error =>
                            error.Description));

                    throw new InvalidOperationException(
                        $"No se pudo crear el rol {role}: {errors}");
                }
            }
        }

        const string adminEmail = "admin@golbet.local";

        var admin = await userManager.FindByEmailAsync(
            adminEmail);

        if (admin is null)
        {
            admin = new AppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FullName = "Administrador GolBet",
                Balance = 100000m
            };

            var createResult = await userManager.CreateAsync(
                admin,
                "Admin123");

            if (!createResult.Succeeded)
            {
                var errors = string.Join(
                    "; ",
                    createResult.Errors.Select(error =>
                        error.Description));

                throw new InvalidOperationException(
                    $"No se pudo crear el administrador: {errors}");
            }
        }

        if (!await userManager.IsInRoleAsync(
                admin,
                AdminRole))
        {
            await userManager.AddToRoleAsync(
                admin,
                AdminRole);
        }
    }
}
