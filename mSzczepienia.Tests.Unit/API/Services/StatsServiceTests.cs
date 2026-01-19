using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Api.Services;
using mSzczepienia.Shared.DTO.Stats;
using System.Security.Claims;

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

    #region GetAdminStatsAsync Tests

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
        result.RejectedCount.Should().Be(5);
        result.NotReviewedCount.Should().Be(15);
        await _statsRepositoryMock.Received(1).GetAdminStatsAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAdminStatsAsync_ShouldReturnEmptyStats_WhenNoData()
    {
        // Arrange
        var emptyStats = new DashboardStatsDto { TotalBooklets = 0, AcceptedCount = 0, RejectedCount = 0, NotReviewedCount = 0 };
        _statsRepositoryMock.GetAdminStatsAsync(Arg.Any<CancellationToken>()).Returns(emptyStats);

        // Act
        var result = await _sut.GetAdminStatsAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.TotalBooklets.Should().Be(0);
        result.AcceptedCount.Should().Be(0);
    }

    #endregion

    #region GetFacilityStatsAsync Tests

    [Fact]
    public async Task GetFacilityStatsAsync_ShouldReturnStats_WhenFacilityIdValid()
    {
        // Arrange
        var facilityId = 1;
        var expectedStats = new DashboardStatsDto
        {
            TotalBooklets = 50,
            AcceptedCount = 40,
            RejectedCount = 5,
            NotReviewedCount = 5
        };

        _statsRepositoryMock.GetFacilityStatsAsync(facilityId, Arg.Any<CancellationToken>())
            .Returns(expectedStats);

        // Act
        var result = await _sut.GetFacilityStatsAsync(facilityId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedStats);
        result.TotalBooklets.Should().Be(50);
        result.AcceptedCount.Should().Be(40);
        await _statsRepositoryMock.Received(1).GetFacilityStatsAsync(facilityId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetFacilityStatsAsync_ShouldCallRepositoryWithCorrectFacilityId()
    {
        // Arrange
        var facilityId = 42;
        var stats = new DashboardStatsDto { TotalBooklets = 10 };
        _statsRepositoryMock.GetFacilityStatsAsync(facilityId, Arg.Any<CancellationToken>())
            .Returns(stats);

        // Act
        var result = await _sut.GetFacilityStatsAsync(facilityId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        await _statsRepositoryMock.Received(1).GetFacilityStatsAsync(42, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetFacilityStatsAsync_ShouldReturnEmptyStats_WhenNoSubmissions()
    {
        // Arrange
        var facilityId = 5;
        var emptyStats = new DashboardStatsDto { TotalBooklets = 0 };
        _statsRepositoryMock.GetFacilityStatsAsync(facilityId, Arg.Any<CancellationToken>())
            .Returns(emptyStats);

        // Act
        var result = await _sut.GetFacilityStatsAsync(facilityId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.TotalBooklets.Should().Be(0);
    }

    #endregion

    #region GetWorkerStatsAsync Tests

    [Fact]
    public async Task GetWorkerStatsAsync_ShouldReturnStats_WhenValidUserWithNameIdentifierClaim()
    {
        // Arrange
        var firebaseUid = "worker-uid-123";
        var expectedStats = new DashboardStatsDto
        {
            TotalBooklets = 20,
            AcceptedCount = 15,
            RejectedCount = 2,
            NotReviewedCount = 3
        };

        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, firebaseUid) };
        var identity = new ClaimsIdentity(claims);
        var user = new ClaimsPrincipal(identity);

        _statsRepositoryMock.GetWorkerStatsByFirebaseUidAsync(firebaseUid, Arg.Any<CancellationToken>())
            .Returns(expectedStats);

        // Act
        var result = await _sut.GetWorkerStatsAsync(user, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedStats);
        result.TotalBooklets.Should().Be(20);
        await _statsRepositoryMock.Received(1).GetWorkerStatsByFirebaseUidAsync(firebaseUid, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetWorkerStatsAsync_ShouldReturnStats_WhenValidUserWithUserIdClaim()
    {
        // Arrange
        var firebaseUid = "worker-uid-456";
        var expectedStats = new DashboardStatsDto { TotalBooklets = 25 };

        var claims = new[] { new Claim("user_id", firebaseUid) };
        var identity = new ClaimsIdentity(claims);
        var user = new ClaimsPrincipal(identity);

        _statsRepositoryMock.GetWorkerStatsByFirebaseUidAsync(firebaseUid, Arg.Any<CancellationToken>())
            .Returns(expectedStats);

        // Act
        var result = await _sut.GetWorkerStatsAsync(user, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.TotalBooklets.Should().Be(25);
        await _statsRepositoryMock.Received(1).GetWorkerStatsByFirebaseUidAsync(firebaseUid, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetWorkerStatsAsync_ShouldReturnStats_WhenValidUserWithUidClaim()
    {
        // Arrange
        var firebaseUid = "worker-uid-789";
        var expectedStats = new DashboardStatsDto { TotalBooklets = 30 };

        var claims = new[] { new Claim("uid", firebaseUid) };
        var identity = new ClaimsIdentity(claims);
        var user = new ClaimsPrincipal(identity);

        _statsRepositoryMock.GetWorkerStatsByFirebaseUidAsync(firebaseUid, Arg.Any<CancellationToken>())
            .Returns(expectedStats);

        // Act
        var result = await _sut.GetWorkerStatsAsync(user, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.TotalBooklets.Should().Be(30);
    }

    [Fact]
    public async Task GetWorkerStatsAsync_ShouldThrowUnauthorizedAccessException_WhenMissingFirebaseUid()
    {
        // Arrange
        var claims = new Claim[0];
        var identity = new ClaimsIdentity(claims);
        var user = new ClaimsPrincipal(identity);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _sut.GetWorkerStatsAsync(user, CancellationToken.None));
    }

    [Fact]
    public async Task GetWorkerStatsAsync_ShouldThrowUnauthorizedAccessException_WhenNullUser()
    {
        // Arrange
        var identity = new ClaimsIdentity(new Claim[0]);
        var user = new ClaimsPrincipal(identity);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _sut.GetWorkerStatsAsync(user, CancellationToken.None));
    }

    #endregion
}
