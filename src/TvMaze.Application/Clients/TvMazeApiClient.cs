using TvMaze.Application.Interfaces;
using TvMaze.Application.Models;

namespace TvMaze.Application.Clients;

public class TvMazeApiClient : ITvMazeApiClient
{
    private readonly HttpClient _httpClient;

    public TvMazeApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<TvMazeApiResponse> GetShowsAsync(int page, CancellationToken cancellationToken = default)
    {
        return await GetResponseApiAsync($"/shows?page={page}", cancellationToken);
    }

    public async Task<TvMazeApiResponse> GetCastAsync(int showId, CancellationToken cancellationToken = default)
    {
        return await GetResponseApiAsync($"/shows/{showId}/cast", cancellationToken);
    }

    private async Task<TvMazeApiResponse> GetResponseApiAsync(string url, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage httpResponse = await _httpClient.GetAsync(url, cancellationToken);
        var apiResponse = new TvMazeApiResponse();

        if (httpResponse.IsSuccessStatusCode)
        {
            apiResponse.IsSuccessStatusCode = true;
            apiResponse.Content = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
        }
        return apiResponse;
    }
}


