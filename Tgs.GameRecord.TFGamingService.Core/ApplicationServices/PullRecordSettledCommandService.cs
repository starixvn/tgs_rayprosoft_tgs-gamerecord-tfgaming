using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.TFGamingService.Core.ApplicationServices;

internal class PullRecordSettledCommandService : ICommandService<PullRecordSettledCommand>
{
    private readonly ILogger<PullRecordSettledCommandService> _logger;
    private readonly ICommandService<HandleRecordCommand> _service;
    private readonly IGameRecordUtensil _utensil;

    public PullRecordSettledCommandService(
        ILogger<PullRecordSettledCommandService> logger,
        IGameRecordUtensil utensil,
        ICommandService<HandleRecordCommand> service)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async ValueTask ExecuteAsync(PullRecordSettledCommand command, CancellationToken cancellationToken = default)
    {
        foreach (var record in command.Records)
        {
            try
            {
                var recordInfo = await _utensil
                    .SettleConvertRecordAsync(
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
                _logger.LogError(ex, "結算遊戲紀錄發生例外.({record})", record);
            }
        }
    }
}
