using BetliveApi.DTOs.Bet;

namespace BetliveApi.Services.Interfaces;

public interface IBetService
{
    Task<BetDto> PlaceBetAsync(Guid userId, PlaceBetDto dto);
    Task<IEnumerable<BetDto>> GetUserBetsAsync(Guid userId);
    Task SettleGameBetsAsync(Guid gameId); // called when admin sets game result
}
