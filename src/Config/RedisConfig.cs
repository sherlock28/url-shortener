namespace url_shortener.Config;

public class RedisConfig
{
    public string Host { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public double ExpirationTimeInMinutes { get; set; } = 5.0;
}
