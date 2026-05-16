namespace Argonauts.Application.Services.Abstractions;

/// <summary>
/// 
/// </summary>
public interface IDestroySpaceshipService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ownerId"></param>
    /// <returns></returns>
    Task<bool> Destroy(Guid ownerId);
}