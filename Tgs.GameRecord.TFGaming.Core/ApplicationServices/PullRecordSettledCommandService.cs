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

            logger.LogInformation("LogOn:{logOn} | Action:GameHistorySettledAsync | Response:{response}",
                DateTime.UtcNow.ToString("O"),
                JsonSerializer.Serialize(gameHistorySettleds));

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
                    logger.LogError(ex, "LogOn:{logOn} Message: An exception occurred in this game record.({gameRecord})", DateTime.Now.ToString("o"), gameHistorySettled);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "LogOn:{logOn} | Action:GameHistorySettledAsync | Message:TFGaming處理結算過程中發生例外", DateTime.UtcNow.ToString("O"));
        }
    }
}
