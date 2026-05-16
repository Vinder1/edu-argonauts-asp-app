using Argonauts.Content;
using Argonauts.Core.Utility.Content;

namespace Argonauts.Web.Extensions;

/// <summary>
/// 
/// </summary>
public static class ContentConfiguration
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    public static void AddGameContent(this WebApplicationBuilder builder)
    {
        var dataLoader = new DataContainer();

        dataLoader.AddDataPack(new StandardDataPack());

        builder.Services.AddSingleton(dataLoader);
    }
}