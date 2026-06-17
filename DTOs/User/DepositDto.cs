using System.ComponentModel.DataAnnotations;

namespace BetliveApi.DTOs.User;

public class DepositDto
{
    [Required]
    [Range(1, 100000)]
    public decimal Amount { get; set; }
}
