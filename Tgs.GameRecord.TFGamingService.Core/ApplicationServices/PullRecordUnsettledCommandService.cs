using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.TFGamingService.Core.ApplicationServices;

internal class PullRecordUnsettledCommandService : ICommandService<PullRecordUnsettledCommand>
{
    private readonly ILogger<PullRecordUnsettledCommandService> _logger;
    private readonly ICommandService<HandleRecordCommand> _service;
    private readonly IGameRecordUtensil _utensil;

    public PullRecordUnsettledCommandService(
        ILogger<PullRecordUnsettledCommandService> logger,
        IGameRecordUtensil utensil,
        ICommandService<HandleRecordCommand> service)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async ValueTask ExecuteAsync(PullRecordUnsettledCommand command, CancellationToken cancellationToken = default)
    {
        foreach (var record in command.Records)
        {
            try
            {
                var recordInfo = await _utensil
                    .UnsettleConvertRecordAsync(
                        record,
                        cancellationToken)
                    .ConfigureAwait(false);
                await _service
                .ExecuteAsync(
                     command: new HandleRecordCommand(recordInfo),
                     cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "未結算遊戲紀錄發生例外.({record})", record);
            }
        }
    }
}
