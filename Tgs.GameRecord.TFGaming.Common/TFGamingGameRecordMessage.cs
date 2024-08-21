using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Tgs.GameRecord.TFGaming.Common;

public record TFGamingGameRecordMessage
{
	[Required]
	[DataType(DataType.Url)]
	[DefaultValue(typeof(Uri), "https://spi-test.r4espt.com/")]
	public Uri ApiUrl { get; init; } = default!;
	[Required]
	[DefaultValue(typeof(string), "00022362f50e16fa474c80fc24da29793420aff6ce4b5d05a4a7dc52a261078ce72ceef10e7dc28f7bcfe89fe4922624fe72992e1f2f4f78360e4360970ce1b3f2e4567d01b071190c9051058fc1c14202f4ea117497c29fdbe8c9eddfb0e4fa6a3efe3bcbc7cfd8665c5b311e4a7065b8ad37a1a30d64919d8a3f2ff82c2ead23eb")]
	public string PrivateToken { get; init; } = default!;
	[Required]
	[DataType(DataType.DateTime)]
	[DefaultValue(typeof(DateTimeOffset), "2022-10-24T00:00:00+08:00")]
	public DateTimeOffset StartTime { get; init; }
	[Required]
	[DataType(DataType.DateTime)]
	[DefaultValue(typeof(DateTimeOffset), "2022-10-25T00:00:00+08:00")]
	public DateTimeOffset EndTime { get; init; }
}
