using System.ComponentModel.DataAnnotations;

namespace BetliveApi.DTOs.Game;

public class CreateGameDto
{
    [Required]
    public string HomeTeam { get; set; } = string.Empty;

    [Required]
    public string AwayTeam { get; set; } = string.Empty;

    [Required]
    public string Sport { get; set; } = string.Empty;

    [Required]
    public DateTime StartsAt { get; set; }

    [Range(1.01, 1000)]
    public decimal OddsHome { get; set; }

    [Range(1.01, 1000)]
    public decimal OddsDraw { get; set; }

    [Range(1.01, 1000)]
    public decimal OddsAway { get; set; }
}
