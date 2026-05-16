using Destructurama;
using Serilog;

namespace Argonauts.Web.Extensions;

/// <summary>
/// 
/// </summary>
public static class LoggingConfiguration
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    public static void ConfigureLogger(this WebApplicationBuilder builder)
    {
#if DEBUG
        Serilog.Debugging.SelfLog.Enable(msg => Console.Error.WriteLine($"[Serilog SelfLog] {msg}"));
#endif

        builder.Host.UseSerilog((context, configuration) =>
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .Destructure.UsingAttributes());
    }
}