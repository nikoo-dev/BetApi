namespace BetliveApi.Models;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string Role { get; set; } = "Player"; // "Player" or "Admin"

    public decimal Balance { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties (EF Core uses these to build relationships)
    public ICollection<Bet> Bets { get; set; } = new List<Bet>();
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
