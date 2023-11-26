using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace TvMaze.Database.Entities;

[Table("Show")]
public class ShowEntity
{
    public ShowEntity( int tvMazeId, string name )
    {
       TvMazeId = tvMazeId;
       Name = name;
    }

    public int Id { get; set; }
    public int TvMazeId { get; set; }
    public string Name { get; set; }

    public virtual List<CastEntity> Casts { get; set; }
}
