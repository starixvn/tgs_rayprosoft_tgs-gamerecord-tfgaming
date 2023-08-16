using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Options;
using NSubstitute;
using Tgs.GameInterface.TFGamingService.Protos;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.GameRecord.TFGamingService.Core.ApplicationServices;

namespace Tgs.GameRecord.TFGamingService.Core.UnitTest.ApplicationServices;

public class HandleRecordUnsettledCommandServiceUnitTest
{
    [Fact]
    public async void Normal()
    {
        var gameRecordInfo = Options.Create(new GameRecordInfoOptions
        {
            GameProviderCode = "TFTF",
            IntervalMinutes = 3,
            IntervalMinutesMax = 1440,
            IntervalMinutesMinimum = 2,
            LockMinute = 2
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

        await service
            .ExecuteAsync(
            command)
            .ConfigureAwait(false);

        await fakeClient
            .Received(1)
            .MessageAsync(
            gameProviderCode: Arg.Is(gameRecordInfo.Value.GameProviderCode),
            externalRoundId: Arg.Is(command.Record.ExternalRoundId),
            externalGameId: Arg.Is(gameRecordInfo.Value.GameExternalId),
            playerCode: Arg.Is(command.Record.PlayerCode),
            status: Arg.Is(GameRecordStatus.Open),
            clientStatus: Arg.Is(GameRecordClientStatus.Open),
            externalCreatedOn: Arg.Is(command.Record.ExternalCreatedOn.ToDateTimeOffset()),
            externalClosedOn: Arg.Is((DateTimeOffset?)null),
            bet: Arg.Is(218.00m),
            validBet: Arg.Is(218.00m),
            settle: Arg.Is(0.0m),
            commission: Arg.Is(0.0m),
            turnover: Arg.Is(0.0m),
            playerWinLoss: Arg.Is(0.0m),
            additionalData: Arg.Is(command.Record.AdditionalData),
            cancellationToken: Arg.Any<CancellationToken>())
            .ConfigureAwait(false);
    }
}
