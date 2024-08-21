using System.Text.Json;
using Microsoft.Extensions.Logging;
using Tgs.GameInterface.TFGamingService.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.TFGaming.Core.ApplicationServices;

internal class PullRecordSettledCommandService(
	ILogger<PullRecordSettledCommandService> logger,
	IGameInterfaceTFGamingClient client,
	ICommandService<HandleRecordSettledCommand> service) : ICommandService<PullRecordSettledCommand>
{
	public async ValueTask ExecuteAsync(PullRecordSettledCommand command, CancellationToken cancellationToken = default)
	{
		try
		{
			var gameHistorySettleds = await client
				.GameHistorySettledAsync(
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
					Nameof = nameof(PullRecordSettledCommandService),
					Req = new
					{
						command.ApiUrl,
						command.PrivateToken,
						StartTime = command.StartTime.UtcDateTime.ToString("u"),
						EndTime = command.EndTime.UtcDateTime.ToString("u")
					},
					Resp = gameHistorySettleds
				}));

			foreach (var gameHistorySettled in gameHistorySettleds)
			{
				try
				{
					await service.ExecuteAsync(
						command: new HandleRecordSettledCommand(gameHistorySettled),
						cancellationToken: cancellationToken)
						.ConfigureAwait(false);
				}
				catch (Exception ex)
				{
					logger.LogError(ex, "{logError}",
						JsonSerializer.Serialize(new
						{
							LogOn = DateTime.Now.ToString("u"),
							Nameof = nameof(PullRecordSettledCommandService),
							GameRecord = gameHistorySettled,
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
					Nameof = nameof(PullRecordSettledCommandService),
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
