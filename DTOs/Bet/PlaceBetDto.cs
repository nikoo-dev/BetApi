using System.ComponentModel.DataAnnotations;
using BetliveApi.Models;

namespace BetliveApi.DTOs.Bet;

public class PlaceBetDto
{
    [Required]
    public Guid GameId { get; set; }

    [Required]
    public GameOutcome ChosenOutcome { get; set; }

    [Required]
    [Range(0.01, 100000)]
    public decimal Amount { get; set; }
}
