using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using TvMaze.Application.Config;
using TvMaze.Application.Interfaces;
using TvMaze.Application.Models;
using TvMaze.Application.Services;
using TvMaze.Database.Entities;
using TvMaze.Database.Interfaces;
using Xunit;

namespace TvMaze.Application.Tests.Services
{
    public class TvMazeScraperServiceTests
    {
        private readonly TvMazeScraperService service;
        private readonly Mock<ILogger<TvMazeScraperService>> loggerMock;
        private readonly Mock<ITvMazeApiClient>  apiClientMock;
        private readonly Mock<ITvMazeRepository>  repositoryMock;
        private readonly Mock<IOptions<ScraperConfig>> scraperConfigMock;

        public TvMazeScraperServiceTests()
        {
            loggerMock = new Mock<ILogger<TvMazeScraperService>>();
            apiClientMock = new Mock<ITvMazeApiClient>();
            repositoryMock = new Mock<ITvMazeRepository>();
            scraperConfigMock = new Mock<IOptions<ScraperConfig>>();
            var scraperConfig = new ScraperConfig
            {
                ScrapeFirstPageShows = 0,
                ScrapeLastPageShows = 1
            };
            scraperConfigMock.Setup(x => x.Value).Returns(scraperConfig);
            service = new TvMazeScraperService(loggerMock.Object, apiClientMock.Object, repositoryMock.Object, scraperConfigMock.Object);
        }

        [Fact]
        public async Task ScrapeShowsWithCastAsync_SuccessfulScrape()
        {
            // Arrange
            apiClientMock.Setup(x => x.GetShowsAsync(0, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new TvMazeApiResponse()
                         {
                            IsSuccessStatusCode = true,
                            Content = "[{\"id\": 1, \"name\": \"Show1\"}]"
                         });

            apiClientMock.Setup(x => x.GetCastAsync(1, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new TvMazeApiResponse()
                        {
                            IsSuccessStatusCode = true,
                            Content = "[{\"person\": {\"id\": 2, \"name\": \"Actor1\", \"birthday\": \"1990-01-01\"}}]"
                        });

            repositoryMock.Setup(x => x.AddShow(It.IsAny<ShowEntity>(), It.IsAny<CancellationToken>()))
                          .ReturnsAsync(new ShowEntity(1, "Show1") { Id = 1 });

            repositoryMock.Setup(x => x.AddCast(It.IsAny<int>(), It.IsAny<CastEntity>(), It.IsAny<CancellationToken>()));

            // Act
            await service.ScrapeShowsWithCastAsync(CancellationToken.None);

            // Assert
            apiClientMock.Verify(x => x.GetShowsAsync(0, It.IsAny<CancellationToken>()), Times.Once);
            apiClientMock.Verify(x => x.GetCastAsync(1, It.IsAny<CancellationToken>()), Times.Once);
            repositoryMock.Verify(x => x.AddShow(It.IsAny<ShowEntity>(), It.IsAny<CancellationToken>()), Times.Once);
            repositoryMock.Verify(x => x.AddCast(It.IsAny<int>(), It.IsAny<CastEntity>(), It.IsAny<CancellationToken>()), Times.Once);
        }

    }
}
