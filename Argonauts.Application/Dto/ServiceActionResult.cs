namespace Argonauts.Application.Dto;

/// <summary>
/// 
/// </summary>
public class ServiceActionResult
{
    /// <summary>
    /// 
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public string? ErrorDescription { get; init; }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static ServiceActionResult Ok() => new ()
    {
        Success = true
    };
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="error"></param>
    /// <returns></returns>
    public static ServiceActionResult Invalid(string error) => new ()
    {
        ErrorDescription = error,
        Success = false
    };
}