namespace TvMaze.Application.Interfaces;

public interface ITvMazeScraperService
{
    Task ScrapeShowsWithCastAsync(CancellationToken stoppingToken = default);
}