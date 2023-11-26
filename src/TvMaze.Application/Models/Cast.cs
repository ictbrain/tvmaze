using System.Text.Json.Serialization;

namespace TvMaze.Application.Models;

public class Cast
{
    [JsonPropertyName("person")]
    public required Person Person { get; set; }
}
