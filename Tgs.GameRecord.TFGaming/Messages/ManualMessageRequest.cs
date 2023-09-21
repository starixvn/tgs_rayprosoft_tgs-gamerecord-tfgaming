namespace Tgs.GameRecord.TFGaming.Messages;

public record ManualMessageRequest(
    Uri ApiUrl,
    string PrivateToken,
    DateTime StartTime,
    DateTime EndTime);
