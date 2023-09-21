namespace Tgs.GameRecord.TFGaming.Core.WorkerServices;

public record GameHistoryWorkerCommand(
Uri ApiUrl,
string PrivateToken,
DateTime StartTime,
DateTime EndTime);
