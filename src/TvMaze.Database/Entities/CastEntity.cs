using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace TvMaze.Database.Entities;

[Table("Cast")]
public class CastEntity
{
    public CastEntity( int tvMazeId, string name, DateTime? birthday, int showId )
    {
        TvMazeId = tvMazeId;
        Name = name;
        Birthday = birthday;
        ShowId = showId;
    }

    public int Id { get; set; }
    public int TvMazeId { get; set; }
    public string Name { get; set; }
    public DateTime? Birthday { get; set; }

    public virtual int ShowId { get; set; }
    public virtual ShowEntity Show { get; set; }
}
