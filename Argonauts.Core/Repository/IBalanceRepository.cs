using Argonauts.Core.Entity.Player;

namespace Argonauts.Core.Repository;

/// <summary>
/// Repository for balance-related data operations.
/// </summary>
public interface IBalanceRepository
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <param name="saveManually"></param>
    /// <returns></returns>
    public Task CreateIfMissing(Guid user, bool saveManually = true);

    /// <summary>
    /// Gets the balance for a specific user.
    /// </summary>
    /// <param name="user">The user's unique ID.</param>
    /// <returns>The user's balance.</returns>
    Task<Balance?> GetBalanceForUserAsync(Guid user);

    /// <summary>
    /// Adds some currency to user.
    /// </summary>
    /// <param name="user">The user's unique ID.</param>
    /// <param name="value"></param>
    /// <returns>The user's balance.</returns>
    Task AddCurrencyAsync(Guid user, int value);

    /// <summary>
    /// Adds some currency to user.
    /// </summary>
    /// <param name="user">The user's unique ID.</param>
    /// <param name="value"></param>
    /// <returns>The user's balance.</returns>
    Task AddQuantsAsync(Guid user, int value);
    
    /// <summary>
    /// Adds some currency to user.
    /// </summary>
    /// <param name="user">The user's unique ID.</param>
    /// <param name="currency"></param>
    /// <param name="quants"></param>
    /// <returns>The user's balance.</returns>
    Task AddCurrencyAndQuantsAsync(Guid user, int currency, int quants);
    
    /// <summary>
    /// Adds some currency to user.
    /// </summary>
    /// <param name="user">The user's unique ID.</param>
    /// <returns>The user's balance.</returns>
    Task ClearBalanceAsync(Guid user);
}
