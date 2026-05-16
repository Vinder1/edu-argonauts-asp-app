using Argonauts.Core.Repository;
using Argonauts.Infrastructure.Database.Repository;
using Argonauts.Infrastructure.Database.Repository.DataSources;
using Microsoft.EntityFrameworkCore;

namespace Argonauts.Web.Extensions;

/// <summary>
/// 
/// </summary>
public static class RepositoryConfiguration
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    public static void AddRepositories(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        
        services.AddDbContext<GameDbContext>(options =>
        {
            options.UseNpgsql(
                builder.Configuration.GetConnectionString("GameDb"));
        });

        services.AddScoped<IPlayerRepository, EfPlayerRepository>();
        services.AddScoped<ISpaceshipRepository, EfSpaceshipRepository>();
        services.AddScoped<IGalaxyRepository, EfGalaxyRepository>();
        services.AddScoped<IStarVisitRepository, EfStarVisitRepository>();
        services.AddScoped<IBalanceRepository, EfBalanceRepository>();
        services.AddScoped<ISpaceshipConditionRepository, EfSpaceshipConditionRepository>();

        services.AddScoped<INamedSpaceshipRepository, EfNamedSpaceshipRepository>();
        services.AddScoped<IExplorationStatusRepository, CacheExplorationStatusRepository>();
        services.AddScoped<IMovementStatusRepository, CacheMovementStatusRepository>();
        services.AddScoped<IBattleStatusRepository, CacheBattleStatusRepository>();
        services.AddScoped<IMessageRepository, CacheMessageRepository>();
        services.AddScoped<IQuestRepository, EfQuestRepository>();
        services.AddScoped<ICacheService, ValkeyCacheService>();
    }
}