namespace Argonauts.Application.Dto;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public class ServiceActionResultWithBody<T> : ServiceActionResult
{
    /// <summary>
    /// 
    /// </summary>
    public required T? Body { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static ServiceActionResultWithBody<T> Ok(T body) => new()
    {
        Success = true,
        Body = body
    };

    /// <summary>
    /// 
    /// </summary>
    /// <param name="error"></param>
    /// <returns></returns>
    public static new ServiceActionResultWithBody<T> Invalid(string error) => new()
    {
        ErrorDescription = error,
        Success = false,
        Body = default
    };
}