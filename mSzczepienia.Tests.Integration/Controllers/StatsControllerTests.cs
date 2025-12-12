using System.Net;
using FluentAssertions;
using mSzczepienia.Shared.DTO.Stats;
using mSzczepienia.Tests.Integration.Fixtures;

namespace mSzczepienia.Tests.Integration.Controllers;

[Collection("Stats Controller Collection")]
public class StatsControllerTests
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public StatsControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    #region GetAdminDashboardStats Tests

    [Fact]
    public async Task GetAdminDashboardStats_WithAdminRole_ShouldReturnOk()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin");

        // Act
        var response = await client.GetAsync("/api/stats/admin-dashboard-stats");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<DashboardStatsDto>();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAdminDashboardStats_WithoutRole_ShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("user");

        // Act
        var response = await client.GetAsync("/api/stats/admin-dashboard-stats");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetAdminDashboardStats_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClientWithoutAuth();

        // Act
        var response = await client.GetAsync("/api/stats/admin-dashboard-stats");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAdminDashboardStats_ShouldReturnCorrectCounts()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin");

        // Act
        var response = await client.GetAsync("/api/stats/admin-dashboard-stats");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<DashboardStatsDto>();
        result.Should().NotBeNull();
        result!.TotalBooklets.Should().Be(5); // 5 total submissions
        result.AcceptedCount.Should().Be(2); // 2 accepted
        result.NotReviewedCount.Should().Be(2); // 2 pending
        result.RejectedCount.Should().Be(1); // 1 rejected
    }

    [Fact]
    public async Task GetAdminDashboardStats_ShouldReturnValidDashboardStatsDto()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin");

        // Act
        var response = await client.GetAsync("/api/stats/admin-dashboard-stats");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<DashboardStatsDto>();
        result.Should().NotBeNull();
        result!.TotalBooklets.Should().BeGreaterThanOrEqualTo(0);
        result.AcceptedCount.Should().BeGreaterThanOrEqualTo(0);
        result.NotReviewedCount.Should().BeGreaterThanOrEqualTo(0);
        result.RejectedCount.Should().BeGreaterThanOrEqualTo(0);
        result.TotalBooklets.Should().Be(result.AcceptedCount + result.NotReviewedCount + result.RejectedCount);
    }

    #endregion

    #region Authorization Tests

    [Fact]
    public async Task GetAdminDashboardStats_WithoutToken_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClientWithoutAuth();

        // Act
        var response = await client.GetAsync("/api/stats/admin-dashboard-stats");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAdminDashboardStats_WithUserRole_ShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("user");

        // Act
        var response = await client.GetAsync("/api/stats/admin-dashboard-stats");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetAdminDashboardStats_WithAdminRole_ShouldReturnSuccess()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin");

        // Act
        var response = await client.GetAsync("/api/stats/admin-dashboard-stats");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAdminDashboardStats_ShouldConsistentlyReturnSameData()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin");

        // Act - Call twice to verify consistency
        var response1 = await client.GetAsync("/api/stats/admin-dashboard-stats");
        var result1 = await response1.Content.ReadFromJsonAsync<DashboardStatsDto>();

        var response2 = await client.GetAsync("/api/stats/admin-dashboard-stats");
        var result2 = await response2.Content.ReadFromJsonAsync<DashboardStatsDto>();

        // Assert
        result1.Should().BeEquivalentTo(result2);
    }

    #endregion
}
