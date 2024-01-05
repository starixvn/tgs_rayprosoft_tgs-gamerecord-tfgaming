namespace Tgs.GameRecord.TFGaming.Core.ApplicationServices;

public record PullRecordSettledCommand(
    Uri ApiUrl,
    string PrivateToken,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime);
