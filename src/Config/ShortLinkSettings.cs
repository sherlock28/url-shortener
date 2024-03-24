namespace url_shortener.Config;

public class ShortLinkSettings
{
    public int Length { get; set; } = 7;
    public string Alphabet { get; set; } =
        "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
}