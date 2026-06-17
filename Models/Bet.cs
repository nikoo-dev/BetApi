namespace BetliveApi.Models;

public class Bet
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // Foreign keys
    public Guid UserId { get; set; }
    public Guid GameId { get; set; }

    public decimal Amount { get; set; }         // how much the user bet

    public GameOutcome ChosenOutcome { get; set; } // what they predicted

    public decimal OddsAtPlacement { get; set; }   // locked-in odds when bet was placed

    public decimal PotentialWin { get; set; }       // Amount * OddsAtPlacement

    public BetStatus Status { get; set; } = BetStatus.Pending;

    public DateTime PlacedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User User { get; set; } = null!;
    public Game Game { get; set; } = null!;
}

public enum BetStatus
{
    Pending,  // game not finished yet
    Won,
    Lost,
    Refunded  // game was cancelled
}
