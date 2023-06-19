namespace Tgs.GameRecord.TFGamingService;

public record ManualMessageRequest(
    Uri ApiUrl,
    string PrivateToken,
    DateTime StartTime,
    DateTime EndTime);
