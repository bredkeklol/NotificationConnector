namespace connector.Options;
public class RedisOptions
{
    public string ConnectionString { get; set; } = "redis:6379";
    public string Channel { get; set; } = "notifications";
}