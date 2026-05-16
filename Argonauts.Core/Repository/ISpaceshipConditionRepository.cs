using Argonauts.Core.Entity.Player;

namespace Argonauts.Core.Repository;

/// <summary>
/// Repository for spaceship condition-related data operations.
/// </summary>
public interface ISpaceshipConditionRepository
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <param name="saveManually"></param>
    /// <returns></returns>
    public Task CreateIfMissing(Guid user, bool saveManually = true);

    /// <summary>
    /// Gets the spaceship condition for a specific user.
    /// </summary>
    /// <param name="user">The user's unique ID.</param>
    /// <returns>The spaceship condition.</returns>
    Task<SpaceshipCondition?> GetForUserAsync(Guid user);

    /// <summary>
    /// Sets Max values for Energy, Durability and Antimatter  
    /// </summary>
    /// <param name="user">The user's unique ID.</param>
    /// <returns>The spaceship condition.</returns>
    Task RestoreAsync(Guid user);
    /// <summary>
    /// Only updates Energy, Durability, Antimatter
    /// </summary>
    /// <param name="id"></param>
    /// <param name="condition"></param>
    /// <returns></returns>
    Task UpdateAsync(Guid id, SpaceshipCondition condition);
    /// <summary>
    /// Only updates MaxEnergy, MaxDurability, MaxAntimatter
    /// </summary>
    /// <param name="id"></param>
    /// <param name="condition"></param>
    /// <returns></returns>
    Task UpdateMaxAsync(Guid id, SpaceshipCondition condition);
}
