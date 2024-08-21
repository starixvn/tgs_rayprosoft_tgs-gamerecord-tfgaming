using Tgs.GameRecord.TFGaming.Core.ApplicationServices;

namespace Tgs.GameRecord.TFGaming.Core;

public interface IPullRecordDispatcher
{
	ValueTask PushCommandAsync(PullRecordCommand command, CancellationToken cancellationToken = default);
}
