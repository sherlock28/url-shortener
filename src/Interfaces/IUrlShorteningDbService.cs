using url_shortener.Entities;

namespace url_shortener.Interfaces;

public interface IUrlShorteningDbService
{
    Task<(bool, string)> SaveShortenedUrl(HttpContext httpContext, string url, string code);
    Task<ShortenedUrl?> SearchUrlByCode(string code);
}
