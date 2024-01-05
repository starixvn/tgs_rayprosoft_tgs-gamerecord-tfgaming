using Tgs.GameRecord.TFGaming.Core;
namespace Tgs.GameRecord.TFGaming;

public record RedisOptions
{
    public string ConnectionString { get; set; } = "localhost";
}
