using Argonauts.Core.Repository;
using Argonauts.Infrastructure.Database.Entity;
using Argonauts.Infrastructure.Database.Mapping;
using Argonauts.Infrastructure.Database.Repository.DataSources;
using Microsoft.EntityFrameworkCore;

namespace Argonauts.Infrastructure.Database.Repository;

/// <summary>
/// Repository implementation for balance operations.
/// </summary>
public class EfBalanceRepository(GameDbContext context, AppToDbMapper mapper) : IBalanceRepository
{
    private readonly GameDbContext _context = context;
    private readonly AppToDbMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <param name="saveManually"></param>
    /// <returns></returns>
    public async Task CreateIfMissing(Guid user, bool saveManually = true)
    {
        var balance = await _context.Balances.AsNoTracking()
            .FirstOrDefaultAsync(s => s.OwnerId == user);
        if (balance == null)
        {
            _context.Balances.Add(new Balance { OwnerId = user });
        }
        if (saveManually)
        {
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Gets the balance for a specific user.
    /// </summary>
    /// <param name="user">The user's unique ID.</param>
    /// <returns>The user's balance.</returns>
    public async Task<Core.Entity.Player.Balance?> GetBalanceForUserAsync(Guid user)
    {
        var balance = await _context.Balances.AsNoTracking()
            .FirstOrDefaultAsync(s => s.OwnerId == user);
        if (balance == null)
            return null;
        return _mapper.ToDomainBalance(balance);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task AddCurrencyAsync(Guid user, int value)
    {
        var balance = await _context.Balances.FirstOrDefaultAsync(s => s.OwnerId == user);
        if (balance == null)
            return;
        balance.Currency += value;
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task AddQuantsAsync(Guid user, int value)
    {
        var balance = await _context.Balances.FirstOrDefaultAsync(s => s.OwnerId == user);
        if (balance == null)
            return;
        balance.Quants += value;
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task ClearBalanceAsync(Guid user)
    {
        var balance = await _context.Balances.FirstOrDefaultAsync(s => s.OwnerId == user);
        if (balance == null)
            return;
        balance.Quants = 0;
        balance.Currency = 0;
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <param name="currency"></param>
    /// <param name="quants"></param>
    /// <returns></returns>
    public async Task AddCurrencyAndQuantsAsync(Guid user, int currency, int quants)
    {
        var balance = await _context.Balances.FirstOrDefaultAsync(s => s.OwnerId == user);
        if (balance == null)
            return;
        balance.Currency += currency;
        balance.Quants += quants;
        await _context.SaveChangesAsync();
    }
}
