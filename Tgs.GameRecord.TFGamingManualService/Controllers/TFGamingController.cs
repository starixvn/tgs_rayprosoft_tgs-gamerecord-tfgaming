using Microsoft.AspNetCore.Mvc;
using Tgs.GameRecord.TFGamingManualService.ViewModels;
using Tgs.GameRecord.TFGamingService.Core.WorkerServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.TFGamingManualService.Controllers;

[ApiController]
[Route("[controller]")]
public class TFGamingController : ControllerBase
{
    /// <summary>
    /// Games the history by sports book asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="model">The model.</param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Post
    ///
    ///         {
    ///             "apiUrl": "https://spi-test.r4espt.com/",
    ///             "privateToken": "00022362f50e16fa474c80fc24da29793420aff6ce4b5d05a4a7dc52a261078ce72ceef10e7dc28f7bcfe89fe4922624fe72992e1f2f4f78360e4360970ce1b3f2e4567d01b071190c9051058fc1c14202f4ea117497c29fdbe8c9eddfb0e4fa6a3efe3bcbc7cfd8665c5b311e4a7065b8ad37a1a30d64919d8a3f2ff82c2ead23eb",
    ///             "startTime": "2022-10-24T00:00:00+08:00",
    ///             "endTime": "2022-10-25T00:00:00+08:00
    ///         }
    /// </remarks>
    [HttpPost]
    public ValueTask GameHistoryAsync(
            [FromServices] ICommandService<GameHistoryWorkerCommand> service,
            GameRecordQueryViewModel model)
            => service.ExecuteAsync(new GameHistoryWorkerCommand(
                ApiUrl: model.ApiUrl,
                PrivateToken: model.PrivateToken,
                StartTime: model.StartTime,
                EndTime: model.EndTime));
}
