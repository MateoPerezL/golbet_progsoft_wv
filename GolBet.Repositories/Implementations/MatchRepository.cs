using GolBet.Entities;
using GolBet.Entities.Enums;
using GolBet.Repositories.Data;
using GolBet.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GolBet.Repositories.Implementations;

public class MatchRepository
    : GenericRepository<Match>, IMatchRepository
{
    public MatchRepository(AppDbContext context)
        : base(context)
    {
    }

    public async Task<IEnumerable<Match>> GetAllWithTeamsAsync(
        MatchStatus? status = null)
    {
        var query = _dbSet
            .Include(match => match.HomeTeam)
            .Include(match => match.AwayTeam)
            .Where(match => match.IsActive)
            .AsNoTracking()
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(match => match.Status == status.Value);
        }

        return await query
            .OrderBy(match => match.Date)
            .ToListAsync();
    }

    public async Task<Match?> GetByIdWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(match => match.HomeTeam)
            .Include(match => match.AwayTeam)
            .Include(match => match.Bets)
            .AsNoTracking()
            .FirstOrDefaultAsync(match => match.Id == id);
    }
}
