namespace Tgs.GameRecord.TFGaming.Core.JobApplicationServices;

public record JobExecuteCommand(
    Uri ApiUrl,
    string PrivateToken);
