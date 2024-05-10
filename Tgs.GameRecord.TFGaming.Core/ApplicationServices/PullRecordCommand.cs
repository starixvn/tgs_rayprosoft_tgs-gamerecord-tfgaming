namespace Tgs.GameRecord.TFGaming.Core.ApplicationServices;

public record PullRecordCommand(
	Uri ApiUrl,
	string PrivateToken,
	DateTimeOffset StartTime,
	DateTimeOffset EndTime);
