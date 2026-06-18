using BetliveApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BetliveApi.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // Only seed if database is empty
        if (await context.Users.AnyAsync() || await context.Games.AnyAsync())
            return;

        // --- Admin user ---
        var admin = new User
        {
            Id           = Guid.NewGuid(),
            Username     = "admin",
            Email        = "admin@betlive.ge",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            Role         = "Admin",
            Balance      = 0
        };

        // --- Sample games ---
        var games = new List<Game>
        {
            new()
            {
                HomeTeam = "Dinamo Tbilisi",
                AwayTeam = "Saburtalo",
                Sport    = "Football",
                StartsAt = DateTime.UtcNow.AddDays(1),
                OddsHome = 1.75m,
                OddsDraw = 3.20m,
                OddsAway = 4.50m,
                Status   = GameStatus.Upcoming
            },
            new()
            {
                HomeTeam = "Manchester City",
                AwayTeam = "Arsenal",
                Sport    = "Football",
                StartsAt = DateTime.UtcNow.AddDays(2),
                OddsHome = 1.90m,
                OddsDraw = 3.50m,
                OddsAway = 3.80m,
                Status   = GameStatus.Upcoming
            },
            new()
            {
                HomeTeam = "Barcelona",
                AwayTeam = "Real Madrid",
                Sport    = "Football",
                StartsAt = DateTime.UtcNow.AddDays(3),
                OddsHome = 2.10m,
                OddsDraw = 3.40m,
                OddsAway = 3.20m,
                Status   = GameStatus.Upcoming
            },
            new()
            {
                HomeTeam = "LA Lakers",
                AwayTeam = "Golden State Warriors",
                Sport    = "Basketball",
                StartsAt = DateTime.UtcNow.AddDays(1),
                OddsHome = 2.05m,
                OddsDraw = 15.00m,
                OddsAway = 1.80m,
                Status   = GameStatus.Upcoming
            }
        };

        context.Users.Add(admin);
        context.Games.AddRange(games);
        await context.SaveChangesAsync();

        Console.WriteLine("✓ Database seeded: admin user + 4 sample games created.");
        Console.WriteLine("  Admin credentials: admin@betlive.ge / Admin123!");
    }
}
