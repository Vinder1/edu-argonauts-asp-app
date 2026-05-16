using Valkey.Glide;
using static Valkey.Glide.ConnectionConfiguration;

namespace Argonauts.Web.Extensions;

/// <summary>
/// 
/// </summary>
public static class ValkeyConfiguration
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static void AddValkeyCache(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("valkey")
            ?? throw new InvalidOperationException("Connection string for Valkey not found");

        var connectionData = new Dictionary<string, string>(connectionString.Split(";").Select(s =>
        {
            var args = s.Split("=");
            return new KeyValuePair<string, string>(args[0], args[1]);
        }));

        var config = new StandaloneClientConfigurationBuilder()
            .WithAddress(connectionData["Host"], ushort.Parse(connectionData["Port"]))
            .WithRequestTimeout(TimeSpan.FromSeconds(5))
            .WithConnectionTimeout(TimeSpan.FromSeconds(10))
            .WithCredentials(new ServerCredentials(connectionData["password"]))
            .WithTls(false)
            .Build();

        builder.Services.AddSingleton(config);
        // builder.Services.AddSingleton(sp => GlideClient.CreateClient(config).GetAwaiter().GetResult());
    }
}