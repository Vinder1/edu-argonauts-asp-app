using Argonauts.Application.Services.Abstractions;
using Argonauts.Core.Entity.Player;
using Argonauts.Core.Repository;

namespace Argonauts.Application.Services.Implementations;

/// <summary>
/// Service implementation for managing balance operations.
/// </summary>
/// <remarks>
/// Initializes a new instance of the BalanceService class.
/// </remarks>
/// <param name="balanceRepository">The balance repository.</param>
public class BalanceService(IBalanceRepository balanceRepository) : IBalanceService
{
    private readonly IBalanceRepository _balanceRepository = balanceRepository
        ?? throw new ArgumentNullException(nameof(balanceRepository));

    /// <summary>
    /// Gets the balance for a specific user.
    /// </summary>
    /// <param name="user">The user's unique ID.</param>
    /// <returns>The user's balance.</returns>
    public Task<Balance?> GetForUserAsync(Guid user)
    {
        return _balanceRepository.GetBalanceForUserAsync(user);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public Task AddCurrencyAsync(Guid user, int value)
    {
        return _balanceRepository.AddCurrencyAsync(user, value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public Task AddQuantsAsync(Guid user, int value)
    {
        return _balanceRepository.AddQuantsAsync(user, value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public Task ClearBalanceAsync(Guid user)
    {
        return _balanceRepository.ClearBalanceAsync(user);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <param name="currency"></param>
    /// <param name="quants"></param>
    /// <returns></returns>
    public Task AddCurrencyAndQuantsAsync(Guid user, int currency, int quants)
    {
        return _balanceRepository.AddCurrencyAndQuantsAsync(user, currency, quants);
    }

}
