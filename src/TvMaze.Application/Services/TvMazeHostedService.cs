using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TvMaze.Application.Config;
using TvMaze.Application.Interfaces;
using TvMaze.Database.Contexts;

namespace TvMaze.Application.Services;

public class TvMazeHostedService : BackgroundService
{
    private IServiceProvider Services { get; }
    private readonly ILogger<TvMazeHostedService> _logger;
    private readonly ScraperConfig _scraperConfig;

    public TvMazeHostedService(IServiceProvider services, ILogger<TvMazeHostedService> logger, IOptions<ScraperConfig> scraperConfig)
    {
        this.Services = services;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _scraperConfig = scraperConfig?.Value ?? throw new ArgumentNullException(nameof(scraperConfig));
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("TvMaze Hosted Service running.");
        await DoWorkAsync(cancellationToken);
    }

    private async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        using var scope = Services.CreateScope();
        await MigrateAsync(scope, cancellationToken);
        await ScrapeAsync(scope, cancellationToken);
    }

    public async Task MigrateAsync(IServiceScope scope, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Start migrating TvMaze Database");
            var context = scope.ServiceProvider.GetRequiredService<TvMazeDbContext>();
            if (context == null)
            {
                _logger.LogCritical("Failed to Migrate TvMaze Database. TvMazeDbContext context is null");
                return;
            }
            await context.Database.MigrateAsync(cancellationToken);
            await context.Database.EnsureCreatedAsync(cancellationToken);
            _logger.LogInformation("Finished migrating TvMaze Database");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to Migrate TvMaze Database.");
        }
    }

    private async Task ScrapeAsync(IServiceScope scope, CancellationToken cancellationToken)
    {
        var tvMazeScraperService = scope.ServiceProvider.GetRequiredService<ITvMazeScraperService>();

        while (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("Start Scraping Shows with Cast.");
            try
            {
                await tvMazeScraperService.ScrapeShowsWithCastAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("ScraperService Failed. Exception Message={Message}", ex.Message);
            }
            _logger.LogInformation("End Scraping Shows with Cast. And Wait for {hours} hours until next scrape.", _scraperConfig.ScrapeIntervalHours);
            await Task.Delay(TimeSpan.FromHours(_scraperConfig.ScrapeIntervalHours), cancellationToken);
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogDebug("TvMaze Hosted Service is stopping.");
        await Task.CompletedTask;
    }
}

