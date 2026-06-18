using BetliveApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BetliveApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Game> Games => Set<Game>();
    public DbSet<Bet> Bets => Set<Bet>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // --- User ---
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.HasIndex(u => u.Username).IsUnique();
            entity.Property(u => u.Balance).HasPrecision(18, 2);
        });

        // --- Game ---
        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(g => g.Id);
            entity.Property(g => g.OddsHome).HasPrecision(10, 2);
            entity.Property(g => g.OddsDraw).HasPrecision(10, 2);
            entity.Property(g => g.OddsAway).HasPrecision(10, 2);
        });

        // --- Bet ---
        modelBuilder.Entity<Bet>(entity =>
        {
            entity.HasKey(b => b.Id);
            entity.Property(b => b.Amount).HasPrecision(18, 2);
            entity.Property(b => b.OddsAtPlacement).HasPrecision(10, 2);
            entity.Property(b => b.PotentialWin).HasPrecision(18, 2);

            entity.HasOne(b => b.User)
                  .WithMany(u => u.Bets)
                  .HasForeignKey(b => b.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(b => b.Game)
                  .WithMany(g => g.Bets)
                  .HasForeignKey(b => b.GameId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // --- Transaction ---
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Amount).HasPrecision(18, 2);

            entity.HasOne(t => t.User)
                  .WithMany(u => u.Transactions)
                  .HasForeignKey(t => t.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // --- RefreshToken ---
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.HasIndex(r => r.Token).IsUnique();

            entity.HasOne(r => r.User)
                  .WithMany(u => u.RefreshTokens)
                  .HasForeignKey(r => r.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
