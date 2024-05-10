using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.TFGaming.Core.ApplicationServices;

internal class HandleRecordUnsettledCommandService(
	ILogger<HandleRecordUnsettledCommandService> logger,
	IOptions<GameRecordInfoOptions> options,
	IMessageSender messageSender) : ICommandService<HandleRecordUnsettledCommand>
{
	private readonly GameRecordInfoOptions _gameRecordInfo = options.Value;

	public async ValueTask ExecuteAsync(HandleRecordUnsettledCommand command, CancellationToken cancellationToken = default)
	{
		var externalCreatedOn = command.Record.ExternalCreatedOn.ToDateTimeOffset();
		var bet = command.Record.Bet.ToDecimal().GetValueOrDefault(0);
		var validBet = command.Record.ValidBet.ToDecimal().GetValueOrDefault(0);

		var request = new Handle.Common.Protos.GameRecordHandleRequest
		{
			GameProviderCode = _gameRecordInfo.GameProviderCode,
			PlayerCode = command.Record.PlayerCode,
			ExternalRoundId = command.Record.ExternalRoundId,
			ExternalBetId = command.Record.ExternalRoundId,
			ExternalGameId = _gameRecordInfo.GameExternalId,
			ExternalCreatedOn = externalCreatedOn.ToTimestamp(),
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
			Bet = bet,
			ValidBet = validBet,
			Settle = 0.0M,
			Commission = 0.0m,
			Turnover = 0.0M,
			WinLoss = 0.0M,
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
