using System.Text.Json.Serialization;

namespace TvMaze.Application.Models;

public class Person
{
    [JsonPropertyName("id")]
    public required int Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("birthday")]
    public DateTime? Birthday { get; set; }
}
