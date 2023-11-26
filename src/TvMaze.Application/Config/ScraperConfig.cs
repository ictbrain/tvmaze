namespace TvMaze.Application.Config;

public class ScraperConfig
{
    public string TvMazeBaseUrl { get; set; } = "http://api.tvmaze.com";
    public string UserAgent { get; set; } = "TvMazeScraper UserAgent";
    public int HttpTimeoutSeconds { get; set; } = 30;
    public int ScrapeIntervalHours { get; set; } = 24;
    public int ScrapeFirstPageShows { get; set; } = 0;
    public int ScrapeLastPageShows { get; set; } = 292;
    public int ScrapeRateLimitApiCalls { get; set; } = 40;
    public int ScrapeRateLimitTimeSpanSeconds { get; set; } = 1;
}
