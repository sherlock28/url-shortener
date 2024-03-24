using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using url_shortener.Config;
using url_shortener.Database;
using url_shortener.Interfaces;

namespace url_shortener.Services;

public class UrlShorteningService : IUrlShorteningService
{
    private readonly IServiceScopeFactory ServiceScopeFactory;
    public IOptionsMonitor<ShortLinkSettings> ShortLinkSettings;
    public int MaxValue { get; set; }

    public UrlShorteningService(IServiceScopeFactory serviceScopeFactory, IOptionsMonitor<ShortLinkSettings> shortLinkSettings)
    {
        ServiceScopeFactory = serviceScopeFactory;
        ShortLinkSettings = shortLinkSettings;
        MaxValue = shortLinkSettings.CurrentValue.Alphabet.Length;
    }

    private readonly Random Random = new();

    public async Task<string> GenerateUniqueCode()
    {
        var codeChars = new char[ShortLinkSettings.CurrentValue.Length];

        while (true)
        {
            using (var scope = ServiceScopeFactory.CreateScope())
            {
                ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                for (var i = 0; i < ShortLinkSettings.CurrentValue.Length; i++)
                {
                    var randomIndex = Random.Next(MaxValue);

                    codeChars[i] = ShortLinkSettings.CurrentValue.Alphabet[randomIndex];
                }

                var code = new string(codeChars);

                if (!await context.ShortenedUrls.AnyAsync(s => s.Code == code))
                {
                    return code;
                }
            }
        }
    }
}