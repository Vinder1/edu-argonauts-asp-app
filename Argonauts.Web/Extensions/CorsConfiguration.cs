namespace Argonauts.Web.Extensions;
/// <summary>
/// 
/// </summary>
public static class CorsConfiguration
{
    /// <summary>
    /// 
    /// </summary>
    public static void AddFrontendCors(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("VueCors", policy =>
            {
                policy.WithOrigins("http://localhost:5173")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
    }
}