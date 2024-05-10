using Tgs.ServiceUtility;

namespace Tgs.GameRecord.TFGaming.Core.ApplicationServices;

internal class PullRecordCommandService(
	ICommandService<PullRecordUnsettledCommand> unsettledService,
	ICommandService<PullRecordSettledCommand> settledService) : ICommandService<PullRecordCommand>
{
	public async ValueTask ExecuteAsync(PullRecordCommand command, CancellationToken cancellationToken = default)
	{
		await unsettledService.ExecuteAsync(
			command: new PullRecordUnsettledCommand(
				ApiUrl: command.ApiUrl,
				PrivateToken: command.PrivateToken,
				StartTime: command.StartTime,
				EndTime: command.EndTime),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		await settledService.ExecuteAsync(
			command: new PullRecordSettledCommand(
				ApiUrl: command.ApiUrl,
				PrivateToken: command.PrivateToken,
				StartTime: command.StartTime,
				EndTime: command.EndTime),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
