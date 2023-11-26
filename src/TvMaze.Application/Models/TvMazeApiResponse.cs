namespace TvMaze.Application.Models;

public class TvMazeApiResponse
{
    public bool IsSuccessStatusCode { get; set; }
    public string Content { get; set; } = string.Empty;
}
