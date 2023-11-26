using TvMaze.Database.Entities;

namespace TvMaze.Database.Interfaces;

public interface ITvMazeRepository
{
    Task<ShowEntity> AddShow(ShowEntity show, CancellationToken cancellationToken = default);
    Task AddCast(int showId, CastEntity cast, CancellationToken cancellationToken = default);
    Task<List<ShowEntity>> GetShowsWithCast(int skip, int take, CancellationToken cancellationToken = default);
}
