using Argonauts.Infrastructure.Database.Mapping;
using Argonauts.Web.Mapping;

namespace Argonauts.Web.Extensions;

/// <summary>
/// 
/// </summary>
public static class AutoMapperConfiguration
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    public static void ConfigureAutoMapper(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<AppToDbMapper>();
        builder.Services.AddSingleton<AppToEndpointsMapper>();
    }
}