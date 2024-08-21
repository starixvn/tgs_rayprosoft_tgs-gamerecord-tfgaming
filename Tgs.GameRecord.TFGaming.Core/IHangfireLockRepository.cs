namespace Tgs.GameRecord.TFGaming.Core;

public interface IHangfireLockRepository
{
	public Task<bool> LockingAsync(string application, CancellationToken cancellationToken = default);
}
