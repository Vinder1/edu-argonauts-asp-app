using Microsoft.AspNetCore.HttpLogging;
using Serilog;
using Serilog.Events;

namespace Argonauts.Web.Extensions;

/// <summary>
/// 
/// </summary>
public static class HttpLoggingConfiguration
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    public static void ConfigureHttpLogging(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpLogging(logging =>
        {
            logging.LoggingFields = HttpLoggingFields.All;
            logging.RequestBodyLogLimit = 4096;
            logging.ResponseBodyLogLimit = 4096;
        });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="app"></param>
    public static void UseCustomSerilogAndHttpLogging(this WebApplication app)
    {
        app.UseWhen(
            predicate: ctx => ctx.Request.Path.StartsWithSegments("/api"),
            configuration: app =>
            {

                app.UseSerilogRequestLogging(opt =>
                {
                    opt.GetLevel = (ctx, _, ex) =>
                        ex != null || ctx.Response.StatusCode >= 500 ? LogEventLevel.Error :
                        LogEventLevel.Information;

                    opt.EnrichDiagnosticContext = (dContext, httpContext) =>
                    {
                        if (httpContext.Items.TryGetValue("CorrelationId", out var id))
                            dContext.Set("CorrelationId", id);
                    };
                });

                //app.UseHttpLogging();
            });
    }
}