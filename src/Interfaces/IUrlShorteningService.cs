namespace url_shortener.Interfaces;

public interface IUrlShorteningService
{
    Task<string> GenerateUniqueCode();
}
