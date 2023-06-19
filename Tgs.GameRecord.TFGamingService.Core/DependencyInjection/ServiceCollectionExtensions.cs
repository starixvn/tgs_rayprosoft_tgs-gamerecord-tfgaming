using System.Reflection;
using Tgs.GameCommand.Client;
using Tgs.GameInterface.TFGamingService.Client;
using Tgs.GameRecord.TFGamingService.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameRecordCore(
        this IServiceCollection services,
        Action<RedisOptions, IServiceProvider> redisDBconfigure,
        Action<GameRecordInfoOptions, IServiceProvider> gameRecordInfoOptions,
        Action<GameInterfaceTFGamingClientOptions, IServiceProvider> gameInterfaceTFGamingClientOptions,
        Action<GameCommandClientOptions, IServiceProvider> gameCommandClientOptions)
        => services
            .AddOptions<RedisOptions>()
            .Configure(redisDBconfigure)
            .Services
            .AddOptions<GameRecordInfoOptions>()
            .Configure(gameRecordInfoOptions)
            .Services
            .RegisterAllServiceUtilityTypes(
                Assembly.GetExecutingAssembly(),
                ServiceLifetime.Transient)
            .AddMemoryCache()
            .AddGameRoundsWorkClient()
            .AddSingleton<RedisConnectionManager>()
            .AddGameInterfaceTFGamingServiceClient(gameInterfaceTFGamingClientOptions)
            .AddGameCommandClient(gameCommandClientOptions)
            .AddTransient<IScheduleDispatch>(sp => ActivatorUtilities.CreateInstance<ScheduleDispatch>(sp, sp.GetRequiredService<RedisConnectionManager>().Database))
            .AddTransient<IUtensil>(sp => ActivatorUtilities.CreateInstance<UtensilRepository>(sp, TimeSpan.FromHours(1)))
            .AddSingleton<IGameRecordUtensil, GameRecordUtensil>();
}
