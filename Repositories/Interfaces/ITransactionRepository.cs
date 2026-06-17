using BetliveApi.Models;

namespace BetliveApi.Repositories.Interfaces;

public interface ITransactionRepository
{
    Task<IEnumerable<Transaction>> GetByUserIdAsync(Guid userId);
    Task<Transaction> CreateAsync(Transaction transaction);
}
