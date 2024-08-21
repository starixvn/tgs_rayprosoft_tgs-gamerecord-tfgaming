using Tgs.GameRecord.TFGaming.Core.JobApplicationServices;

namespace Tgs.GameRecord.TFGaming.Core;

public interface IHangfireUtensilRepository
{
	public Task JobCheckAsync();

	public Task JobExecuteAsync(JobExecuteCommand source);

	public Task JobInitAsync();
}
