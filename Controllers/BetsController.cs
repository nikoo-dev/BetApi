using System.Security.Claims;
using BetliveApi.DTOs.Bet;
using BetliveApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BetliveApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BetsController : ControllerBase
{
    private readonly IBetService _betService;

    public BetsController(IBetService betService)
    {
        _betService = betService;
    }

    /// <summary>Place a bet on an upcoming game</summary>
    [HttpPost]
    [ProducesResponseType(typeof(BetDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> PlaceBet([FromBody] PlaceBetDto dto)
    {
        var userId = GetCurrentUserId();
        var bet = await _betService.PlaceBetAsync(userId, dto);
        return CreatedAtAction(nameof(GetMyBets), bet);
    }

    /// <summary>Get all bets placed by the logged-in user</summary>
    [HttpGet("my")]
    [ProducesResponseType(typeof(IEnumerable<BetDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyBets()
    {
        var userId = GetCurrentUserId();
        var bets = await _betService.GetUserBetsAsync(userId);
        return Ok(bets);
    }

    // Extracts the user's ID from the JWT token claims
    private Guid GetCurrentUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User ID not found in token.");
        return Guid.Parse(claim);
    }
}
