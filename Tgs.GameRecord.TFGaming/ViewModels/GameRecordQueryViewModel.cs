namespace Tgs.GameRecord.TFGaming.ViewModels;

public record GameRecordQueryViewModel(
    Uri ApiUrl,
    string PrivateToken,
    DateTime StartTime,
    DateTime EndTime);
