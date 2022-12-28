using Tgs.GameInterface.TFGamingService.Protos;

namespace Tgs.GameRecord.TFGamingService.Core.ApplicationServices;

public record GamePullRecordUnsettledCommand(IEnumerable<GameHistoryUnsettled> Records);
