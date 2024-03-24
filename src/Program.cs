using Serilog;
using Microsoft.EntityFrameworkCore;
using url_shortener.Config;
using url_shortener.Database;
using url_shortener.ServiceExtensions;
using url_shortener.ServiceExtensions.RedisConn;
using url_shortener.ServiceExtensions.DatabaseService;
using url_shortener.ServiceExtensions.CacheService;

var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.override.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.secret.json", optional: true, reloadOnChange: false)
                .AddUserSecrets<Program>(optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration)
                    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.Configure<RedisConfig>(configuration.GetSection("Redis"));
builder.Services.Configure<ShortLinkSettings>(configuration.GetSection("ShortLinkSettings"));

// Add services to the container.
builder.Services.AddSingleton(new HttpClient { });

builder.Services.AddControllers();
builder.Services.UseUrlShorteningService();
builder.Services.UseUrlShorteningDbService();
builder.Services.UseCache();
builder.Services.UseRedisConnection();
builder.Services.UseApplicationDbContext(configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
var isSwaggerEnabled = configuration.GetValue<bool>("SwaggerEnabled");

if (isSwaggerEnabled)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();
}


app.UseAuthorization();

app.UseRouting();
app.MapControllers();

app.Run();

