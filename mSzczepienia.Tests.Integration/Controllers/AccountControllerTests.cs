using mSzczepienia.Shared.DTO.Accounts;
using mSzczepienia.Tests.Integration.Fixtures;
using System.Net;

namespace mSzczepienia.Tests.Integration.Controllers;

public class AccountControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public AccountControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    #region GetProfile Tests

    [Fact]
    public async Task GetProfile_WithAdminRole_ShouldReturnOk()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/account/profile");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ProfileDto>();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetProfile_WithWorkerRole_ShouldReturnOk()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");

        // Act
        var response = await client.GetAsync("/api/account/profile");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ProfileDto>();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetProfile_WithCitizenRole_ShouldReturnOk()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("citizen", "test-citizen-uid-1");

        // Act
        var response = await client.GetAsync("/api/account/profile");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ProfileDto>();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetProfile_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClientWithoutAuth();

        // Act
        var response = await client.GetAsync("/api/account/profile");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetProfile_AdminProfile_ShouldReturnCorrectData()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/account/profile");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ProfileDto>();
        result.Should().NotBeNull();
        result.Name.Should().NotBeNullOrEmpty();
        result.Surname.Should().NotBeNullOrEmpty();
        result.Email.Should().NotBeNullOrEmpty();
        result.Role.Should().NotBeNull();
    }

    [Fact]
    public async Task GetProfile_WorkerProfile_ShouldReturnCorrectData()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");

        // Act
        var response = await client.GetAsync("/api/account/profile");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ProfileDto>();
        result.Should().NotBeNull();
        result.Name.Should().Be("Jan");
        result.Surname.Should().Be("Kowalski");
        result.Email.Should().Be("worker1@test.com");
    }

    [Fact]
    public async Task GetProfile_CitizenProfile_ShouldReturnCorrectData()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("citizen", "test-citizen-uid-1");

        // Act
        var response = await client.GetAsync("/api/account/profile");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ProfileDto>();
        result.Should().NotBeNull();
        result.Name.Should().Be("John");
        result.Surname.Should().Be("Smith");
        result.Email.Should().Be("citizen1@test.com");
    }

    [Fact]
    public async Task GetProfile_WithValidToken_ShouldReturnProfileWithAllFields()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-2");

        // Act
        var response = await client.GetAsync("/api/account/profile");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ProfileDto>();
        result.Should().NotBeNull();
        
        result.Name.Should().NotBeNullOrEmpty();
        result.Surname.Should().NotBeNullOrEmpty();
        result.Email.Should().NotBeNullOrEmpty();
        result.CreationDate.Should().NotBe(default(DateTime));
        result.DateOfBirth.Should().NotBe(default(DateTime));
        result.Role.Should().NotBeNull();
    }

    [Fact]
    public async Task GetProfile_MultipleCallsWithSameToken_ShouldReturnConsistentData()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response1 = await client.GetAsync("/api/account/profile");
        var result1 = await response1.Content.ReadFromJsonAsync<ProfileDto>();

        var response2 = await client.GetAsync("/api/account/profile");
        var result2 = await response2.Content.ReadFromJsonAsync<ProfileDto>();

        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        response2.StatusCode.Should().Be(HttpStatusCode.OK);
        result1.Should().NotBeNull();
        result2.Should().NotBeNull();
        result1.Email.Should().Be(result2.Email);
        result1.Name.Should().Be(result2.Name);
    }

    [Fact]
    public async Task GetProfile_DifferentAdminAccounts_ShouldReturnDifferentProfiles()
    {
        // Arrange
        var client1 = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");
        var client2 = _factory.CreateClientWithAuth("admin", "test-admin-uid-2");

        // Act
        var response1 = await client1.GetAsync("/api/account/profile");
        var result1 = await response1.Content.ReadFromJsonAsync<ProfileDto>();

        var response2 = await client2.GetAsync("/api/account/profile");
        var result2 = await response2.Content.ReadFromJsonAsync<ProfileDto>();

        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        response2.StatusCode.Should().Be(HttpStatusCode.OK);
        result1.Should().NotBeNull();
        result2.Should().NotBeNull();
        result1.Email.Should().NotBe(result2.Email);
    }

    [Fact]
    public async Task GetProfile_UnknownFirebaseUid_ShouldReturnNotFound()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "unknown-uid-that-does-not-exist");

        // Act
        var response = await client.GetAsync("/api/account/profile");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetProfile_WithAdminRole_ShouldContainAdminData()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-3");

        // Act
        var response = await client.GetAsync("/api/account/profile");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ProfileDto>();
        result.Should().NotBeNull();
        result!.Name.Should().NotBeNullOrEmpty();
        result.Surname.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetProfile_WithWorkerRole_ShouldContainWorkerData()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-2");

        // Act
        var response = await client.GetAsync("/api/account/profile");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ProfileDto>();
        result.Should().NotBeNull();
        result!.Name.Should().Be("Maria");
        result.Surname.Should().Be("Nowak");
        result.Email.Should().Be("worker2@test.com");
    }

    #endregion

    #region Authorization Tests

    [Fact]
    public async Task GetProfile_WithoutToken_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClientWithoutAuth();

        // Act
        var response = await client.GetAsync("/api/account/profile");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetProfile_WithValidTokenAllRoles_ShouldSucceed()
    {
        // Arrange & Act & Assert - Admin
        var adminClient = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");
        var adminResponse = await adminClient.GetAsync("/api/account/profile");
        adminResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Arrange & Act & Assert - Worker
        var workerClient = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");
        var workerResponse = await workerClient.GetAsync("/api/account/profile");
        workerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Arrange & Act & Assert - Citizen
        var citizenClient = _factory.CreateClientWithAuth("citizen", "test-citizen-uid-1");
        var citizenResponse = await citizenClient.GetAsync("/api/account/profile");
        citizenResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region Response Structure Tests

    [Fact]
    public async Task GetProfile_ShouldReturnValidEmail()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/account/profile");
        var result = await response.Content.ReadFromJsonAsync<ProfileDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.Email.Should().Contain("@");
        result.Email.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetProfile_CreationDateShouldBeValid()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");

        // Act
        var response = await client.GetAsync("/api/account/profile");
        var result = await response.Content.ReadFromJsonAsync<ProfileDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.CreationDate.Should().BeBefore(DateTime.UtcNow.AddSeconds(1));
    }

    #endregion
}
