using System.Text.Json;
using Microsoft.Extensions.Logging;
using Tgs.GameInterface.TFGamingService.Client;
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

            logger.LogInformation("LogOn:{logOn} | Action:GameHistoryUnsettledAsync | Response:{response}",
                DateTime.UtcNow.ToString("O"),
                JsonSerializer.Serialize(gameHistoryUnsettleds));

            foreach (var gameHistoryUnsettled in gameHistoryUnsettleds)
            {
                try
                {
                    await service
                        .ExecuteAsync(
                        command: new HandleRecordUnsettledCommand(gameHistoryUnsettled),
                         cancellationToken: cancellationToken)
                        .ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "LogOn:{logOn} Message: An exception occurred in this game record.({gameRecord})", DateTime.Now.ToString("o"), gameHistoryUnsettled);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "LogOn:{logOn} | Action:GameHistoryUnsettledAsync | Message:TFGaming處理未結算過程中發生例外", DateTime.UtcNow.ToString("O"));
        }
    }
}
