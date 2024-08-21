using System.Text.Json;
using Microsoft.Extensions.Logging;
using Tgs.GameInterface.TFGamingService.Client;
using Tgs.GameInterface.TFGamingService.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.TFGaming.Core.ApplicationServices;

internal class PullRecordUnsettledCommandService(
	ILogger<PullRecordUnsettledCommandService> logger,
	IGameInterfaceTFGamingClient client,
	ICommandService<HandleRecordUnsettledCommand> service) : ICommandService<PullRecordUnsettledCommand>
{
	public async ValueTask ExecuteAsync(PullRecordUnsettledCommand command, CancellationToken cancellationToken = default)
	{
		try
		{
			var gameHistoryUnsettleds = await client.GameHistoryUnsettledAsync(
				apiUrl: command.ApiUrl,
				privateToken: command.PrivateToken,
				startTime: command.StartTime.UtcDateTime,
				endTime: command.EndTime.UtcDateTime,
				pageSize: 1000,
				cancellationToken: cancellationToken)
				.ToArrayAsync(cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			logger.LogInformation("{logInformation}",
				JsonSerializer.Serialize(new
				{
					LogOn = DateTime.Now.ToString("u"),
					Nameof = nameof(PullRecordUnsettledCommandService),
					Req = new
					{
						command.ApiUrl,
						command.PrivateToken,
						StartTime = command.StartTime.UtcDateTime.ToString("u"),
						EndTime = command.EndTime.UtcDateTime.ToString("u")
					},
					Resp = gameHistoryUnsettleds
				}));

			foreach (var gameHistoryUnsettled in gameHistoryUnsettleds)
			{
				try
				{
					await service.ExecuteAsync(
						command: new HandleRecordUnsettledCommand(gameHistoryUnsettled),
						cancellationToken: cancellationToken)
						.ConfigureAwait(false);
				}
				catch (Exception ex)
				{
					logger.LogError(ex, "{logError}",
						JsonSerializer.Serialize(new
						{
							LogOn = DateTime.Now.ToString("u"),
							Nameof = nameof(PullRecordUnsettledCommandService),
							GameRecord = gameHistoryUnsettled,
							Message = "An exception occurred in this game record"
						}));
				}
			}
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "{logError}",
				JsonSerializer.Serialize(new
				{
					LogOn = DateTime.Now.ToString("u"),
					Nameof = nameof(PullRecordUnsettledCommandService),
					Req = new
					{
						command.ApiUrl,
						command.PrivateToken,
						StartTime = command.StartTime.UtcDateTime.ToString("u"),
						EndTime = command.EndTime.UtcDateTime.ToString("u")
					},
					Message = "An exception occurred while retrieving game records"
				}));
		}
	}
}
