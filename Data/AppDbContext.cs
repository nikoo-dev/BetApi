using Microsoft.EntityFrameworkCore;

namespace BetliveApi.Data;

// Models will be added in Step 2 (DB schema)
// This is the placeholder that Program.cs references now
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // DbSets added in Step 2:
    // public DbSet<User> Users => Set<User>();
    // public DbSet<Game> Games => Set<Game>();
    // public DbSet<Bet>  Bets  => Set<Bet>();
    // public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Fluent API configurations added in Step 2
    }
}
