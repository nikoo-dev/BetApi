using BetliveApi.Models;

namespace BetliveApi.DTOs.Bet;

public class BetDto
{
    public Guid Id { get; set; }
    public Guid GameId { get; set; }
    public string HomeTeam { get; set; } = string.Empty;
    public string AwayTeam { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public GameOutcome ChosenOutcome { get; set; }
    public decimal OddsAtPlacement { get; set; }
    public decimal PotentialWin { get; set; }
    public BetStatus Status { get; set; }
    public DateTime PlacedAt { get; set; }
}
