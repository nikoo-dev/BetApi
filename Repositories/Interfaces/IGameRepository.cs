using BetliveApi.Models;

namespace BetliveApi.Repositories.Interfaces;

public interface IGameRepository
{
    Task<Game?> GetByIdAsync(Guid id);
    Task<IEnumerable<Game>> GetAllAsync();
    Task<IEnumerable<Game>> GetByStatusAsync(GameStatus status);
    Task<Game> CreateAsync(Game game);
    Task<Game> UpdateAsync(Game game);
    Task DeleteAsync(Guid id);
}
