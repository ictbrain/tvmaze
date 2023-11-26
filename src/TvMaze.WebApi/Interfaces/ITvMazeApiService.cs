using TvMaze.WebApi.Models;

namespace TvMaze.WebApi.Interfaces;

public interface ITvMazeApiService
{
    Task<List<ShowViewModel>> GetShowViewModels(int take, int skip);
}