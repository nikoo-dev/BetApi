using BetliveApi.Models;

namespace BetliveApi.Repositories.Interfaces;

public interface IBetRepository
{
    Task<Bet?> GetByIdAsync(Guid id);
    Task<IEnumerable<Bet>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<Bet>> GetByGameIdAsync(Guid gameId);
    Task<Bet> CreateAsync(Bet bet);
    Task<Bet> UpdateAsync(Bet bet);
}
