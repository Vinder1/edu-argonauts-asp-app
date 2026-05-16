using Argonauts.Core.Entity.Player;

namespace Argonauts.Application.Services.Abstractions;

/// <summary>
/// Service for managing balance operations.
/// </summary>
public interface IBalanceService
{
    /// <summary>
    /// Gets the balance for a specific user.
    /// </summary>
    /// <param name="user">The user's unique ID.</param>
    /// <returns>The user's balance.</returns>
    Task<Balance?> GetForUserAsync(Guid user);

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
