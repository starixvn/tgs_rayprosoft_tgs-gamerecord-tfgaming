namespace Tgs.GameRecord.TFGamingService.ViewModels;

public record GameRecordQueryViewModel(
    Uri ApiUrl,
    string PrivateToken,
    DateTime StartTime,
    DateTime EndTime);
