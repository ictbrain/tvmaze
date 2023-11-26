using System.Text.Json.Serialization;

namespace TvMaze.Application.Models;

public class Show
{
    [JsonPropertyName("id")]
    public required int Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }
}
