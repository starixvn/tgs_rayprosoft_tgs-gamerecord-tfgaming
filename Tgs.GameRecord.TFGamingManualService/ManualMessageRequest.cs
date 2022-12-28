namespace Tgs.GameRecord.TFGamingManualService;

public record ManualMessageRequest(
    Uri ApiUrl,
    string PrivateToken,
    DateTime StartTime,
    DateTime EndTime);
