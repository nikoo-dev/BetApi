using BetliveApi.DTOs.User;

namespace BetliveApi.Services.Interfaces;

public interface IUserService
{
    Task<UserDto> GetByIdAsync(Guid id);
    Task<UserDto> GetProfileAsync(Guid id);
    Task<decimal> GetBalanceAsync(Guid id);
    Task<UserDto> DepositAsync(Guid id, decimal amount);
}
