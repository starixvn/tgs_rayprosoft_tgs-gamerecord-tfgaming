using System.Text.Json.Nodes;
using Microsoft.Extensions.Options;
using Tgs.GameInterface.TFGamingService.Protos;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.TFGaming.Core.ApplicationServices;

internal class HandleRecordSettledCommandService(
    IOptions<GameRecordInfoOptions> gameRecordInfoOptions,
    IGameRecordHandleClient client) : ICommandService<HandleRecordSettledCommand>
{
    private readonly GameRecordInfoOptions _gameRecordInfo = gameRecordInfoOptions.Value;

    public async ValueTask ExecuteAsync(HandleRecordSettledCommand command, CancellationToken cancellationToken = default)
    {
        var externalCreatedOn = command.Record.ExternalCreatedOn.ToDateTimeOffset();
        var externalClosedOn = command.Record.ExternalClosedOn.ToDateTimeOffset();
        var bet = command.Record.Bet.ToDecimal().GetValueOrDefault(0);
        var validBet = command.Record.ValidBet.ToDecimal().GetValueOrDefault(0);
        var settle = command.Record.Settle.ToDecimal().GetValueOrDefault(0);
        var commission = 0.0m;
        var turnover = command.Record.Turnover.ToDecimal().GetValueOrDefault(0);
        var winLoss = command.Record.PlayerWinLoss.ToDecimal().GetValueOrDefault(0);
        var wagerState = command.Record.Status switch
        {
            GameHistoryStatus.Draw => WagerStates.Draw,
            GameHistoryStatus.Cancelled => WagerStates.Refund,
            GameHistoryStatus.Win => WagerStates.Win,
            GameHistoryStatus.Loss => WagerStates.Lose,
            _ => (winLoss > 0) ? WagerStates.Win : WagerStates.Lose
        };

        await client
            .MessageAsync(
            gameProviderCode: _gameRecordInfo.GameProviderCode,
            playerCode: command.Record.PlayerCode,
            wagerState: wagerState,
            wagerType: WagerTypes.Normal,
            freeGameState: FreeGameStates.None,
            gambleState: GambleStates.None,
            cashoutState: CashoutStates.None,
            resettleState: ResettleStates.None,
            externalRoundId: command.Record.ExternalRoundId,
            externalWagerId: command.Record.ExternalRoundId,
            externalGameId: _gameRecordInfo.GameExternalId,
            externalCreatedOn: externalCreatedOn,
            externalClosedOn: externalClosedOn,
            externalPayoffOn: externalClosedOn,
            externalResettleOn: DateTimeOffset.UnixEpoch,
            bet: bet,
            validBet: validBet,
            settle: settle,
            commission: commission,
            turnover: turnover,
            winLoss: winLoss,
            additionalData: JsonNode.Parse(command.Record.AdditionalData)!,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}
