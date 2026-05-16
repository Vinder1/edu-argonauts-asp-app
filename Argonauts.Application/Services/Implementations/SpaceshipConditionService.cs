using Argonauts.Application.Dto;
using Argonauts.Application.Services.Abstractions;
using Argonauts.Core.Entity.Player;
using Argonauts.Core.Repository;

namespace Argonauts.Application.Services.Implementations;

/// <summary>
/// Service implementation for managing spaceship condition operations.
/// </summary>
/// <param name="spaceshipConditionRepository">The spaceship condition repository.</param>
/// <param name="balanceService">The balance service.</param>
public class SpaceshipConditionService(
    ISpaceshipConditionRepository spaceshipConditionRepository,
    IBalanceService balanceService
) : ISpaceshipConditionService
{
    private readonly ISpaceshipConditionRepository _spaceshipConditionRepository = spaceshipConditionRepository
        ?? throw new ArgumentNullException(nameof(spaceshipConditionRepository));
    private readonly IBalanceService _balanceService = balanceService
        ?? throw new ArgumentNullException(nameof(balanceService));

    /// <inheritdoc/>
    public Task<SpaceshipCondition?> GetForUserAsync(Guid user)
    {
        return _spaceshipConditionRepository.GetForUserAsync(user);
    }

    /// <inheritdoc/>
    public async Task<ServiceActionResult> RestoreAsync(Guid user, bool checkRequirements = true)
    {
        if (checkRequirements)
        {
            // long check for correct location here
        }

        var balance = await _balanceService.GetForUserAsync(user);
        if (balance == null)
        {
            return ServiceActionResult.Invalid("Balance not found");
        }
        if (balance.Currency < 10)
        {
            return ServiceActionResult.Invalid("Not enough currency. Need at least 10.");
        }

        await _balanceService.AddCurrencyAsync(user, -10);
        await _spaceshipConditionRepository.RestoreAsync(user);
        return ServiceActionResult.Ok();
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(Guid id, SpaceshipCondition condition)
    {
        await _spaceshipConditionRepository.UpdateAsync(id, condition);
    }
    
    /// <inheritdoc/>
    public async Task UpdateMaxAsync(Guid id, SpaceshipCondition condition)
    {
        await _spaceshipConditionRepository.UpdateMaxAsync(id, condition);
    }
}
