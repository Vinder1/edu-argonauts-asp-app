using Argonauts.Application.Dto;

namespace Argonauts.Web.Auth.Abstractions;

/// <summary>
/// 
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Create new player
    /// </summary>
    /// <param name="name"></param>
    /// <param name="login"></param>
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <returns> Jwt token for user </returns>
    Task<UserDto> SignInNewPlayerAsync(string name, string login, string email, string password);

    /// <summary>
    /// Find user
    /// </summary>
    /// <param name="login"></param>
    /// <param name="password"></param>
    /// <returns> Jwt token for user </returns>
    Task<UserDto?> LogInAsync(string login, string password);

    /// <summary>
    /// Find user
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <returns> Jwt token for user </returns>
    UserDto LogInWithRefreshTokenAsync(string refreshToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    string GetJwtTokenForUser(UserDto user);

    /// <summary>
    /// Creates cookies for this user
    /// </summary>
    /// <returns></returns>
    Task SetRefreshToken(UserDto user, HttpContext context);
}