using Argonauts.Web.Auth.Configuration;

namespace Argonauts.Web.Extensions;

/// <summary>
/// 
/// </summary>
public static class OptionsConfiguration
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    public static void AddOptions(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
        builder.Services.Configure<ResendSettings>(builder.Configuration.GetSection("Resend"));
    }
}