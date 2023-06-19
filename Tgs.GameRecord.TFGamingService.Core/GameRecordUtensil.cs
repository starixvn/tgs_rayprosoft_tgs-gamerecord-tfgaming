using Microsoft.Extensions.Options;
using Tgs.GameInterface.TFGamingService.Protos;
using Tgs.GameRecord.TFGamingService.Core.Models;
using Tgs.GameRounds.Work.Common.Protos;

namespace Tgs.GameRecord.TFGamingService.Core;

internal class GameRecordUtensil : IGameRecordUtensil
{
    private readonly GameRecordInfoOptions _gameRecordInfo;
    private readonly IUtensil _utensil;

    public GameRecordUtensil(
        IOptions<GameRecordInfoOptions> gameRecordInfoOptions,
        IUtensil utensil)
    {
        _gameRecordInfo = gameRecordInfoOptions.Value;
        _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
    }

    public async ValueTask<RecordInfo> SettleConvertRecordAsync(
        GameHistorySettled source,
        CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _utensil
            .GetGameProviderIdAsync(
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameProviderCode = _utensil
            .GetGameProviderCode();

        bool success = true;

        var player = await _utensil
            .GetPlayerAsync(
                playerCode: source.PlayerCode,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameId = await _utensil
            .GetGameIdAsync(
                gameProviderId: gameProviderId,
                gameExternalId: _gameRecordInfo.GameExternalId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (player == null)
        {
            success = false;
        }

        Guid? gameProviderConfigurationId = default;
        if (player != null)
        {
            gameProviderConfigurationId = await _utensil
                .GetBrandGameProviderConfigurationIdAsync(
                    brandId: player.BrandId,
                    gameProviderId: gameProviderId,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            if (!gameProviderConfigurationId.HasValue)
            {
                success = false;
            }
        }

        var clientStatus =
            source.Status switch
            {
                GameHistoryStatus.Draw => RoundClientStatus.Draw,
                GameHistoryStatus.Cancelled => RoundClientStatus.Void,
                GameHistoryStatus.Win => RoundClientStatus.Win,
                GameHistoryStatus.Loss => RoundClientStatus.Loss,
                _ => (source.PlayerWinLoss > 0) ? RoundClientStatus.Win : RoundClientStatus.Loss
            };

        var externalCreatedOn = source.ExternalCreatedOn.ToDateTimeOffset();
        var externalClosedOn = source.ExternalClosedOn.ToDateTimeOffset();
        var bet = source.Bet.ToDecimal().GetValueOrDefault(0);
        var validBet = source.ValidBet.ToDecimal().GetValueOrDefault(0);
        var settle = source.Settle.ToDecimal().GetValueOrDefault(0);
        var commission = 0;
        var turnover = source.Turnover.ToDecimal().GetValueOrDefault(0);
        var winLoss = source.PlayerWinLoss.ToDecimal().GetValueOrDefault(0);

        return new RecordInfo(
            Success: success,
            GameProviderId: gameProviderId,
            GameProviderCode: gameProviderCode,
            ExternalRoundId: source.ExternalRoundId,
            GameId: gameId,
            ExternalGameId: _gameRecordInfo.GameExternalId,
            PlayerCode: (player == null) ? string.Empty : player.PlayerCode,
            WalletId: (player == null) ? Guid.Empty : player.DefaultWalletId,
            CurrencyCode: (player == null) ? string.Empty : player.DefaultWalletCurrencyCode,
            BrandId: (player == null) ? Guid.Empty : player.BrandId,
            GameProviderConfigurationId: gameProviderConfigurationId ?? Guid.Empty,
            Status: RoundStatus.Closed,
            ClientStatus: clientStatus,
            ExternalCreatedOn: externalCreatedOn,
            ExternalClosedOn: externalClosedOn,
            Bet: bet,
            ValidBet: validBet,
            Settle: settle,
            Commission: commission,
            Turnover: turnover,
            WinLoss: winLoss,
            AdditionalData: source.AdditionalData);
    }

    public async ValueTask<RecordInfo> UnsettleConvertRecordAsync(
        GameHistoryUnsettled source,
        CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _utensil
            .GetGameProviderIdAsync(
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameProviderCode = _utensil
            .GetGameProviderCode();

        bool success = true;

        var player = await _utensil
            .GetPlayerAsync(
                playerCode: source.PlayerCode,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameId = await _utensil
            .GetGameIdAsync(
                gameProviderId: gameProviderId,
                gameExternalId: _gameRecordInfo.GameExternalId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (player == null)
        {
            success = false;
        }

        Guid? gameProviderConfigurationId = default;
        if (player != null)
        {
            gameProviderConfigurationId = await _utensil
                .GetBrandGameProviderConfigurationIdAsync(
                    brandId: player.BrandId,
                    gameProviderId: gameProviderId,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            if (!gameProviderConfigurationId.HasValue)
            {
                success = false;
            }
        }

        var externalCreatedOn = source.ExternalCreatedOn.ToDateTimeOffset();
        var bet = source.Bet.ToDecimal().GetValueOrDefault(0);
        var validBet = source.ValidBet.ToDecimal().GetValueOrDefault(0);

        return new RecordInfo(
            Success: success,
            GameProviderId: gameProviderId,
            GameProviderCode: gameProviderCode,
            ExternalRoundId: source.ExternalRoundId,
            GameId: gameId,
            ExternalGameId: _gameRecordInfo.GameExternalId,
            PlayerCode: (player == null) ? string.Empty : player.PlayerCode,
            WalletId: (player == null) ? Guid.Empty : player.DefaultWalletId,
            CurrencyCode: (player == null) ? string.Empty : player.DefaultWalletCurrencyCode,
            BrandId: (player == null) ? Guid.Empty : player.BrandId,
            GameProviderConfigurationId: gameProviderConfigurationId ?? Guid.Empty,
            Status: RoundStatus.Open,
            ClientStatus: RoundClientStatus.Open,
            ExternalCreatedOn: externalCreatedOn,
            ExternalClosedOn: null,
            Bet: bet,
            ValidBet: validBet,
            Settle: 0,
            Commission: 0,
            Turnover: 0,
            WinLoss: 0,
            AdditionalData: source.AdditionalData);
    }
}
