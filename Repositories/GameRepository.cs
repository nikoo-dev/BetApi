using BetliveApi.Data;
using BetliveApi.Models;
using BetliveApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BetliveApi.Repositories;

public class GameRepository : IGameRepository
{
    private readonly AppDbContext _context;

    public GameRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Game?> GetByIdAsync(Guid id)
        => await _context.Games.FindAsync(id);

    public async Task<IEnumerable<Game>> GetAllAsync()
        => await _context.Games
            .OrderBy(g => g.StartsAt)
            .ToListAsync();

    public async Task<IEnumerable<Game>> GetByStatusAsync(GameStatus status)
        => await _context.Games
            .Where(g => g.Status == status)
            .OrderBy(g => g.StartsAt)
            .ToListAsync();

    public async Task<Game> CreateAsync(Game game)
    {
        _context.Games.Add(game);
        await _context.SaveChangesAsync();
        return game;
    }

    public async Task<Game> UpdateAsync(Game game)
    {
        _context.Games.Update(game);
        await _context.SaveChangesAsync();
        return game;
    }

    public async Task DeleteAsync(Guid id)
    {
        var game = await GetByIdAsync(id);
        if (game is null) throw new KeyNotFoundException($"Game {id} not found.");
        _context.Games.Remove(game);
        await _context.SaveChangesAsync();
    }
}
