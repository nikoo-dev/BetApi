using BetliveApi.Data;
using BetliveApi.Models;
using BetliveApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BetliveApi.Repositories;

public class BetRepository : IBetRepository
{
    private readonly AppDbContext _context;

    public BetRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Bet?> GetByIdAsync(Guid id)
        => await _context.Bets
            .Include(b => b.Game)  // also load the related Game
            .Include(b => b.User)  // also load the related User
            .FirstOrDefaultAsync(b => b.Id == id);

    public async Task<IEnumerable<Bet>> GetByUserIdAsync(Guid userId)
        => await _context.Bets
            .Include(b => b.Game)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.PlacedAt)
            .ToListAsync();

    public async Task<IEnumerable<Bet>> GetByGameIdAsync(Guid gameId)
        => await _context.Bets
            .Include(b => b.User)
            .Where(b => b.GameId == gameId)
            .ToListAsync();

    public async Task<Bet> CreateAsync(Bet bet)
    {
        _context.Bets.Add(bet);
        await _context.SaveChangesAsync();
        return bet;
    }

    public async Task<Bet> UpdateAsync(Bet bet)
    {
        _context.Bets.Update(bet);
        await _context.SaveChangesAsync();
        return bet;
    }
}
