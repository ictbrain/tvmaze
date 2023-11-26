using Moq;
using TvMaze.WebApi.Services;
using TvMaze.Database.Entities;
using TvMaze.Database.Interfaces;
using TvMaze.WebApi.Models;
using Xunit;

namespace TvMaze.WebApi.Tests.Service;

public class TvMazeApiServiceTests
{
    private readonly TvMazeApiService _tvMazeApiService;
    private readonly Mock<ITvMazeRepository> _tvMazeRepository;

    public TvMazeApiServiceTests()
    {
        _tvMazeRepository = new Mock<ITvMazeRepository>();
        _tvMazeApiService = new TvMazeApiService(_tvMazeRepository.Object);
    }

    [Fact]
    public async void GetShowViewModels_Return_ShouldContain_ShowIdAndlistOfCast()
    {
        // Arrange
        int skip = 0;
        int take = 2;
        int showOne = 1;
        int showFour = 4;
        var birthDateDeanNorris = new DateTime(1963, 04, 08);
        var birthDateMikeVogel = new DateTime(1979, 07, 17);

        _tvMazeRepository.Setup(m => m.GetShowsWithCast(skip, take, default))
                         .ReturnsAsync(
                         [
                             new ShowEntity(1, "Game of Thrones")
                             {
                                 Id = showOne,
                                 Casts =
                                 [
                                    new CastEntity(7, "Dean Norris", birthDateDeanNorris, showOne) { Id = 7 },
                                    new CastEntity(9, "Mike Vogel", birthDateMikeVogel, showOne) { Id = 9 },
                                 ]
                             },
                             new ShowEntity(4, "Big Bang Theory")
                             {
                                 Id = showFour,
                                 Casts =
                                 [
                                    new CastEntity(6, "Michael Emerson", new DateTime(1950, 01, 01), showFour) { Id = 6 },
                                 ]
                             }
                        ]);

        // Act
        var result = await _tvMazeApiService.GetShowViewModels(skip, take);

        // Assert
        _tvMazeRepository.Verify(m => m.GetShowsWithCast(skip, take, default), Times.Once);
        Assert.NotEmpty(result);
        Assert.IsType<List<ShowViewModel>>(result);
        Assert.Equal(showOne, result.First().Id);
        Assert.Equal(2, result.First().Cast.Count);
        Assert.Equal(showFour, result.Last().Id);
        Assert.Single(result.Last().Cast);
    }

    [Fact]
    public async void GetShowViewModels_Returns_ShowsWithCastOrderedByBirthdate()
    {
        // Arrange
        int skip = 0;
        int take = 2;
        int showOne = 1;
        int showFour = 4;
        var birthDateDeanNorris = new DateTime(1963, 04, 08);
        var birthDateMikeVogel = new DateTime(1979, 07, 17);

        _tvMazeRepository.Setup(m => m.GetShowsWithCast(skip, take, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(
                         [
                             new ShowEntity(1, "Game of Thrones")
                             {
                                 Id = showOne,
                                 Casts =
                                 [
                                    new CastEntity(7, "Dean Norris", birthDateDeanNorris, showOne) { Id = 7 },
                                    new CastEntity(9, "Mike Vogel", birthDateMikeVogel, showOne) { Id = 9 },
                                 ]
                             },
                             new ShowEntity(4, "Big Bang Theory")
                             {
                                 Id = showFour,
                                 Casts =
                                 [
                                    new CastEntity(6, "Michael Emerson", new DateTime(1950, 01, 01), showFour) { Id = 6 },
                                 ]
                             }
                        ]);

        // Act
        var result = await _tvMazeApiService.GetShowViewModels(skip, take);

        // Assert
        _tvMazeRepository.Verify(m => m.GetShowsWithCast(skip, take, It.IsAny<CancellationToken>()), Times.Once);
        Assert.NotEmpty(result);
        Assert.IsType<List<ShowViewModel>>(result);
        Assert.Equal(birthDateMikeVogel, result.First().Cast.First().Birthday);
        Assert.Equal(birthDateDeanNorris, result.First().Cast.Last().Birthday);
    }

}

