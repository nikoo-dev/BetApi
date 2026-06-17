using BetliveApi.DTOs.User;
using BetliveApi.Models;
using BetliveApi.Repositories.Interfaces;
using BetliveApi.Services.Interfaces;

namespace BetliveApi.Services;

public class UserService : IUserService
{
    private readonly IUserRepository        _userRepository;
    private readonly ITransactionRepository _transactionRepository;

    public UserService(IUserRepository userRepository, ITransactionRepository transactionRepository)
    {
        _userRepository        = userRepository;
        _transactionRepository = transactionRepository;
    }

    public async Task<UserDto> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("User not found.");
        return MapToDto(user);
    }

    public async Task<UserDto> GetProfileAsync(Guid id)
        => await GetByIdAsync(id);

    public async Task<decimal> GetBalanceAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("User not found.");
        return user.Balance;
    }

    public async Task<UserDto> DepositAsync(Guid id, decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Deposit amount must be greater than zero.");

        var user = await _userRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("User not found.");

        user.Balance += amount;
        await _userRepository.UpdateAsync(user);

        await _transactionRepository.CreateAsync(new Transaction
        {
            UserId      = id,
            Amount      = amount,
            Type        = TransactionType.Deposit,
            Description = $"Wallet deposit of {amount:F2} GEL"
        });

        return MapToDto(user);
    }

    private static UserDto MapToDto(User user) => new()
    {
        Id        = user.Id,
        Username  = user.Username,
        Email     = user.Email,
        Role      = user.Role,
        Balance   = user.Balance,
        CreatedAt = user.CreatedAt
    };
}
