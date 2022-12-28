namespace Tgs.GameRecord.TFGamingService.Core.WorkerServices;

public record GameHistoryWorkerCommand(
Uri ApiUrl,
string PrivateToken,
DateTime StartTime,
DateTime EndTime);
