using Argonauts.Application.Services.Abstractions;
using Argonauts.Core.Repository;

namespace Argonauts.Application.Services.Implementations;

/// <summary>
/// 
/// </summary>
public class ConsistencyService(IBalanceRepository balanceRepository, ISpaceshipConditionRepository spaceshipConditionRepository) : IConsistencyService
{
    private readonly IBalanceRepository _balanceRepository = balanceRepository
        ?? throw new ArgumentNullException(nameof(balanceRepository));
    private readonly ISpaceshipConditionRepository _spaceshipConditionRepository = spaceshipConditionRepository
        ?? throw new ArgumentNullException(nameof(spaceshipConditionRepository));
    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task AddBalanceAndSpaceshipConditions(Guid user)
    {
        await _balanceRepository.CreateIfMissing(user, saveManually: false);
        await _spaceshipConditionRepository.CreateIfMissing(user, saveManually: true);
    }
}