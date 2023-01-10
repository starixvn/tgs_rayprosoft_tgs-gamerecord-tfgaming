namespace Tgs.GameRecord.TFGamingManualService.ViewModels;

public record GameRecordQueryViewModel(
    Uri ApiUrl,
    string PrivateToken,
    DateTime StartTime,
    DateTime EndTime);
