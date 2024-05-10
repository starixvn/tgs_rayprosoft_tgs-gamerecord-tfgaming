using Tgs.GameRecord.TFGaming.Core.JobApplicationServices;

namespace Tgs.GameRecord.TFGaming.Core;

public interface IHangfireUtensil
{
	public Task JobCheckAsync();

	public Task JobExecuteAsync(JobExecuteCommand source);

	public Task JobInitAsync();
}
