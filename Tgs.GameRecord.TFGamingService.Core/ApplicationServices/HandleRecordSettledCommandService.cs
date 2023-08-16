using Microsoft.Extensions.Options;
using Tgs.GameInterface.TFGamingService.Protos;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.TFGamingService.Core.ApplicationServices;

internal class HandleRecordSettledCommandService : ICommandService<HandleRecordSettledCommand>
{
    private readonly IGameRecordHandleClient _client;
    private readonly GameRecordInfoOptions _gameRecordInfo;

    public HandleRecordSettledCommandService(
        IOptions<GameRecordInfoOptions> gameRecordInfoOptions,
        IGameRecordHandleClient client)
    {
        _gameRecordInfo = gameRecordInfoOptions.Value;
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask ExecuteAsync(HandleRecordSettledCommand command, CancellationToken cancellationToken = default)
    {
        var status = GameRecordStatus.Closed;
        var externalCreatedOn = command.Record.ExternalCreatedOn.ToDateTimeOffset();
        var externalClosedOn = command.Record.ExternalClosedOn.ToDateTimeOffset();
        var bet = command.Record.Bet.ToDecimal().GetValueOrDefault(0);
        var validBet = command.Record.ValidBet.ToDecimal().GetValueOrDefault(0);
        var settle = command.Record.Settle.ToDecimal().GetValueOrDefault(0);
        var commission = 0.0m;
        var turnover = command.Record.Turnover.ToDecimal().GetValueOrDefault(0);
        var playerWinLoss = command.Record.PlayerWinLoss.ToDecimal().GetValueOrDefault(0);
        var clientStatus = command.Record.Status switch
        {
            GameHistoryStatus.Draw => GameRecordClientStatus.Draw,
            GameHistoryStatus.Cancelled => GameRecordClientStatus.Void,
            GameHistoryStatus.Win => GameRecordClientStatus.Win,
            GameHistoryStatus.Loss => GameRecordClientStatus.Loss,
            _ => (playerWinLoss > 0) ? GameRecordClientStatus.Win : GameRecordClientStatus.Loss
        };

        await _client
            .MessageAsync(
            gameProviderCode: _gameRecordInfo.GameProviderCode,
            externalRoundId: command.Record.ExternalRoundId,
            externalGameId: _gameRecordInfo.GameExternalId,
            playerCode: command.Record.PlayerCode,
            status: status,
            clientStatus: clientStatus,
            externalCreatedOn: externalCreatedOn,
            externalClosedOn: externalClosedOn,
            bet: bet,
            validBet: validBet,
            settle: settle,
            commission: commission,
            turnover: turnover,
            playerWinLoss: playerWinLoss,
            additionalData: command.Record.AdditionalData,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}
