using Argonauts.Application.Dto;
using Argonauts.Web.Auth.Abstractions;
using Argonauts.Web.Auth.Configuration;
using Microsoft.Extensions.Options;

namespace Argonauts.Web.Auth.Services;

/// <summary>
/// 
/// </summary>
public class CookieService(IJwtService jwtService, IOptions<JwtSettings> options) : ICookieService
{
    private readonly IJwtService _jwtService = jwtService
        ?? throw new ArgumentNullException(nameof(jwtService));
    private readonly JwtSettings _settings = options.Value;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="response"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    public async Task SaveRefreshToken(HttpResponse response, UserDto user)
    {
        var options = new CookieOptions()
        {
            Expires = DateTime.UtcNow.AddDays(_settings.RefreshTokenLifetimeDays),
            HttpOnly = true
        };

        response.Cookies.Append("spaceApp_refreshToken", _jwtService.GenerateRefreshToken(user), options);
    }
}