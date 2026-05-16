namespace Argonauts.Application.Services.Abstractions;

/// <summary>
/// 
/// </summary>
public interface IConsistencyService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    Task AddBalanceAndSpaceshipConditions(Guid user);
}