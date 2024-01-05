using System.Text.Json.Nodes;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Options;
using NSubstitute;
using Tgs.GameInterface.TFGamingService.Protos;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.GameRecord.TFGaming.Core.ApplicationServices;

namespace Tgs.GameRecord.TFGaming.Core.UnitTest.ApplicationServices;

public class HandleRecordUnsettledCommandServiceUnitTest
{
    [Fact]
    public async void Normal()
    {
        var gameRecordInfo = Options.Create(new GameRecordInfoOptions
        {
            GameProviderCode = "TFTF",
            GameExternalId = "tf_esports",
            GameProviderConfiguration = "TFTFConfiguration",
            HangfirePrefix = "gamerecord-tftf-hangfire:",
            HangfireLocking = "gamerecord-tftf-locking:",
            IntervalMinutes = 15,
            IntervalMinutesMax = 1440,
            IntervalMinutesMinimum = 2,
            LockMinute = 13
        });
        var fakeClient = Substitute.For<IGameRecordHandleClient>();

        var command = new HandleRecordUnsettledCommand(
            Record: new GameHistoryUnsettled
            {
                PlayerCode = "4ukbau4ts4xz",
                ExternalRoundId = "CDUBF33756745W0234PLS4340",
                ExternalCreatedOn = DateTimeOffset.Parse("2023-08-15T19:55:00Z").ToTimestamp(),
                Bet = 218.00m,
                ValidBet = 218.00m,
                AdditionalData = "{\"id\":\"CDUBF33756745W0234PLS4340\"}"
            });

        var service = new HandleRecordUnsettledCommandService(
            gameRecordInfo,
            fakeClient);

        await service.ExecuteAsync(command);

        await fakeClient
            .Received(1)
            .MessageAsync(
            gameProviderCode: Arg.Is(gameRecordInfo.Value.GameProviderCode),
            playerCode: Arg.Is(command.Record.PlayerCode),
            wagerState: Arg.Is(WagerStates.Running),
            wagerType: Arg.Is(WagerTypes.Normal),
            freeGameState: Arg.Is(FreeGameStates.None),
            gambleState: Arg.Is(GambleStates.None),
            cashoutState: Arg.Is(CashoutStates.None),
            resettleState: Arg.Is(ResettleStates.None),
            externalRoundId: Arg.Is(command.Record.ExternalRoundId),
            externalWagerId: Arg.Is(command.Record.ExternalRoundId),
            externalGameId: Arg.Is("tf_esports"),
            externalCreatedOn: Arg.Is(command.Record.ExternalCreatedOn.ToDateTimeOffset()),
            externalClosedOn: Arg.Is(DateTimeOffset.UnixEpoch),
            externalPayoffOn: Arg.Is(DateTimeOffset.UnixEpoch),
            externalResettleOn: Arg.Is(DateTimeOffset.UnixEpoch),
            bet: Arg.Is(218.00m),
            validBet: Arg.Is(218.00m),
            settle: Arg.Is(0.0m),
            commission: Arg.Is(0.0m),
            turnover: Arg.Is(0.0m),
            winLoss: Arg.Is(0.0m),
            additionalData: Arg.Any<JsonNode>(),
            cancellationToken: Arg.Any<CancellationToken>());
    }
}
