using Microsoft.AspNetCore.Mvc;
using url_shortener.Entities;
using url_shortener.Interfaces;

namespace url_shortener.Controllers;

[Route("api/shorten")]
[ApiController]
public class UrlShorteningController : ControllerBase
{
    public IUrlShorteningService UrlShorteningService;
    public IUrlShorteningDbService UrlShorteningDbService;
    public ICacheService CacheService;

    public record ShortenUrlRequest(string Url);

    public UrlShorteningController(IUrlShorteningService urlShorteningService, IUrlShorteningDbService urlShorteningDbService, ICacheService cacheService)
    {
        UrlShorteningService = urlShorteningService;
        UrlShorteningDbService = urlShorteningDbService;
        CacheService = cacheService;
    }

    [HttpPost("", Name = "shorten")]
    public async Task<IActionResult> GetShortenUrl([FromBody] ShortenUrlRequest request)
    {
        if (!Uri.TryCreate(request.Url, UriKind.Absolute, out _))
        {
            return BadRequest("The specified URL is invalid.");
        }

        var code = await UrlShorteningService.GenerateUniqueCode();

        (bool, string) results = await UrlShorteningDbService.SaveShortenedUrl(HttpContext, request.Url, code);

        return Ok(results.Item2);
    }

    [HttpGet("{code}", Name = "search")]
    public async Task<IActionResult> GetOriginalUrl(string code)
    {
        ShortenedUrlDto? data = await CacheService.GetData(code);

        if (data is not null)
        {
            return Redirect(data.LongUrl);
        }

        ShortenedUrl? shortenedUrl = await UrlShorteningDbService.SearchUrlByCode(code);

        if (shortenedUrl is null)
        {
            return NotFound();
        }

        CacheService.SetData(new ShortenedUrlDto
        {
            Code = shortenedUrl.Code,
            LongUrl = shortenedUrl.LongUrl
        });

        return Redirect(shortenedUrl.LongUrl);
    }
}
