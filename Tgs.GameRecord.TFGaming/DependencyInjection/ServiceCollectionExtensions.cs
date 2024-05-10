using Tgs.GameCommand.Client;
using Tgs.GameInterface.TFGamingService.Client;
using Tgs.GameRecord.TFGaming;
using Tgs.GameRecord.TFGaming.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGameRecord(
		this IServiceCollection services,
		Action<RedisOptions, IServiceProvider> redisDBconfigure,
		Action<GameRecordInfoOptions, IServiceProvider> gameRecordInfoOptions,
		Action<GameCommandClientOptions, IServiceProvider> gameCommandClientOptions,
		Action<GameInterfaceTFGamingClientOptions, IServiceProvider> gameInterfaceTFGamingClientOptions)
		=> services
		.AddOptions<RedisOptions>()
		.Configure(redisDBconfigure)
		.Services
		.AddOptions<GameRecordInfoOptions>()
		.Configure(gameRecordInfoOptions)
		.Services
		.AddSingleton<RedisConnectionManager>()
		.AddGameRecordCore(
			gameCommandClientOptions: gameCommandClientOptions,
			gameInterfaceTFGamingClientOptions: gameInterfaceTFGamingClientOptions,
			redisDatabaseResolver: (sp) => sp.GetRequiredService<RedisConnectionManager>().Database);
}
