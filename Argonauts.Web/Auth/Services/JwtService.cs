using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Argonauts.Application.Dto;
using Argonauts.Web.Auth.Abstractions;
using Argonauts.Web.Auth.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Argonauts.Web.Auth.Services;

/// <summary>
/// 
/// </summary>
/// <param name="options"></param>
public class JwtService(IOptions<JwtSettings> options) : IJwtService
{
    private readonly JwtSettings _settings = options.Value;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public string GenerateToken(UserDto user) =>
        GenerateTokenWithLifetime(user, DateTime.UtcNow.AddMinutes(_settings.TokenLifetimeMinutes));

    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public string GenerateRefreshToken(UserDto user) =>
        GenerateTokenWithLifetime(user, DateTime.UtcNow.AddDays(_settings.RefreshTokenLifetimeDays));

    private string GenerateTokenWithLifetime(UserDto user, DateTime time)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Login),
            new(ClaimTypes.Role, user.Role),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: time,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="SecurityTokenException"></exception>
    /// <exception cref="SecurityTokenExpiredException"></exception>
    /// <exception cref="SecurityTokenInvalidSignatureException"></exception>
    /// <exception cref="SecurityTokenInvalidIssuerException"></exception>
    /// <exception cref="SecurityTokenInvalidAudienceException"></exception>
    public UserDto GetUserFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_settings.Key);

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _settings.Issuer,
            ValidAudience = _settings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            if (validatedToken is not JwtSecurityToken jwtToken ||
                !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token algorithm or format");
            }

            var claims = principal.Claims.ToList();

            return new UserDto
            {
                Id = Guid.Parse(GetClaimValue(claims, ClaimTypes.NameIdentifier)),
                Login = GetClaimValue(claims, ClaimTypes.Name),
                Role = GetClaimValue(claims, ClaimTypes.Role),
            };
        }
        catch (SecurityTokenExpiredException ex)
        {
            throw new SecurityTokenExpiredException("Token has expired", ex);
        }
        catch (SecurityTokenInvalidSignatureException ex)
        {
            throw new SecurityTokenInvalidSignatureException("Invalid token signature", ex);
        }
        catch (SecurityTokenInvalidIssuerException ex)
        {
            throw new SecurityTokenInvalidIssuerException("Invalid token issuer", ex);
        }
        catch (SecurityTokenInvalidAudienceException ex)
        {
            throw new SecurityTokenInvalidAudienceException("Invalid token audience", ex);
        }
        catch (FormatException ex) when (ex.InnerException is SecurityTokenException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new SecurityTokenException("Failed to validate or parse token", ex);
        }
    }

    private static string GetClaimValue(List<Claim> claims, string claimType) => 
        claims.FirstOrDefault(c => c.Type == claimType)?.Value
        ?? throw new SecurityTokenException($"Claim '{claimType}' not found");
}