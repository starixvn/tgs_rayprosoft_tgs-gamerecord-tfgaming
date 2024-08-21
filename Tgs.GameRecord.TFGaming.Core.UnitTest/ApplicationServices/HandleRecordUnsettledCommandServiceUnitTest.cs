using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Tgs.GameInterface.TFGamingService.Protos;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.TFGaming.Core.ApplicationServices;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.TFGaming.Core.UnitTest.ApplicationServices;

public class HandleRecordUnsettledCommandServiceUnitTest
{
	[Fact]
	public async Task Normal()
	{
		var faceLogger = NullLogger<HandleRecordUnsettledCommandService>.Instance;
		var fakeGameRecordInfoOptions = Options.Create(new GameRecordInfoOptions
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
		var fakePolicySettingsOptions = Options.Create(new PolicySettingsOptions
		{
			RetryCount = 1,
			RetrySeconds = 1,
		});
		var fakeGameRecordHandleClient = Substitute.For<IGameRecordHandleClient>();

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

		var sut = new HandleRecordUnsettledCommandService(
			faceLogger,
			fakeGameRecordInfoOptions,
			fakePolicySettingsOptions,
			fakeGameRecordHandleClient);

		await sut.ExecuteAsync(command);

		_ = fakeGameRecordHandleClient
			.Received(1)
			.HandleAsync(
			source: Arg.Is(new Handle.Common.ViewModels.GameRecordHandleViewModel
			{
				GameProviderCode = fakeGameRecordInfoOptions.Value.GameProviderCode,
				PlayerCode = command.Record.PlayerCode,
				ExternalRoundId = command.Record.ExternalRoundId,
				ExternalBetId = command.Record.ExternalRoundId,
				ExternalGameId = fakeGameRecordInfoOptions.Value.GameExternalId,
				ExternalCreatedOn = command.Record.ExternalCreatedOn.ToDateTimeOffset(),
				ExternalClosedOn = DateTimeOffset.UnixEpoch,
				ExternalPayoffOn = DateTimeOffset.UnixEpoch,
				ExternalResettleOn = DateTimeOffset.UnixEpoch,
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
