using BetliveApi.DTOs.Bet;
using BetliveApi.Models;
using BetliveApi.Repositories.Interfaces;
using BetliveApi.Services.Interfaces;

namespace BetliveApi.Services;

public class BetService : IBetService
{
    private readonly IBetRepository         _betRepository;
    private readonly IUserRepository        _userRepository;
    private readonly IGameRepository        _gameRepository;
    private readonly ITransactionRepository _transactionRepository;

    public BetService(
        IBetRepository betRepository,
        IUserRepository userRepository,
        IGameRepository gameRepository,
        ITransactionRepository transactionRepository)
    {
        _betRepository         = betRepository;
        _userRepository        = userRepository;
        _gameRepository        = gameRepository;
        _transactionRepository = transactionRepository;
    }

    public async Task<BetDto> PlaceBetAsync(Guid userId, PlaceBetDto dto)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found.");

        var game = await _gameRepository.GetByIdAsync(dto.GameId)
            ?? throw new KeyNotFoundException("Game not found.");

        // Business rules
        if (game.Status != GameStatus.Upcoming)
            throw new ArgumentException("Bets can only be placed on upcoming games.");

        if (user.Balance < dto.Amount)
            throw new ArgumentException("Insufficient balance.");

        // Lock in current odds based on chosen outcome
        var oddsAtPlacement = dto.ChosenOutcome switch
        {
            GameOutcome.HomeWin => game.OddsHome,
            GameOutcome.Draw    => game.OddsDraw,
            GameOutcome.AwayWin => game.OddsAway,
            _                   => throw new ArgumentException("Invalid outcome.")
        };

        var bet = new Bet
        {
            UserId           = userId,
            GameId           = dto.GameId,
            Amount           = dto.Amount,
            ChosenOutcome    = dto.ChosenOutcome,
            OddsAtPlacement  = oddsAtPlacement,
            PotentialWin     = Math.Round(dto.Amount * oddsAtPlacement, 2),
            Status           = BetStatus.Pending
        };

        await _betRepository.CreateAsync(bet);

        // Deduct from balance
        user.Balance -= dto.Amount;
        await _userRepository.UpdateAsync(user);

        // Record the transaction
        await _transactionRepository.CreateAsync(new Transaction
        {
            UserId      = userId,
            Amount      = -dto.Amount,
            Type        = TransactionType.BetPlaced,
            Description = $"Bet on {game.HomeTeam} vs {game.AwayTeam}",
            BetId       = bet.Id
        });

        return MapToDto(bet, game);
    }

    public async Task<IEnumerable<BetDto>> GetUserBetsAsync(Guid userId)
    {
        var bets = await _betRepository.GetByUserIdAsync(userId);
        return bets.Select(b => MapToDto(b, b.Game));
    }

    public async Task SettleGameBetsAsync(Guid gameId)
    {
        var game = await _gameRepository.GetByIdAsync(gameId)
            ?? throw new KeyNotFoundException("Game not found.");

        var bets = await _betRepository.GetByGameIdAsync(gameId);

        foreach (var bet in bets.Where(b => b.Status == BetStatus.Pending))
        {
            var user = await _userRepository.GetByIdAsync(bet.UserId)
                ?? throw new KeyNotFoundException("User not found.");

            if (game.Result == null)
            {
                // Game cancelled — refund
                bet.Status    = BetStatus.Refunded;
                user.Balance += bet.Amount;

                await _transactionRepository.CreateAsync(new Transaction
                {
                    UserId      = user.Id,
                    Amount      = bet.Amount,
                    Type        = TransactionType.BetRefunded,
                    Description = $"Refund: {game.HomeTeam} vs {game.AwayTeam} cancelled",
                    BetId       = bet.Id
                });
            }
            else if (bet.ChosenOutcome == game.Result)
            {
                // Winner — credit winnings
                bet.Status    = BetStatus.Won;
                user.Balance += bet.PotentialWin;

                await _transactionRepository.CreateAsync(new Transaction
                {
                    UserId      = user.Id,
                    Amount      = bet.PotentialWin,
                    Type        = TransactionType.BetWon,
                    Description = $"Win: {game.HomeTeam} vs {game.AwayTeam}",
                    BetId       = bet.Id
                });
            }
            else
            {
                bet.Status = BetStatus.Lost;
            }

            await _betRepository.UpdateAsync(bet);
            await _userRepository.UpdateAsync(user);
        }
    }

    private static BetDto MapToDto(Bet bet, Game game) => new()
    {
        Id              = bet.Id,
        GameId          = bet.GameId,
        HomeTeam        = game.HomeTeam,
        AwayTeam        = game.AwayTeam,
        Amount          = bet.Amount,
        ChosenOutcome   = bet.ChosenOutcome,
        OddsAtPlacement = bet.OddsAtPlacement,
        PotentialWin    = bet.PotentialWin,
        Status          = bet.Status,
        PlacedAt        = bet.PlacedAt
    };
}
