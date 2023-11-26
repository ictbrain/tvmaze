using Microsoft.EntityFrameworkCore;
using TvMaze.Database.Contexts;
using TvMaze.Database.Entities;
using TvMaze.Database.Interfaces;

namespace TvMaze.Database.Repositories;

public class TvMazeRepository : ITvMazeRepository
{
    private readonly TvMazeDbContext _tvMazeDbContext;

    public TvMazeRepository(TvMazeDbContext tvMazeDbContext)
    {
        _tvMazeDbContext = tvMazeDbContext ?? throw new ArgumentNullException(nameof(tvMazeDbContext));
    }

    public async Task<ShowEntity> AddShow(ShowEntity show, CancellationToken cancellationToken = default)
    {
        if (!await _tvMazeDbContext.Show.AnyAsync(s => s.Name == show.Name, cancellationToken))
        {   
            await _tvMazeDbContext.Show.AddAsync(show, cancellationToken);
            await _tvMazeDbContext.SaveChangesAsync(cancellationToken);
            return show;     
        }
        else
        {
            return await _tvMazeDbContext.Show.FirstAsync(s => s.Name == show.Name, cancellationToken = default);
        }
    }

    public async Task AddCast(int showId, CastEntity cast, CancellationToken cancellationToken = default)
    {
        if (!await _tvMazeDbContext.Cast.AnyAsync(c => c.ShowId == showId && c.Name == cast.Name, cancellationToken) )
        {
            await _tvMazeDbContext.Cast.AddAsync(cast, cancellationToken);
            await _tvMazeDbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<List<ShowEntity>> GetShowsWithCast(int skip, int take, CancellationToken cancellationToken = default)
    {
        return await _tvMazeDbContext.Show.Include(i => i.Casts)
                                          .OrderBy(i => i.TvMazeId)
                                          .Skip(skip).Take(take)
                                          .ToListAsync(cancellationToken);
    }
}
