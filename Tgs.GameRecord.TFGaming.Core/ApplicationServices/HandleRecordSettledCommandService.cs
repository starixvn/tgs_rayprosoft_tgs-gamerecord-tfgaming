using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tgs.GameInterface.TFGamingService.Protos;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.TFGaming.Core.ApplicationServices;

internal class HandleRecordSettledCommandService(
	ILogger<HandleRecordSettledCommandService> logger,
	IOptions<GameRecordInfoOptions> options,
	IMessageSender messageSender) : ICommandService<HandleRecordSettledCommand>
{
	private readonly GameRecordInfoOptions _gameRecordInfo = options.Value;

	public async ValueTask ExecuteAsync(HandleRecordSettledCommand command, CancellationToken cancellationToken = default)
	{
		var externalCreatedOn = command.Record.ExternalCreatedOn.ToDateTimeOffset();
		var externalClosedOn = command.Record.ExternalClosedOn.ToDateTimeOffset();
		var bet = command.Record.Bet.ToDecimal().GetValueOrDefault(0);
		var validBet = command.Record.ValidBet.ToDecimal().GetValueOrDefault(0);
		var settle = command.Record.Settle.ToDecimal().GetValueOrDefault(0);
		var turnover = command.Record.Turnover.ToDecimal().GetValueOrDefault(0);
		var winLoss = command.Record.PlayerWinLoss.ToDecimal().GetValueOrDefault(0);
		var betState = command.Record.Status switch
		{
			GameHistoryStatus.Draw => Handle.Common.Protos.BetState.Draw,
			GameHistoryStatus.Cancelled => Handle.Common.Protos.BetState.Cancel,
			GameHistoryStatus.Win => Handle.Common.Protos.BetState.Win,
			GameHistoryStatus.Loss => Handle.Common.Protos.BetState.Loss,
			_ => (winLoss > 0) ? Handle.Common.Protos.BetState.Win : Handle.Common.Protos.BetState.Loss
		};

		var request = new Handle.Common.Protos.GameRecordHandleRequest
		{
			GameProviderCode = _gameRecordInfo.GameProviderCode,
			PlayerCode = command.Record.PlayerCode,
			ExternalRoundId = command.Record.ExternalRoundId,
			ExternalBetId = command.Record.ExternalRoundId,
			ExternalGameId = _gameRecordInfo.GameExternalId,
			ExternalCreatedOn = externalCreatedOn.ToTimestamp(),
			ExternalClosedOn = externalClosedOn.ToTimestamp(),
			ExternalPayoffOn = externalClosedOn.ToTimestamp(),
			ExternalResettleOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
			BetState = betState,
			BetType = Handle.Common.Protos.BetType.Normal,
			FreeGameState = Handle.Common.Protos.FreeGameState.None,
			GambleState = Handle.Common.Protos.GambleState.None,
			CashoutState = Handle.Common.Protos.CashoutState.None,
			ResettleState = Handle.Common.Protos.ResettleState.None,
			JackpotState = Handle.Common.Protos.JackpotState.None,
			Bet = bet,
			ValidBet = validBet,
			Settle = settle,
			Commission = 0.0m,
			Turnover = turnover,
			WinLoss = winLoss,
			Jackpot = 0.0M,
			AdditionalData = command.Record.AdditionalData
		};

		await messageSender.PublishAsync(
			subject: Handle.Common.GameRecordHandleNatsConst.GameRecordHandleRequest,
			data: request,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		logger.LogInformation("LogOn:{logOn} Subject:{subject} Mesaage:This publication requests information.({request})",
			DateTime.Now.ToString("o"),
			Handle.Common.GameRecordHandleNatsConst.GameRecordHandleRequest,
			new
			{
				request.GameProviderCode,
				request.PlayerCode,
				request.ExternalRoundId,
				request.ExternalBetId,
				request.ExternalGameId,
				ExternalCreatedOn = request.ExternalCreatedOn.ToDateTimeOffset(),
				ExternalClosedOn = request.ExternalClosedOn.ToDateTimeOffset(),
				request.BetState
			});
	}
}
