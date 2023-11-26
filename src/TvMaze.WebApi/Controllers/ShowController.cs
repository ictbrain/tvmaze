using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TvMaze.WebApi.Config;
using TvMaze.WebApi.Interfaces;
using TvMaze.WebApi.Models;

namespace TvMaze.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShowController : ControllerBase
{
    private readonly ITvMazeApiService _tvMazeApiService;
    private readonly int _defaultShowPageSize;
    private readonly int _defaultCurrentShowPage;

    public ShowController(ITvMazeApiService tvMazeApiService, IOptions<ApiConfig> apiConfig)
    {
        _tvMazeApiService = tvMazeApiService ?? throw new ArgumentNullException(nameof(tvMazeApiService));
        var config = apiConfig?.Value ?? throw new ArgumentNullException(nameof(apiConfig));
        _defaultCurrentShowPage = config.DefaultCurrentShowPage;
        _defaultShowPageSize = config.DefaultShowPageSize;
    }

    public async Task<List<ShowViewModel>> Get(int? page)
    {
        var skip = (page ?? _defaultCurrentShowPage) * _defaultShowPageSize;
        var take = _defaultShowPageSize;
        return await _tvMazeApiService.GetShowViewModels(skip, take);
    }
}
