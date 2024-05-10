using System.Text.Json.Nodes;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Tgs.GameInterface.TFGamingService.Protos;
using Tgs.GameRecord.TFGaming.Core.ApplicationServices;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.TFGaming.Core.UnitTest.ApplicationServices;

public class HandleRecordSettledCommandServiceUnitTest
{
	[Fact]
	public async Task Loss()
	{
		var faceLogger = NullLogger<HandleRecordSettledCommandService>.Instance;
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

		var command = new HandleRecordSettledCommand(
			Record: new GameHistorySettled
			{
				PlayerCode = "4ukbau4ts4xz",
				ExternalRoundId = "CDUBF33756745W0234PLS4340",
				Status = GameHistoryStatus.Loss,
				ExternalCreatedOn = DateTimeOffset.Parse("2023-08-15T19:55:00Z").ToTimestamp(),
				ExternalClosedOn = DateTimeOffset.Parse("2023-08-15T22:44:58.319438Z").ToTimestamp(),
				Bet = 218.00m,
				ValidBet = 218.00m,
				Settle = 0.0m,
				Turnover = 218.00m,
				PlayerWinLoss = -218.00m,
				AdditionalData = "{\"id\":\"CDUBF33756745W0234PLS4340\",\"bet_selection\":\"away\",\"odds\":-0.7,\"currency\":\"HKD\",\"amount\":218,\"game_type_name\":\"League of Legends\",\"game_market_name\":\"MAP 3 WINNER (MAPS)\",\"market_option\":\"map\",\"map_num\":\"MAP 3\",\"bet_type_name\":\"SPWINMAP\",\"competition_name\":\"2023 Campeonato Brasileiro de League of Legends Academy Split 2\",\"event_id\":25044986,\"event_name\":\"KaBuM! Academy vs RED Academy\",\"event_datetime\":\"2023-08-15T19:55:00Z\",\"date_created\":\"2023-08-15T22:02:34.343958Z\",\"settlement_datetime\":\"2023-08-15T22:44:58.319438Z\",\"modified_datetime\":\"2023-08-15T22:45:00.070824Z\",\"settlement_status\":\"settled\",\"result\":\"home\",\"result_status\":\"LOSS\",\"earnings\":-218,\"handicap\":null,\"is_combo\":false,\"member_code\":\"4ukbau4ts4xz\",\"is_unsettled\":false,\"ticket_type\":\"live\",\"malay_odds\":-0.7,\"euro_odds\":2.42,\"member_odds\":1.42,\"member_odds_style\":\"hongkong\",\"game_type_id\":3,\"request_source\":\"mobile-browser\"}"
			});

		var service = new HandleRecordSettledCommandService(
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
				ExternalClosedOn = command.Record.ExternalClosedOn,
				ExternalPayoffOn = command.Record.ExternalClosedOn,
				ExternalResettleOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
				BetState = Handle.Common.Protos.BetState.Loss,
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
				Turnover = 218.00m,
				WinLoss = -218.00m,
				Jackpot = 0.0M,
				AdditionalData = command.Record.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Win()
	{
		var faceLogger = NullLogger<HandleRecordSettledCommandService>.Instance;
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

		var command = new HandleRecordSettledCommand(
			Record: new GameHistorySettled
			{
				PlayerCode = "4ukbau4ts4xz",
				ExternalRoundId = "CDUBE34993342I5001BOT4525",
				Status = GameHistoryStatus.Win,
				ExternalCreatedOn = DateTimeOffset.Parse("2023-08-14T08:50:01.798746Z").ToTimestamp(),
				ExternalClosedOn = DateTimeOffset.Parse("2023-08-14T09:06:41.269454Z").ToTimestamp(),
				Bet = 200.00m,
				ValidBet = 200.00m,
				Settle = 338.00m,
				Turnover = 200.00m,
				PlayerWinLoss = 138.00m,
				AdditionalData = "{\"id\":\"CDUBE34993342I5001BOT4525\",\"bet_selection\":\"home\",\"odds\":0.69,\"currency\":\"HKD\",\"amount\":200,\"game_type_name\":\"CS:GO\",\"game_market_name\":\"MAP 1 ROUND HANDICAP (INCL. OVERTIME)\",\"market_option\":\"map\",\"map_num\":\"MAP 1\",\"bet_type_name\":\"AH\",\"competition_name\":\"2023 CCT Central Europe Series #7\",\"event_id\":25015184,\"event_name\":\"Looking4Org vs B8\",\"event_datetime\":\"2023-08-14T08:00:00Z\",\"date_created\":\"2023-08-14T08:50:01.798746Z\",\"settlement_datetime\":\"2023-08-14T09:06:41.269454Z\",\"modified_datetime\":\"2023-08-14T09:06:45.134534Z\",\"settlement_status\":\"settled\",\"result\":\"16:11\",\"result_status\":\"WIN\",\"earnings\":338,\"handicap\":\"-4.5\",\"is_combo\":false,\"member_code\":\"4ukk96lpkb0a\",\"is_unsettled\":false,\"ticket_type\":\"live\",\"malay_odds\":0.69,\"euro_odds\":1.69,\"member_odds\":0.69,\"member_odds_style\":\"hongkong\",\"game_type_id\":1,\"request_source\":\"desktop-browser\"}"
			});

		var service = new HandleRecordSettledCommandService(
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
				ExternalClosedOn = command.Record.ExternalClosedOn,
				ExternalPayoffOn = command.Record.ExternalClosedOn,
				ExternalResettleOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
				BetState = Handle.Common.Protos.BetState.Win,
				BetType = Handle.Common.Protos.BetType.Normal,
				FreeGameState = Handle.Common.Protos.FreeGameState.None,
				GambleState = Handle.Common.Protos.GambleState.None,
				CashoutState = Handle.Common.Protos.CashoutState.None,
				ResettleState = Handle.Common.Protos.ResettleState.None,
				JackpotState = Handle.Common.Protos.JackpotState.None,
				Bet = 200.00m,
				ValidBet = 200.00m,
				Settle = 338.00m,
				Commission = 0.0M,
				Turnover = 200.00m,
				WinLoss = 138.00m,
				Jackpot = 0.0M,
				AdditionalData = command.Record.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
