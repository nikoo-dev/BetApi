using BetliveApi.DTOs.Game;
using BetliveApi.Models;

namespace BetliveApi.Services.Interfaces;

public interface IGameService
{
    Task<IEnumerable<GameDto>> GetAllAsync();
    Task<IEnumerable<GameDto>> GetUpcomingAsync();
    Task<GameDto> GetByIdAsync(Guid id);
    Task<GameDto> CreateAsync(CreateGameDto dto);
    Task<GameDto> SetResultAsync(Guid id, GameOutcome result);
}
