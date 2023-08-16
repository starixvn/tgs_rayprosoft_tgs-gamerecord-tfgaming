using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.TFGamingService.Core.ApplicationServices;

internal class PullRecordSettledCommandService : ICommandService<PullRecordSettledCommand>
{
    private readonly ILogger<PullRecordSettledCommandService> _logger;
    private readonly ICommandService<HandleRecordSettledCommand> _service;

    public PullRecordSettledCommandService(
        ILogger<PullRecordSettledCommandService> logger,
        ICommandService<HandleRecordSettledCommand> service)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async ValueTask ExecuteAsync(PullRecordSettledCommand command, CancellationToken cancellationToken = default)
    {
        foreach (var record in command.Records)
        {
            try
            {
                await _service
                    .ExecuteAsync(
                    command: new HandleRecordSettledCommand(record),
                     cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LogOn:{logOn} Message: An exception occurred in this game record.({gameRecord})", DateTime.Now.ToString("o"), record);
            }
        }
    }
}
