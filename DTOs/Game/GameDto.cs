using BetliveApi.Models;

namespace BetliveApi.DTOs.Game;

public class GameDto
{
    public Guid Id { get; set; }
    public string HomeTeam { get; set; } = string.Empty;
    public string AwayTeam { get; set; } = string.Empty;
    public string Sport { get; set; } = string.Empty;
    public DateTime StartsAt { get; set; }
    public GameStatus Status { get; set; }
    public decimal OddsHome { get; set; }
    public decimal OddsDraw { get; set; }
    public decimal OddsAway { get; set; }
    public GameOutcome? Result { get; set; }
}
