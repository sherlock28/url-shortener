using url_shortener.Services;
using url_shortener.Interfaces;

namespace url_shortener.ServiceExtensions.CacheService;

public static partial class IServiceCollectionExtensions
{
    public static IServiceCollection UseCache(this IServiceCollection serviceCollection)
     => serviceCollection.AddSingleton<ICacheService, RedisService>();
}
