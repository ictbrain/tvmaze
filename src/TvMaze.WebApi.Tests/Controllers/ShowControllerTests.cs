using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using TvMaze.WebApi.Controllers;
using TvMaze.WebApi.Config;
using TvMaze.WebApi.Interfaces;
using TvMaze.WebApi.Models;

namespace TvMaze.WebApi.Tests.Controllers;

public class ShowControllerTests
{
    private readonly Mock<ITvMazeApiService> apiServiceMock;
    private readonly Mock<IOptions<ApiConfig>> apiConfigMock;
    private ShowController showController;

    public ShowControllerTests()
    {
        apiServiceMock = new Mock<ITvMazeApiService>();
        apiConfigMock = new Mock<IOptions<ApiConfig>>();

        var apiConfig = new ApiConfig
        {
            DefaultCurrentShowPage = 1,
            DefaultShowPageSize = 1
        };

        apiConfigMock.Setup(x => x.Value).Returns(apiConfig);

        showController = new ShowController(apiServiceMock.Object, apiConfigMock.Object);
    }

    [Fact]
    public async Task Get_ShouldReturnShowViewModels()
    {
        // Arrange
        int skip = 1;
        int take = 1;
        apiServiceMock.Setup(x => x.GetShowViewModels(skip, take))
                      .ReturnsAsync(
                      [
                        new(1, "show1",
                        [
                          new(2, "ActorName", new DateTime(1990,1,1) ),
                          new(3, "ActorName2", new DateTime(1988,1,1) )
                        ])
                      ]);

        // Act
        var result = await showController.Get(default);

        // Assert
        Assert.NotEmpty(result);
        Assert.IsType<List<ShowViewModel>>(result);
        apiServiceMock.Verify(x => x.GetShowViewModels(1, 1), Times.Once);
    }

    [Fact]
    public async Task Get_WithPageParameter_ShouldReturnShowViewModels()
    {
        // Arrange
        int skip = 2;
        int take = 1;
        var page = 2;

        apiServiceMock.Setup(x => x.GetShowViewModels(skip, take))
              .ReturnsAsync(
              [
                new(1, "show1",
                [
                  new(2, "ActorName", new DateTime(1990, 1, 1)),
                  new(3, "ActorName2", new DateTime(1988, 1, 1))
                ])
              ]);

        // Act
        var result = await showController.Get(page);

        // Assert
        Assert.NotEmpty(result);
        Assert.IsType<List<ShowViewModel>>(result);
        apiServiceMock.Verify(x => x.GetShowViewModels(2, 1), Times.Once);
    }

}
