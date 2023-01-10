using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using Tgs.GameInterface.TFGamingService.Protos;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Common.MessageModels;
using Tgs.MessageQueue;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.TFGamingService.Core.ApplicationServices;

internal class GamePullRecordSettledCommandService : ICommandService<GamePullRecordSettledCommand>
{
    private readonly GameRecordInfoOptions _gameRecordInfo;
    private readonly ILogger<GamePullRecordSettledCommandService> _logger;
    private readonly IMessageSender _messageSender;
    private readonly MongoSettingOptions _mongoSetting;
    private readonly IUtensil _utensil;

    public GamePullRecordSettledCommandService(
        ILogger<GamePullRecordSettledCommandService> logger,
        IOptions<GameRecordInfoOptions> gameRecordInfoOptions,
        IOptions<MongoSettingOptions> mongoSettingOptions,
        IMessageSender messageSender,
        IUtensil utensil)
    {
        _gameRecordInfo = gameRecordInfoOptions.Value;
        _mongoSetting = mongoSettingOptions.Value;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
        _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
    }

    public async ValueTask ExecuteAsync(GamePullRecordSettledCommand command, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _utensil
            .GetGameProviderIdAsync(
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var gameId = await _utensil
            .GetGameIdAsync(
                gameProviderId: gameProviderId,
                gameExternalId: _gameRecordInfo.GameExternalId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        foreach (var record in command.Records)
        {
            bool success = true;
            var playerModel = await _utensil
                .GetPlayerAsync(
                    playerCode: record.PlayerCode,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            if (playerModel == null)
            {
                success = false;
            }

            Guid? gameProviderConfigurationId = default;
            if (playerModel != null)
            {
                gameProviderConfigurationId = await _utensil
                    .GetBrandGameProviderConfigurationIdAsync(
                        brandId: playerModel.BrandId,
                        gameProviderId: gameProviderId,
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
                if (!gameProviderConfigurationId.HasValue)
                {
                    success = false;
                }
            }

            GameRoundClientStatus clientStatus =
                record.Status switch
                {
                    GameHistoryStatus.Draw => GameRoundClientStatus.Draw,
                    GameHistoryStatus.Cancelled => GameRoundClientStatus.Void,
                    GameHistoryStatus.Win => GameRoundClientStatus.Win,
                    GameHistoryStatus.Loss => GameRoundClientStatus.Loss,
                    _ => (record.PlayerWinLoss > 0) ? GameRoundClientStatus.Win : GameRoundClientStatus.Loss
                };

            var message = new GameRecordMessage(
                ExternalRoundId: record.ExternalRoundId,
                GameProviderId: gameProviderId,
                GameProviderCode: _gameRecordInfo.GameProviderCode,
                GameId: gameId,
                ExternalGameId: _gameRecordInfo.GameExternalId,
                PlayerCode: (playerModel == null) ? string.Empty : playerModel.PlayerCode,
                CurrencyCode: (playerModel == null) ? string.Empty : playerModel.DefaultWalletCurrencyCode,
                WalletId: (playerModel == null) ? Guid.Empty : playerModel.DefaultWalletId,
                BrandId: (playerModel == null) ? Guid.Empty : playerModel.BrandId,
                GameProviderConfigurationId: gameProviderConfigurationId ?? Guid.Empty,
                Status: GameRoundStatus.Closed,
                ClientStatus: clientStatus,
                ExternalCreatedOn: record.ExternalCreatedOn.ToDateTimeOffset(),
                ExternalClosedOn: record.ExternalClosedOn.ToDateTimeOffset(),
                BetAmount: record.Bet.ToDecimal().GetValueOrDefault(0),
                ValidBetAmount: record.ValidBet.ToDecimal().GetValueOrDefault(0),
                SettleAmount: record.Settle,
                Commission: 0,
                Turnover: record.Turnover,
                PlayerWinLoss: record.PlayerWinLoss,
                AdditionalData: record.AdditionalData,
                Success: success);

            _logger.LogInformation("LogOn:{logOn}  Subject:{subject} MessageRequest:{message}",
                DateTime.Now.ToString("o"),
                _mongoSetting.GlobPattern,
                message);

            await _messageSender
                .PublishAsync(
                    subject: _mongoSetting.GlobPattern,
                    data: message.ToBson(),
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
