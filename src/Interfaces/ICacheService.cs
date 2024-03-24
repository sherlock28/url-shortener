using url_shortener.Entities;

namespace url_shortener.Interfaces;

public interface ICacheService
{
    void SetData(ShortenedUrlDto shortenedUrlDto);
    Task<ShortenedUrlDto?> GetData(string key);
}
