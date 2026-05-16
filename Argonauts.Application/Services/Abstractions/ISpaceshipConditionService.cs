using Argonauts.Application.Dto;
using Argonauts.Core.Entity.Player;

namespace Argonauts.Application.Services.Abstractions;

/// <summary>
/// Service for managing spaceship condition operations.
/// </summary>
public interface ISpaceshipConditionService
{
    /// <summary>
    /// Gets the spaceship condition for a specific user.
    /// </summary>
    /// <param name="user">The user's unique ID.</param>
    /// <returns>The spaceship condition.</returns>
    Task<SpaceshipCondition?> GetForUserAsync(Guid user);
    /// <summary>
    /// Gets the spaceship condition for a specific user.
    /// </summary>
    /// <param name="user">The user's unique ID.</param>
    /// <param name="checkRequirements"></param>
    /// <returns>The spaceship condition.</returns>
    Task<ServiceActionResult> RestoreAsync(Guid user, bool checkRequirements = true);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="guid"></param>
    /// <param name="condition"></param>
    /// <returns></returns>
    Task UpdateAsync(Guid guid, SpaceshipCondition condition);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="guid"></param>
    /// <param name="condition"></param>
    /// <returns></returns>
    Task UpdateMaxAsync(Guid guid, SpaceshipCondition condition);
}
