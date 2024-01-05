using System.Text.Json.Nodes;
using Microsoft.Extensions.Options;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.TFGaming.Core.ApplicationServices;

internal class HandleRecordUnsettledCommandService(
    IOptions<GameRecordInfoOptions> gameRecordInfoOptions,
    IGameRecordHandleClient client) : ICommandService<HandleRecordUnsettledCommand>
{
    private readonly GameRecordInfoOptions _gameRecordInfo = gameRecordInfoOptions.Value;

    public async ValueTask ExecuteAsync(HandleRecordUnsettledCommand command, CancellationToken cancellationToken = default)
    {
        var externalCreatedOn = command.Record.ExternalCreatedOn.ToDateTimeOffset();
        var bet = command.Record.Bet.ToDecimal().GetValueOrDefault(0);
        var validBet = command.Record.ValidBet.ToDecimal().GetValueOrDefault(0);

        await client
            .MessageAsync(
            gameProviderCode: _gameRecordInfo.GameProviderCode,
            playerCode: command.Record.PlayerCode,
            wagerState: WagerStates.Running,
            wagerType: WagerTypes.Normal,
            freeGameState: FreeGameStates.None,
            gambleState: GambleStates.None,
            cashoutState: CashoutStates.None,
            resettleState: ResettleStates.None,
            externalRoundId: command.Record.ExternalRoundId,
            externalWagerId: command.Record.ExternalRoundId,
            externalGameId: _gameRecordInfo.GameExternalId,
            externalCreatedOn: externalCreatedOn,
            externalClosedOn: DateTimeOffset.UnixEpoch,
            externalPayoffOn: DateTimeOffset.UnixEpoch,
            externalResettleOn: DateTimeOffset.UnixEpoch,
            bet: bet,
            validBet: validBet,
            settle: 0.0m,
            commission: 0.0m,
            turnover: 0.0m,
            winLoss: 0.0m,
            additionalData: JsonNode.Parse(command.Record.AdditionalData)!,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}
