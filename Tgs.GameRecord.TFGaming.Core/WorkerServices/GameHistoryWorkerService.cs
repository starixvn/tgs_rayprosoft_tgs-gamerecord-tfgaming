using System.Text.Json;
using Microsoft.Extensions.Logging;
using Tgs.GameInterface.TFGamingService.Client;
using Tgs.GameRecord.TFGaming.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.TFGaming.Core.WorkerServices;

internal class GameHistoryWorkerService : ICommandService<GameHistoryWorkerCommand>
{
    private readonly IGameInterfaceTFGamingClient _client;
    private readonly ILogger<GameHistoryWorkerService> _logger;
    private readonly ICommandService<PullRecordSettledCommand> _settledService;
    private readonly ICommandService<PullRecordUnsettledCommand> _unsettledService;

    public GameHistoryWorkerService(
        IGameInterfaceTFGamingClient client,
        ILogger<GameHistoryWorkerService> logger,
        ICommandService<PullRecordUnsettledCommand> unsettledService,
        ICommandService<PullRecordSettledCommand> settledService)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _unsettledService = unsettledService ?? throw new ArgumentNullException(nameof(unsettledService));
        _settledService = settledService ?? throw new ArgumentNullException(nameof(settledService));
    }

    public async ValueTask ExecuteAsync(GameHistoryWorkerCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var gameHistoryUnsettleds = await _client
                .GameHistoryUnsettledAsync(
                    apiUrl: command.ApiUrl,
                    privateToken: command.PrivateToken,
                    startTime: command.StartTime.ToUniversalTime(),
                    endTime: command.EndTime.ToUniversalTime(),
                    pageSize: 1000,
                    cancellationToken: cancellationToken)
                .ToArrayAsync(cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            _logger.LogInformation("LogOn:{logOn} | Action:GameHistoryUnsettledAsync | Response:{response}",
                DateTime.UtcNow.ToString("O"),
                JsonSerializer.Serialize(gameHistoryUnsettleds));

            await _unsettledService
                .ExecuteAsync(
                    command: new PullRecordUnsettledCommand(
                        Records: gameHistoryUnsettleds),
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LogOn:{logOn} | Action:GameHistoryUnsettledAsync | Message:TFGaming處理未結算過程中發生例外", DateTime.UtcNow.ToString("O"));
        }

        try
        {
            var gameHistorySettleds = await _client
                    .GameHistorySettledAsync(
                        apiUrl: command.ApiUrl,
                        privateToken: command.PrivateToken,
                        startTime: command.StartTime.ToUniversalTime(),
                        endTime: command.EndTime.ToUniversalTime(),
                        pageSize: 1000,
                        cancellationToken: cancellationToken)
                    .ToArrayAsync(cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

            _logger.LogInformation("LogOn:{logOn} | Action:GameHistorySettledAsync | Response:{response}",
                DateTime.UtcNow.ToString("O"),
                JsonSerializer.Serialize(gameHistorySettleds));

            await _settledService
                .ExecuteAsync(
                    command: new PullRecordSettledCommand(
                        Records: gameHistorySettleds),
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LogOn:{logOn} | Action:GameHistorySettledAsync | Message:TFGaming處理結算過程中發生例外", DateTime.UtcNow.ToString("O"));
        }
    }
}
