namespace TvMaze.WebApi.Models;

public class ShowViewModel
{
    public ShowViewModel(int id, string name, List<CastViewModel> cast)
    {
        Id = id;
        Name = name;
        Cast = cast;
    }

    public int Id { get; }
    public string Name { get; }
    public List<CastViewModel> Cast { get; }
}
