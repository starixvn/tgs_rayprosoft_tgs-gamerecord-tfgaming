using Tgs.GameInterface.TFGamingService.Protos;

namespace Tgs.GameRecord.TFGaming.Core.ApplicationServices;

public record PullRecordSettledCommand(IEnumerable<GameHistorySettled> Records);
