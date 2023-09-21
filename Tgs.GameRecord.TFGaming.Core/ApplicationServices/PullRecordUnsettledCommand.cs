using Tgs.GameInterface.TFGamingService.Protos;

namespace Tgs.GameRecord.TFGaming.Core.ApplicationServices;

public record PullRecordUnsettledCommand(IEnumerable<GameHistoryUnsettled> Records);
