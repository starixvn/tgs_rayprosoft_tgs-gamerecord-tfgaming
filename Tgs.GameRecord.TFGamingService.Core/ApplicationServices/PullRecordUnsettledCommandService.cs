using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.TFGamingService.Core.ApplicationServices;

internal class PullRecordUnsettledCommandService : ICommandService<PullRecordUnsettledCommand>
{
    private readonly ILogger<PullRecordUnsettledCommandService> _logger;
    private readonly ICommandService<HandleRecordUnsettledCommand> _service;

    public PullRecordUnsettledCommandService(
        ILogger<PullRecordUnsettledCommandService> logger,
        ICommandService<HandleRecordUnsettledCommand> service)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async ValueTask ExecuteAsync(PullRecordUnsettledCommand command, CancellationToken cancellationToken = default)
    {
        foreach (var record in command.Records)
        {
            try
            {
                await _service
                    .ExecuteAsync(
                    command: new HandleRecordUnsettledCommand(record),
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
