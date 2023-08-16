using Microsoft.Extensions.Options;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.TFGamingService.Core.ApplicationServices;

internal class HandleRecordUnsettledCommandService : ICommandService<HandleRecordUnsettledCommand>
{
    private readonly IGameRecordHandleClient _client;
    private readonly GameRecordInfoOptions _gameRecordInfo;

    public HandleRecordUnsettledCommandService(
        IOptions<GameRecordInfoOptions> gameRecordInfoOptions,
        IGameRecordHandleClient client)
    {
        _gameRecordInfo = gameRecordInfoOptions.Value;
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask ExecuteAsync(HandleRecordUnsettledCommand command, CancellationToken cancellationToken = default)
    {
        var status = GameRecordStatus.Open;
        var clientStatus = GameRecordClientStatus.Open;
        var externalCreatedOn = command.Record.ExternalCreatedOn.ToDateTimeOffset();
        var externalClosedOn = (DateTimeOffset?)null;
        var bet = command.Record.Bet.ToDecimal().GetValueOrDefault(0);
        var validBet = command.Record.ValidBet.ToDecimal().GetValueOrDefault(0);
        var settle = 0.0m;
        var commission = 0.0m;
        var turnover = 0.0m;
        var playerWinLoss = 0.0m;

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
