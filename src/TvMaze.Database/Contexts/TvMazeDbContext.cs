using Microsoft.EntityFrameworkCore;
using TvMaze.Database.Entities;

namespace TvMaze.Database.Contexts;

public class TvMazeDbContext : DbContext
{
    public TvMazeDbContext(DbContextOptions<TvMazeDbContext> options) : base (options)
    {
    }

    public DbSet<ShowEntity> Show { get; set; }
    public DbSet<CastEntity> Cast { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        MapShowEntity(builder);
        MapCastEntity(builder);
    }

    private static void MapShowEntity(ModelBuilder builder)
    {
        var entity = builder.Entity<ShowEntity>();

        entity.HasKey(s => s.Id);

        entity.Property(s => s.Id)
              .IsRequired()
              .ValueGeneratedOnAdd();

        entity.Property(s => s.TvMazeId)
              .IsRequired();

        entity.HasIndex(s => s.TvMazeId)
              .IsUnique();

        entity.Property(s => s.Name)
              .IsRequired();

        entity.HasIndex(s =>s.Name)
              .IsUnique();

        entity.HasMany(s => s.Casts)
              .WithOne(c => c.Show)
              .HasForeignKey(c => c.ShowId);
    }

    private static void MapCastEntity(ModelBuilder builder)
    {
        var entity = builder.Entity<CastEntity>();

        entity.HasKey(c => c.Id);

        entity.Property(s => s.Id)
              .IsRequired()
              .ValueGeneratedOnAdd();

        entity.Property(s => s.TvMazeId)
              .IsRequired();

        entity.Property(s => s.Name)
              .IsRequired();

        entity.HasIndex(c => new { c.ShowId, c.Name })
              .IsUnique();

        entity.HasOne(c => c.Show)
              .WithMany(s => s.Casts)
              .HasForeignKey(c => c.ShowId);
    }
}
