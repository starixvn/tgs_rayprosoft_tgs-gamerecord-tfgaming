using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Tgs.GameInterface.TFGamingService.Protos;
using Tgs.GameRecord.TFGaming.Core.ApplicationServices;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.TFGaming.Core.UnitTest.ApplicationServices;

public class HandleRecordUnsettledCommandServiceUnitTest
{
	[Fact]
	public async Task Normal()
	{
		var faceLogger = NullLogger<HandleRecordUnsettledCommandService>.Instance;
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
		var fakeMessageSender = Substitute.For<IMessageSender>();

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
			faceLogger,
			gameRecordInfo,
			fakeMessageSender);

		await service.ExecuteAsync(command);

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(Handle.Common.GameRecordHandleNatsConst.GameRecordHandleRequest),
			data: Arg.Is(new Handle.Common.Protos.GameRecordHandleRequest
			{
				GameProviderCode = gameRecordInfo.Value.GameProviderCode,
				PlayerCode = command.Record.PlayerCode,
				ExternalRoundId = command.Record.ExternalRoundId,
				ExternalBetId = command.Record.ExternalRoundId,
				ExternalGameId = gameRecordInfo.Value.GameExternalId,
				ExternalCreatedOn = command.Record.ExternalCreatedOn,
				ExternalClosedOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
				ExternalPayoffOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
				ExternalResettleOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
				BetState = Handle.Common.Protos.BetState.Running,
				BetType = Handle.Common.Protos.BetType.Normal,
				FreeGameState = Handle.Common.Protos.FreeGameState.None,
				GambleState = Handle.Common.Protos.GambleState.None,
				CashoutState = Handle.Common.Protos.CashoutState.None,
				ResettleState = Handle.Common.Protos.ResettleState.None,
				JackpotState = Handle.Common.Protos.JackpotState.None,
				Bet = 218.00m,
				ValidBet = 218.00m,
				Settle = 0.0M,
				Commission = 0.0M,
				Turnover = 0.0M,
				WinLoss = 0.0M,
				Jackpot = 0.0M,
				AdditionalData = command.Record.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
