using TvMaze.Application.Models;

namespace TvMaze.Application.Interfaces;

public interface ITvMazeApiClient
{
    Task<TvMazeApiResponse> GetShowsAsync(int page, CancellationToken cancellationToken = default);

    Task<TvMazeApiResponse> GetCastAsync(int showId, CancellationToken cancellationToken = default);
}