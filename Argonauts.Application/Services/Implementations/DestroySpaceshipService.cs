using Argonauts.Application.Services.Abstractions;
using Argonauts.Core.Repository;

namespace Argonauts.Application.Services.Implementations;

/// <summary>
/// 
/// </summary>
public class DestroySpaceshipService(
    ISpaceshipRepository spaceshipRepository,
    ISpaceshipConditionService spaceshipConditionService,
    IBalanceService balanceService,
    IConsistencyService consistencyService
) : IDestroySpaceshipService
{
    private readonly ISpaceshipRepository _spaceshipRepository = spaceshipRepository
        ?? throw new ArgumentNullException(nameof(spaceshipRepository));
    private readonly ISpaceshipConditionService _spaceshipConditionService = spaceshipConditionService
        ?? throw new ArgumentNullException(nameof(spaceshipConditionService));
    private readonly IBalanceService _balanceService = balanceService
        ?? throw new ArgumentNullException(nameof(balanceService));
    private readonly IConsistencyService _consistencyService = consistencyService
        ?? throw new ArgumentNullException(nameof(consistencyService));

    /// <inheritdoc/>
    public async Task<bool> Destroy(Guid ownerId)
    {
        var spaceship = await _spaceshipRepository.GetAsync(ownerId);
        if (spaceship == null)
        {
            return false;
        }

        var conditions = await _spaceshipConditionService.GetForUserAsync(ownerId);
        if (conditions == null)
        {
            await _consistencyService.AddBalanceAndSpaceshipConditions(ownerId);
        }
        
        await _spaceshipConditionService.RestoreAsync(ownerId, checkRequirements: false);
        await _balanceService.ClearBalanceAsync(ownerId);
        await _spaceshipRepository.MoveAsync(ownerId, 0, 0);

        return true;
    }
}