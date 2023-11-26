using TvMaze.Database.Entities;
using TvMaze.Database.Interfaces;
using TvMaze.WebApi.Interfaces;
using TvMaze.WebApi.Models;

namespace TvMaze.WebApi.Services;

public class TvMazeApiService : ITvMazeApiService
{
    private readonly ITvMazeRepository _tvMazeRepository;

    public TvMazeApiService(ITvMazeRepository tvMazeRepository)
    {
        _tvMazeRepository = tvMazeRepository ?? throw new ArgumentNullException(nameof(tvMazeRepository));
    }

    public async Task<List<ShowViewModel>> GetShowViewModels(int skip, int take)
    {
        var shows = await _tvMazeRepository.GetShowsWithCast(skip, take);
        return shows.Select(MapToShowViewModel).ToList();
    }

    private ShowViewModel MapToShowViewModel(ShowEntity show)
    {
        var cast = show.Casts.Select(MapToCastViewModel)
                             .OrderByDescending(c => c.Birthday)
                             .ToList();

        return new ShowViewModel(show.TvMazeId, show.Name, cast);
    }

    private CastViewModel MapToCastViewModel(CastEntity cast)
    {
        return new CastViewModel(cast.Id, cast.Name, cast.Birthday);
    }
}
