using url_shortener.Services;
using url_shortener.Interfaces;

namespace url_shortener.ServiceExtensions.DatabaseService;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection UseUrlShorteningDbService(this IServiceCollection @this) =>
        @this.AddSingleton<IUrlShorteningDbService, UrlShorteningDbService>();
}
