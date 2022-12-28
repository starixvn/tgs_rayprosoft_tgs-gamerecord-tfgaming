namespace Tgs.GameRecord.TFGamingService.Core;

public record RedisOptions
{
    public string ConnectionString { get; set; } = "localhost";
}
