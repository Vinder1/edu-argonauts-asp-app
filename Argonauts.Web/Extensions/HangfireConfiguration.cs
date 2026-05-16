using Argonauts.Application.External;
using Argonauts.Infrastructure.Hangfire;
using Hangfire;
using Hangfire.Redis.StackExchange;

namespace Argonauts.Web.Extensions;

/// <summary>
/// 
/// </summary>
public static class HangfireConfiguration
{
    /// <summary>
    /// 
    /// </summary>
    public static void AddFastHangfire(this WebApplicationBuilder builder)
    {
        var hangfireConn = builder.Configuration.GetConnectionString("HangfireRedis")
            ?? throw new InvalidOperationException("Hangfire connection string missing");

        // // I will leave it here for future 
        // // maybe I would want to use Postgres as a message storage again
        // builder.Services.AddHangfire(config =>
        // {
        //     config.UsePostgreSqlStorage(hangfireConn, new PostgreSqlStorageOptions
        //     {
        //         SchemaName = "hangfire",
        //         PrepareSchemaIfNecessary = true,
        //         UseNativeDatabaseTransactions = true,
        //         QueuePollInterval = TimeSpan.FromMilliseconds(500),
        //         InvisibilityTimeout = TimeSpan.FromMinutes(3),
        //         CountersAggregateInterval = TimeSpan.FromMinutes(5),
        //     });
        // });

        builder.Services.AddTransient<IBackgroundScheduler, HangfireScheduler>();

        builder.Services.AddHangfire(config =>
        {
            config.UseRedisStorage(hangfireConn, new RedisStorageOptions
            {
                Db = 0,
                Prefix = "hangfire:",
                InvisibilityTimeout = TimeSpan.FromMinutes(3),
                FetchTimeout = TimeSpan.FromSeconds(2),
            });
        });

        builder.Services.AddHangfireServer(op =>
        {
            op.WorkerCount = Math.Clamp(Environment.ProcessorCount, 1, 4);
            op.HeartbeatInterval = TimeSpan.FromSeconds(15);
            op.ServerCheckInterval = TimeSpan.FromMinutes(1);
            op.SchedulePollingInterval = TimeSpan.FromSeconds(3);
            op.ServerTimeout = TimeSpan.FromMinutes(5);
            op.ShutdownTimeout = TimeSpan.FromSeconds(30);
            op.Queues = ["default"];
        });
    }
}