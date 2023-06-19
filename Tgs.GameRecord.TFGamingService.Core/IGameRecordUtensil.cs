using Tgs.GameInterface.TFGamingService.Protos;
using Tgs.GameRecord.TFGamingService.Core.Models;

namespace Tgs.GameRecord.TFGamingService.Core;

public interface IGameRecordUtensil
{
    public ValueTask<RecordInfo> SettleConvertRecordAsync(
        GameHistorySettled source,
        CancellationToken cancellationToken = default);

    public ValueTask<RecordInfo> UnsettleConvertRecordAsync(
        GameHistoryUnsettled source,
        CancellationToken cancellationToken = default);
}
