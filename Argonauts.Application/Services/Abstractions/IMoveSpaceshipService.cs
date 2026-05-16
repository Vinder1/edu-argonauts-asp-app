using Argonauts.Application.Dto;
using Argonauts.Core.Entity.Galaxy;
using Argonauts.Core.Entity.Movement;

namespace Argonauts.Application.Services.Abstractions;

/// <summary>
/// 
/// </summary>
public interface IMoveSpaceshipService
{
    /// <summary>
    /// Moves a spaceship to a new star.
    /// </summary>
    /// <param name="ownerId">The owner's unique ID.</param>
    /// <param name="newStar">The destination star.</param>
    /// <returns>The updated spaceship entity.</returns>
    Task<ServiceActionResultWithBody<ArrivalInfo>> MoveSpaceshipAsync(Guid ownerId, Star newStar);
    /// <summary>
    /// Gets the current movement status for a player.
    /// </summary>
    /// <param name="playerId">The player's unique ID.</param>
    /// <returns>The movement status if exists, null otherwise.</returns>
    Task<MovementStatus?> GetStatusAsync(Guid playerId);
}