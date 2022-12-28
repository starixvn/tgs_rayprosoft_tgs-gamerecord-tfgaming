using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Common.MessageModels;
using Tgs.MessageQueue;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.TFGamingService.Core.ApplicationServices;

internal class GamePullRecordUnsettledCommandService : ICommandService<GamePullRecordUnsettledCommand>
{
    private readonly GameRecordInfoOptions _gameRecordInfo;
    private readonly ILogger<GamePullRecordUnsettledCommandService> _logger;
    private readonly IMessageSender _messageSender;
    private readonly IUtensil _utensil;

    public GamePullRecordUnsettledCommandService(
        ILogger<GamePullRecordUnsettledCommandService> logger,
        IOptions<GameRecordInfoOptions> options,
        IMessageSender messageSender,
        IUtensil utensil)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _gameRecordInfo = options.Value;
        _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
        _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
    }

    public async ValueTask ExecuteAsync(GamePullRecordUnsettledCommand command, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _utensil.GetGameProviderIdAsync().ConfigureAwait(false);
        var gameId = await _utensil.GetGameIdAsync(gameProviderId, _gameRecordInfo.GameExternalId).ConfigureAwait(false);
        foreach (var record in command.Records)
        {
            bool success = true;
            var playerModel = await _utensil.GetPlayerAsync(record.PlayerCode).ConfigureAwait(false);

            if (playerModel == null)
            {
                success = false;
            }

            Guid? gameProviderConfigurationId = default;
            if (playerModel != null)
            {
                gameProviderConfigurationId = await _utensil.BrandGetGameProviderConfigurationIdAsync(playerModel.BrandId, gameProviderId).ConfigureAwait(false);
                if (!gameProviderConfigurationId.HasValue)
                {
                    success = false;
                }
            }

            var data = new GameRecordMessage(
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
                Status: GameRoundStatus.Open,
                ClientStatus: GameRoundClientStatus.Open,
                ExternalCreatedOn: record.ExternalCreatedOn.ToDateTimeOffset(),
                ExternalClosedOn: null,
                BetAmount: record.Bet.ToDecimal().GetValueOrDefault(0),
                ValidBetAmount: record.ValidBet.ToDecimal().GetValueOrDefault(0),
                SettleAmount: 0,
                Commission: 0,
                Turnover: 0,
                PlayerWinLoss: 0,
                AdditionalData: record.AdditionalData,
                Success: success);

            var subject = _gameRecordInfo.MongoGlobPattern;

            _logger.LogInformation("LogOn:{logOn}  Subject:{subject} MessageRequest:{request}", DateTime.Now.ToString("o"), subject, data);

            await _messageSender
                    .PublishAsync(
                        subject: subject,
                        data: data.ToBson(),
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
        }
    }
}
