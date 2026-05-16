using Argonauts.Application.Dto;

namespace Argonauts.Web.Auth.Abstractions;

/// <summary>
/// Service for generating and managing JWT tokens.
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Generates a JWT token for the specified player.
    /// </summary>
    /// <param name="player">The player entity.</param>
    /// <returns>The generated JWT token string.</returns>
    string GenerateToken(UserDto player);
    
    /// <summary>
    /// Generates a JWT token for the specified player.
    /// </summary>
    /// <param name="player">The player entity.</param>
    /// <returns>The generated JWT token string.</returns>
    string GenerateRefreshToken(UserDto player);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    UserDto GetUserFromToken(string token);
}