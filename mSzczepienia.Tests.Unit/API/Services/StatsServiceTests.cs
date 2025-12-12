using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Api.Services;
using mSzczepienia.Shared.DTO.Stats;

namespace mSzczepienia.Tests.Unit.API.Services;

public class StatsServiceTests
{
    private readonly IStatsRepository _statsRepositoryMock;
    private readonly StatsService _sut;

    public StatsServiceTests()
    {
        _statsRepositoryMock = Substitute.For<IStatsRepository>();
        _sut = new StatsService(_statsRepositoryMock);
    }

    [Fact]
    public async Task GetAdminStatsAsync_ShouldReturnStats_WhenRepoReturnsData()
    {
        // Arrange
        var expectedStats = new DashboardStatsDto
        {
            TotalBooklets = 100,
            AcceptedCount = 80,
            RejectedCount = 5,
            NotReviewedCount = 15
        };

        _statsRepositoryMock.GetAdminStatsAsync(Arg.Any<CancellationToken>()).Returns(expectedStats);

        // Act
        var result = await _sut.GetAdminStatsAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedStats);

        result.TotalBooklets.Should().Be(100);
        result.AcceptedCount.Should().Be(80);

        await _statsRepositoryMock.Received(1).GetAdminStatsAsync(Arg.Any<CancellationToken>());
    }
}
