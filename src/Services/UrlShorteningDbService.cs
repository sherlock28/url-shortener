using Microsoft.Extensions.Options;
using url_shortener.Config;
using url_shortener.Entities;
using url_shortener.Database;
using url_shortener.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace url_shortener.Services;

public class UrlShorteningDbService : IUrlShorteningDbService
{
    private readonly IServiceScopeFactory ServiceScopeFactory;
    public IOptionsMonitor<ShortLinkSettings> ShortLinkSettings;
    public int MaxValue { get; set; }

    public UrlShorteningDbService(IServiceScopeFactory serviceScopeFactory, IOptionsMonitor<ShortLinkSettings> shortLinkSettings)
    {
        ServiceScopeFactory = serviceScopeFactory;
        ShortLinkSettings = shortLinkSettings;
    }

    public async Task<(bool, string)> SaveShortenedUrl(HttpContext httpContext, string url, string code)
    {
        try
        {
            using (var scope = ServiceScopeFactory.CreateScope())
            {
                ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var shortenedUrl = new ShortenedUrl
                {
                    Id = Guid.NewGuid(),
                    LongUrl = url,
                    Code = code,
                    ShortUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/{code}",
                    CreatedOnUtc = DateTime.UtcNow
                };


                context.ShortenedUrls.Add(shortenedUrl);

                await context.SaveChangesAsync();

                return (true, shortenedUrl.ShortUrl);
            }
        }
        catch (Exception)
        {
            return (false, string.Empty);
        }
    }

    public async Task<ShortenedUrl?> SearchUrlByCode(string code)
    {
        using (var scope = ServiceScopeFactory.CreateScope())
        {
            ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            ShortenedUrl? shortenedUrl = await context.ShortenedUrls.FirstOrDefaultAsync(s => s.Code == code);

            return shortenedUrl;
        }
    }
}