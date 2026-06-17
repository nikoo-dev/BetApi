using BetliveApi.DTOs.Game;
using BetliveApi.Models;
using BetliveApi.Repositories.Interfaces;
using BetliveApi.Services.Interfaces;

namespace BetliveApi.Services;

public class GameService : IGameService
{
    private readonly IGameRepository _gameRepository;
    private readonly IBetService _betService;

    public GameService(IGameRepository gameRepository, IBetService betService)
    {
        _gameRepository = gameRepository;
        _betService     = betService;
    }

    public async Task<IEnumerable<GameDto>> GetAllAsync()
    {
        var games = await _gameRepository.GetAllAsync();
        return games.Select(MapToDto);
    }

    public async Task<IEnumerable<GameDto>> GetUpcomingAsync()
    {
        var games = await _gameRepository.GetByStatusAsync(GameStatus.Upcoming);
        return games.Select(MapToDto);
    }

    public async Task<GameDto> GetByIdAsync(Guid id)
    {
        var game = await _gameRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Game {id} not found.");
        return MapToDto(game);
    }

    public async Task<GameDto> CreateAsync(CreateGameDto dto)
    {
        var game = new Game
        {
            HomeTeam  = dto.HomeTeam,
            AwayTeam  = dto.AwayTeam,
            Sport     = dto.Sport,
            StartsAt  = dto.StartsAt,
            OddsHome  = dto.OddsHome,
            OddsDraw  = dto.OddsDraw,
            OddsAway  = dto.OddsAway,
            Status    = GameStatus.Upcoming
        };

        await _gameRepository.CreateAsync(game);
        return MapToDto(game);
    }

    public async Task<GameDto> SetResultAsync(Guid id, GameOutcome result)
    {
        var game = await _gameRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Game {id} not found.");

        if (game.Status == GameStatus.Finished)
            throw new ArgumentException("Game result has already been set.");

        game.Result = result;
        game.Status = GameStatus.Finished;

        await _gameRepository.UpdateAsync(game);

        // Settle all bets for this game (pay out winners, mark losers)
        await _betService.SettleGameBetsAsync(id);

        return MapToDto(game);
    }

    private static GameDto MapToDto(Game game) => new()
    {
        Id       = game.Id,
        HomeTeam = game.HomeTeam,
        AwayTeam = game.AwayTeam,
        Sport    = game.Sport,
        StartsAt = game.StartsAt,
        Status   = game.Status,
        OddsHome = game.OddsHome,
        OddsDraw = game.OddsDraw,
        OddsAway = game.OddsAway,
        Result   = game.Result
    };
}
