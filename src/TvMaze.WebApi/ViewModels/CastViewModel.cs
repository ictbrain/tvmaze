namespace TvMaze.WebApi.Models;

public class CastViewModel
{
    public CastViewModel(int id, string name, DateTime? birthday)
    {
        Id = id;
        Name = name;
        Birthday = birthday;
    }

    public int Id { get; }
    public string Name { get; }
    public DateTime? Birthday { get; }
}
