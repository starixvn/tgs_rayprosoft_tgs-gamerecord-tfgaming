namespace Tgs.GameRecord.TFGaming.Core.ApplicationServices;

public record PullRecordUnsettledCommand(
    Uri ApiUrl,
    string PrivateToken,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime);
