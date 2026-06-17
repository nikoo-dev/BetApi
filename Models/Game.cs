namespace BetliveApi.Models;

public class Game
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string HomeTeam { get; set; } = string.Empty;

    public string AwayTeam { get; set; } = string.Empty;

    public string Sport { get; set; } = string.Empty; // "Football", "Basketball", etc.

    public DateTime StartsAt { get; set; }

    public GameStatus Status { get; set; } = GameStatus.Upcoming;

    // Odds for each outcome
    public decimal OddsHome { get; set; }   // e.g. 1.85
    public decimal OddsDraw { get; set; }   // e.g. 3.40
    public decimal OddsAway { get; set; }   // e.g. 4.20

    // Set after game finishes
    public GameOutcome? Result { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public ICollection<Bet> Bets { get; set; } = new List<Bet>();
}

public enum GameStatus
{
    Upcoming,   // not started yet, bets open
    Live,       // in progress
    Finished,   // result known
    Cancelled   // refunds issued
}

public enum GameOutcome
{
    HomeWin,
    Draw,
    AwayWin
}
