using Microsoft.Extensions.Options;
using url_shortener.Config;
using url_shortener.Entities;
using url_shortener.Interfaces;

namespace url_shortener.Services;

public class RedisService : ICacheService
{
    private RedisConnection RedisConnection { get; }
    private IOptionsMonitor<RedisConfig> RedisConfig { get; }
    private ILogger<RedisService> Logger;

    public RedisService(RedisConnection redisConnection, IOptionsMonitor<RedisConfig> redisConfig, ILogger<RedisService> logger)
    {
        RedisConnection = redisConnection;
        RedisConfig = redisConfig;
        Logger = logger;
    }

    public async Task<ShortenedUrlDto?> GetData(string key)
    {
        var data = await RedisConnection.Connection.GetDatabase().StringGetAsync(key);

        if (!string.IsNullOrEmpty(data))
        {
            string jsonString = data;

            ShortenedUrlDto? shortenedUrlDto = ShortenedUrlDto.JsonToShortenedUrlDto(jsonString);

            return shortenedUrlDto;
        }

        return null;
    }

    public void SetData(ShortenedUrlDto shortenedUrlDto)
    {
        var expirationTime = DateTimeOffset.Now.AddMinutes(RedisConfig.CurrentValue.ExpirationTimeInMinutes);

        TimeSpan expiryTime = expirationTime.DateTime.Subtract(DateTime.Now);
        try
        {
            var isSet = RedisConnection.Connection.GetDatabase().StringSet(shortenedUrlDto.Code, ShortenedUrlDto.ShortenedUrlDtoToJsonString(shortenedUrlDto), expiryTime);

            if (!isSet)
            {
                Logger.LogError($"Could not save url data with code '{shortenedUrlDto.Code}' in cache");
                return;
            }

            Logger.LogInformation($"URL data with code '{shortenedUrlDto.Code}' saved in cache");
        }
        catch (Exception ex)
        {
            Logger.LogError($"Could not save url data with code '{shortenedUrlDto.Code}' in cache. Exception message: {ex.Message}");
        }
    }
}