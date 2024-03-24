using Newtonsoft.Json;

namespace url_shortener.Entities;

public class ShortenedUrlDto
{
    public string LongUrl { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;

    public static string ShortenedUrlDtoToJsonString(ShortenedUrlDto shortenedUrl) => JsonConvert.SerializeObject(shortenedUrl);

    public static ShortenedUrlDto? JsonToShortenedUrlDto(string shortenedUrlDto) => JsonConvert.DeserializeObject<ShortenedUrlDto>(shortenedUrlDto);
}
