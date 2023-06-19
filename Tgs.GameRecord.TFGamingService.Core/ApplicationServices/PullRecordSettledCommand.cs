using Tgs.GameInterface.TFGamingService.Protos;

namespace Tgs.GameRecord.TFGamingService.Core.ApplicationServices;

public record PullRecordSettledCommand(IEnumerable<GameHistorySettled> Records);
