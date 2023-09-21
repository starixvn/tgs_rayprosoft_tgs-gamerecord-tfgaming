namespace Tgs.GameRecord.TFGaming.Core.DispatchServices;

public record ScheduleDispatchCommand(
    Uri ApiUrl,
    string PrivateToken);
