namespace BetliveApi.Models;

public class Transaction
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // Foreign key
    public Guid UserId { get; set; }

    public decimal Amount { get; set; } // positive = money in, negative = money out

    public TransactionType Type { get; set; }

    public string Description { get; set; } = string.Empty; // e.g. "Bet placed on Man Utd vs Chelsea"

    // Optional link back to the bet that caused this transaction
    public Guid? BetId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public User User { get; set; } = null!;
}

public enum TransactionType
{
    Deposit,     // user topped up their wallet
    Withdrawal,  // user withdrew funds
    BetPlaced,   // money deducted when bet is placed
    BetWon,      // winnings credited
    BetRefunded  // refund from cancelled game
}
