using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Polly;
using Polly.RateLimit;
using Polly.Wrap;
using System.Net;
using TvMaze.Application.Clients;
using TvMaze.Application.Config;
using TvMaze.Application.Interfaces;
using TvMaze.Application.Services;
using TvMaze.Database.Contexts;
using TvMaze.Database.Interfaces;
using TvMaze.Database.Migrations.Sqlite;
using TvMaze.Database.Repositories;
using TvMaze.WebApi.Config;
using TvMaze.WebApi.Helpers;
using TvMaze.WebApi.Interfaces;
using TvMaze.WebApi.Services;

namespace TvMaze.WebApi;

public class Startup
{
    public Startup(ConfigurationManager configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers()
                .AddJsonOptions(options =>
                 {
                     options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
                 });

        services.Configure<ScraperConfig>(Configuration.GetSection(nameof(ScraperConfig)));
        services.Configure<ApiConfig>(Configuration.GetSection(nameof(ApiConfig)));

        services.AddScoped<ITvMazeApiClient, TvMazeApiClient>();
        services.AddScoped<ITvMazeScraperService, TvMazeScraperService>();
        services.AddScoped<ITvMazeRepository, TvMazeRepository>();
        services.AddScoped<ITvMazeApiService, TvMazeApiService>();

        services.AddHostedService<TvMazeHostedService>();

        services.AddDbContext<TvMazeDbContext>(o => {
                    o.UseSqlite(Configuration.GetConnectionString(nameof(TvMazeDbContext)),
                    sql => sql.MigrationsAssembly(typeof(SqliteMigrationsAssemblyMarker).Assembly.FullName));
                });

        var scraperConfig = new ScraperConfig();
        Configuration.GetSection(nameof(ScraperConfig)).Bind(scraperConfig);

        services.AddHttpClient<ITvMazeApiClient, TvMazeApiClient>()
                .ConfigureHttpClient((sp, httpClient) =>
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(scraperConfig.HttpTimeoutSeconds);
                    httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, scraperConfig.UserAgent);
                    httpClient.BaseAddress = new Uri(scraperConfig.TvMazeBaseUrl);
                })
               .AddPolicyHandler(GetResiliencePolicy(scraperConfig));
    }

    public void Configure(IApplicationBuilder app, IEndpointRouteBuilder route, IWebHostEnvironment env)
    {
        if (!env.IsDevelopment()) 
        {
            app.UseExceptionHandler("/Error");
        }

        route.MapControllers();
    }

    private static AsyncPolicyWrap<HttpResponseMessage> GetResiliencePolicy(ScraperConfig scraperConfig)
    {
        var limit = Policy.RateLimitAsync(scraperConfig.ScrapeRateLimitApiCalls, 
                                          TimeSpan.FromSeconds(scraperConfig.ScrapeRateLimitTimeSpanSeconds), 
                                          scraperConfig.ScrapeRateLimitApiCalls,
                                          (retryAfter, context) =>
                                          {
                                              var response = new HttpResponseMessage(HttpStatusCode.TooManyRequests);
                                              response.Headers.Add(HeaderNames.RetryAfter, retryAfter.Milliseconds.ToString());
                                              return response;
                                          });

        var retry = Policy.HandleResult<HttpResponseMessage>(result => result.StatusCode == HttpStatusCode.TooManyRequests)
                          .Or<RateLimitRejectedException>()
                          .WaitAndRetryForeverAsync((retryNum) => {
                              Console.WriteLine($"Retrying. Num: {retryNum}"); //TODO
                              return TimeSpan.FromSeconds(1);
                          });

        return Policy.WrapAsync(retry, limit);
    }
}
