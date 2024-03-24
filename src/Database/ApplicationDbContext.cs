using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using url_shortener.Config;
using url_shortener.Entities;

namespace url_shortener.Database;

public class ApplicationDbContext : DbContext
{
    public DbSet<ShortenedUrl> ShortenedUrls { get; set; }
    public IOptionsMonitor<ShortLinkSettings> ShortLinkSettings;

    public ApplicationDbContext(DbContextOptions options, IOptionsMonitor<ShortLinkSettings> shortLinkSettings)
        : base(options)
    {
        ShortLinkSettings = shortLinkSettings;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ShortenedUrl>(builder =>
        {
            builder
                .Property(shortenedUrl => shortenedUrl.Code)
                .HasMaxLength(ShortLinkSettings.CurrentValue.Length);

            builder
                .HasIndex(shortenedUrl => shortenedUrl.Code)
                .IsUnique();
        });
    }
}

public static class IServiceCollectionExtensions
{
    public static IServiceCollection UseApplicationDbContext(this IServiceCollection services, IConfiguration config) => services
        .AddDbContext<ApplicationDbContext>(options => options
            .UseNpgsql(config.GetConnectionString("DefaultConnection"), pgOptions => pgOptions
                .MigrationsAssembly("url-shortener")));
}
