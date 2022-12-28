namespace Tgs.GameRecord.TFGamingService.Core.DispatchServices;

public record ScheduleDispatchCommand(
    Uri ApiUrl,
    string PrivateToken);
