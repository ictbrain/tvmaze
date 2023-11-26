using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using TvMaze.Application.Config;
using TvMaze.Application.Interfaces;
using TvMaze.Application.Models;
using TvMaze.Database.Entities;
using TvMaze.Database.Interfaces;

namespace TvMaze.Application.Services;

public class TvMazeScraperService : ITvMazeScraperService
{
    private readonly ILogger<TvMazeScraperService> _logger;
    private readonly ITvMazeApiClient _tvMazeApiClient;
    private readonly ITvMazeRepository _tvMazeRepository;
    private readonly ScraperConfig _scraperConfig;

    public TvMazeScraperService(ILogger<TvMazeScraperService> logger, ITvMazeApiClient tvMazeApiClient, ITvMazeRepository tvMazeRepository, IOptions<ScraperConfig> scraperConfig)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _tvMazeApiClient = tvMazeApiClient ?? throw new ArgumentNullException(nameof(tvMazeApiClient));
        _tvMazeRepository = tvMazeRepository ?? throw new ArgumentNullException(nameof(tvMazeRepository));
        _scraperConfig = scraperConfig?.Value ?? throw new ArgumentNullException(nameof(scraperConfig));
    }

    public async Task ScrapeShowsWithCastAsync(CancellationToken cancellationToken)
    {
        int currentPage = _scraperConfig.ScrapeFirstPageShows;
        int lastPage = _scraperConfig.ScrapeLastPageShows;
        while (currentPage < lastPage)
        {
            try
            {
                await GetShowPageWithCastAsync(currentPage, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to Scrape shows for page{page}. Exception Message={message}", currentPage, ex.Message);
            }
            currentPage++;
        }
    }

    private async Task GetShowPageWithCastAsync(int page, CancellationToken cancellationToken = default)
    {
        try
        {
            var showsInPageResponse = await _tvMazeApiClient.GetShowsAsync(page, cancellationToken);
            if (!showsInPageResponse.IsSuccessStatusCode || string.IsNullOrEmpty(showsInPageResponse.Content))
            {
                _logger.LogError("Failed to get Show for page {page} from TvMazeApi", page);
                return;
            }

            var showsInPage = MapTvMazeApiClientResponse<List<Show>>(showsInPageResponse.Content);
            _logger.LogInformation("Received page={page} from the API, got {totalShows} show(s)", page, showsInPage.Count);

            foreach (var show in showsInPage)
            {
                int showId = show.Id;

                try
                {
                    var showEntity = MapToShowEntity(show);
                    showEntity = await _tvMazeRepository.AddShow(showEntity, cancellationToken);

                    var castResponse = await _tvMazeApiClient.GetCastAsync(showId, cancellationToken);
                    if (!castResponse.IsSuccessStatusCode || string.IsNullOrEmpty(castResponse.Content))
                    {
                        _logger.LogError("Failed to get Cast for show {showId} from TvMazeApi", showId);
                        continue;
                    }

                    var cast = MapTvMazeApiClientResponse<List<Cast>>(castResponse.Content);
                    _logger.LogInformation("Retrieved the cast for show {showId}", showId);

                    var castEntities = MapToUniqueCastEntities(cast, showEntity.Id);
                    foreach (var castEntity in castEntities)
                    {
                        await _tvMazeRepository.AddCast(showEntity.Id, castEntity, cancellationToken);
                    }

                    _logger.LogInformation("Saved show with id={showId} with cast to Repositoy", showId);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error when retrieving cast for show {showId}. Exception={message}", showId, ex.Message);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error when getting page {page} from the API", page);
        }
    }

    private static ShowEntity MapToShowEntity(Show show)
    {
        return new ShowEntity(show.Id, show.Name);
    }

    private static List<CastEntity> MapToUniqueCastEntities(IEnumerable<Cast> cast, int showId)
    {
        return cast.DistinctBy(c => c.Person.Name)
                   .Select(c => new CastEntity(c.Person.Id, c.Person.Name, c.Person.Birthday, showId))
                   .ToList();
    }

    private static T MapTvMazeApiClientResponse<T>( string content )
    {
        return JsonSerializer.Deserialize<T>(content) ?? throw new Exception($"Failed to deserialize json from content={content}"); ;
    }
}
