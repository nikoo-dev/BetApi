using BetliveApi.DTOs.Game;
using BetliveApi.Models;
using BetliveApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BetliveApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GamesController : ControllerBase
{
    private readonly IGameService _gameService;

    public GamesController(IGameService gameService)
    {
        _gameService = gameService;
    }

    /// <summary>Get all games</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GameDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var games = await _gameService.GetAllAsync();
        return Ok(games);
    }

    /// <summary>Get only upcoming games (bets open)</summary>
    [HttpGet("upcoming")]
    [ProducesResponseType(typeof(IEnumerable<GameDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUpcoming()
    {
        var games = await _gameService.GetUpcomingAsync();
        return Ok(games);
    }

    /// <summary>Get a single game by ID</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GameDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var game = await _gameService.GetByIdAsync(id);
        return Ok(game);
    }

    /// <summary>Create a new game — Admin only</summary>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(GameDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] CreateGameDto dto)
    {
        var game = await _gameService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = game.Id }, game);
    }

    /// <summary>Set game result and settle all bets — Admin only</summary>
    [HttpPatch("{id:guid}/result")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(GameDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetResult(Guid id, [FromBody] GameOutcome result)
    {
        var game = await _gameService.SetResultAsync(id, result);
        return Ok(game);
    }
}
