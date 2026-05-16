using Argonauts.Application.Dto;
using Argonauts.Core.Entity.Exploration;
using Argonauts.Core.Entity.Galaxy;

namespace Argonauts.Application.Services.Abstractions;

/// <summary>
/// Service for tracking spaceship visits to stars.
/// </summary>
public interface IStarVisitService
{
    /// <summary>
    /// Records a visit by a spaceship owner to a star he is placed.
    /// </summary>
    /// <param name="ownerId">The spaceship owner's unique ID.</param>
    Task<ServiceActionResult> Create(Guid ownerId);

    /// <summary>
    /// Retrieves the active visit record for an owner at a specific star.
    /// </summary>
    /// <param name="ownerId">The spaceship owner's unique ID.</param>
    /// <param name="star">The star entity.</param>
    /// <returns>The visit record or null if not found.</returns>
    Task<SpaceshipStarVisit?> GetActiveFor(Guid ownerId, Star star);
}