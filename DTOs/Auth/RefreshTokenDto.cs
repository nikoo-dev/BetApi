using System.ComponentModel.DataAnnotations;

namespace BetliveApi.DTOs.Auth;

public class RefreshTokenDto
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
