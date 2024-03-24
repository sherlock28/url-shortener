using url_shortener.Services;
using url_shortener.Interfaces;

namespace url_shortener.ServiceExtensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection UseUrlShorteningService(this IServiceCollection @this) =>
        @this.AddSingleton<IUrlShorteningService, UrlShorteningService>();
}
