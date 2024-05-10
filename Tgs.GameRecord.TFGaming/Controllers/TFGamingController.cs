using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Tgs.GameRecord.TFGaming.Common;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.TFGaming.Controllers;

[ApiController]
[Route("[controller]")]
public class TFGamingController : ControllerBase
{
	/// <summary>
	/// Games the record manual asynchronous.
	/// </summary>
	/// <param name="messageSender">The message sender.</param>
	/// <param name="source">The source.</param>
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
	///             "endTime": "2022-10-25T00:00:00+08:00"
	///         }
	/// </remarks>
	[HttpPost("manual")]
	public ValueTask GameRecordManualAsync(
		[FromServices] IMessageSender messageSender,
		TFGamingGameRecordMessage source)
		=> messageSender.PublishAsync(
			subject: TFGamingNatsConst.ManualPublish,
			data: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(source)),
			cancellationToken: HttpContext.RequestAborted);

	/// <summary>
	/// Games the record schedule asynchronous.
	/// </summary>
	/// <param name="messageSender">The message sender.</param>
	/// <param name="source">The source.</param>
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
	///             "endTime": "2022-10-25T00:00:00+08:00"
	///         }
	/// </remarks>
	[HttpPost("schedule")]
	public ValueTask GameRecordScheduleAsync(
		[FromServices] IMessageSender messageSender,
		TFGamingGameRecordMessage source)
		=> messageSender.PublishAsync(
			subject: TFGamingNatsConst.SchedulePublish,
			data: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(source)),
			cancellationToken: HttpContext.RequestAborted);
}
