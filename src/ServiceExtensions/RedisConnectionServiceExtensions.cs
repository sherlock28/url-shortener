namespace url_shortener.ServiceExtensions.RedisConn;

public static partial class IServiceCollectionExtensions
{
    public static IServiceCollection UseRedisConnection(this IServiceCollection serviceCollection)
     => serviceCollection.AddSingleton<RedisConnection>();
}
