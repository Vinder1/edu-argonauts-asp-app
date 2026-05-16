using Argonauts.Application.External;
using Argonauts.Application.Services.Abstractions;
using Argonauts.Application.Services.Implementations;
using Argonauts.Core.Entity.Player;
using Argonauts.Web.Auth.Abstractions;
using Argonauts.Web.Auth.Services;
using Argonauts.Web.Services;
using Argonauts.Web.Services.Abstractions;
using Microsoft.AspNetCore.Identity;
using Resend;

namespace Argonauts.Web.Extensions;

/// <summary>
/// 
/// </summary>
public static class ServiceConfiguration
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    public static void AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IPlayerService, PlayerService>();
        builder.Services.AddScoped<ISpaceshipService, SpaceshipService>();
        builder.Services.AddScoped<IGalaxyService, GalaxyService>();
        builder.Services.AddScoped<IStarVisitService, StarVisitService>();
        builder.Services.AddScoped<IBalanceService, BalanceService>();
        builder.Services.AddScoped<ISpaceshipConditionService, SpaceshipConditionService>();
        builder.Services.AddScoped<IConsistencyService, ConsistencyService>();
        builder.Services.AddScoped<IMoveSpaceshipService, MoveSpaceshipService>();
        builder.Services.AddScoped<IExplorationService, ExplorationService>();
        builder.Services.AddScoped<IBattleEntityService, BattleEntityService>();
        builder.Services.AddScoped<IBattleProcessService, BattleProcessService>();
        builder.Services.AddScoped<ISpaceshipStateRepository, CacheSpaceshipStateRepository>();
        builder.Services.AddScoped<IDestroySpaceshipService, DestroySpaceshipService>();
        builder.Services.AddScoped<ISpaceshipOnStarService, SpaceshipOnStarService>();
        builder.Services.AddScoped<ISpaceshipUpgradeService, SpaceshipUpgradeService>();
        builder.Services.AddScoped<IChatService, ChatService>();
        builder.Services.AddScoped<IGuideService, GuideService>();
        builder.Services.AddScoped<IQuestService, QuestService>();
        
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IJwtService, JwtService>();
        builder.Services.AddScoped<ICookieService, CookieService>();

        builder.Services.AddScoped<IEmailService, EmailService>();
        builder.Services.AddHttpClient<ResendClient>();
        builder.Services.Configure<ResendClientOptions>(o =>
        {
            var resendSettings = builder.Configuration.GetSection("Resend").Get<Auth.Configuration.ResendSettings>();
            o.ApiToken = resendSettings?.ApiKey ?? throw new InvalidOperationException("Resend API key is not configured");
        });
        builder.Services.AddTransient<IResend, ResendClient>();

        builder.Services.AddScoped<IServerEventService, ServerEventService>();

        builder.Services.AddSingleton<CorrelationIdMiddleware>();

        builder.Services.AddSignalR();

        builder.Services.AddSingleton<IPasswordHasher<Player>, PasswordHasher<Player>>();

        builder.Services.AddHttpClient<IObscenityFilterService, ObscenityFilterService>(client =>
        {
            var baseUrl = builder.Configuration.GetValue<string>("ObscenityFilter:BaseUrl")
                ?? "http://localhost:5073";
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(5);
        });
    }
}