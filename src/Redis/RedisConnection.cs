using StackExchange.Redis;
using Microsoft.Extensions.Options;
using url_shortener.Config;

public class RedisConnection
{
    public ConnectionMultiplexer Connection;
    private IOptionsMonitor<RedisConfig> RedisConfig { get; }

    public RedisConnection(IOptionsMonitor<RedisConfig> redisConfig)
    {
        RedisConfig = redisConfig;
        Connection = GetRedisConnection();
    }

    public ConnectionMultiplexer GetRedisConnection()
    {
        var configOptions = ConfigurationOptions.Parse(RedisConfig.CurrentValue.Host);
        configOptions.Password = RedisConfig.CurrentValue.Password;

        return ConnectionMultiplexer.Connect(configOptions);
    }
}
