using mSzczepienia.Shared.DTO.Citizens;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Tests.Integration.Fixtures;
using System.Net;
using System.Net.Http.Json;

namespace mSzczepienia.Tests.Integration.Controllers;

public class CitizenControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public CitizenControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    #region GetCitizenDetails Tests

    [Fact]
    public async Task GetCitizenDetails_WithCitizenRole_ShouldReturnOk()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("citizen", "test-citizen-uid-1");

        // Act
        var response = await client.GetAsync("/api/citizens/citizen-details");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<CitizenDetailsDto>();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetCitizenDetails_WithAdminRole_ShouldReturnOk()
    {
        // Arrange - admin with citizen profile
        var client = _factory.CreateClientWithAuth("admin", "test-citizen-uid-1");

        // Act
        var response = await client.GetAsync("/api/citizens/citizen-details");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetCitizenDetails_WithWorkerRole_ShouldReturnOk()
    {
        // Arrange - worker with citizen profile
        var client = _factory.CreateClientWithAuth("worker", "test-citizen-uid-1");

        // Act
        var response = await client.GetAsync("/api/citizens/citizen-details");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetCitizenDetails_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClientWithoutAuth();

        // Act
        var response = await client.GetAsync("/api/citizens/citizen-details");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetCitizenDetails_WithValidCitizen_ShouldReturnCorrectData()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("citizen", "test-citizen-uid-1");

        // Act
        var response = await client.GetAsync("/api/citizens/citizen-details");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<CitizenDetailsDto>();
        result.Should().NotBeNull();
        result!.name.Should().Be("John");
        result.surname.Should().Be("Smith");
        result.email.Should().Be("citizen1@test.com");
        result.pesel.Should().Be("12345678901");
        result.facilityName.Should().Be("Test Clinic 1");
    }

    [Fact]
    public async Task GetCitizenDetails_SecondCitizen_ShouldReturnCorrectData()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("citizen", "test-citizen-uid-2");

        // Act
        var response = await client.GetAsync("/api/citizens/citizen-details");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<CitizenDetailsDto>();
        result.Should().NotBeNull();
        result!.name.Should().Be("Anna");
        result.surname.Should().Be("Johnson");
        result.email.Should().Be("citizen2@test.com");
        result.pesel.Should().Be("98765432101");
        result.facilityName.Should().Be("Test Clinic 2");
    }

    [Fact]
    public async Task GetCitizenDetails_UnknownUid_ShouldReturnNotFound()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("citizen", "unknown-citizen-uid");

        // Act
        var response = await client.GetAsync("/api/citizens/citizen-details");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetCitizenDetails_ShouldReturnAllFields()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("citizen", "test-citizen-uid-1");

        // Act
        var response = await client.GetAsync("/api/citizens/citizen-details");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<CitizenDetailsDto>();
        result.Should().NotBeNull();
        result!.name.Should().NotBeNullOrEmpty();
        result.surname.Should().NotBeNullOrEmpty();
        result.email.Should().NotBeNullOrEmpty();
        result.pesel.Should().NotBeNullOrEmpty();
        result.facilityName.Should().NotBeNullOrEmpty();
        result.submissionStatus.Should().NotBeNullOrEmpty();
        result.dateOfBirth.Should().NotBe(default);
        result.creationDate.Should().NotBe(default);
    }

    [Fact]
    public async Task GetCitizenDetails_MultipleCallsWithSameToken_ShouldReturnConsistentData()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("citizen", "test-citizen-uid-1");

        // Act
        var response1 = await client.GetAsync("/api/citizens/citizen-details");
        var result1 = await response1.Content.ReadFromJsonAsync<CitizenDetailsDto>();

        var response2 = await client.GetAsync("/api/citizens/citizen-details");
        var result2 = await response2.Content.ReadFromJsonAsync<CitizenDetailsDto>();

        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        response2.StatusCode.Should().Be(HttpStatusCode.OK);
        result1.Should().NotBeNull();
        result2.Should().NotBeNull();
        result1!.pesel.Should().Be(result2!.pesel);
        result1.email.Should().Be(result2.email);
    }

    #endregion

    #region GetCitizenById Tests

    [Fact]
    public async Task GetCitizenById_WithWorkerRole_ShouldReturnOk()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");

        // Act
        var response = await client.GetAsync("/api/citizens/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<CitizenDetailsDto>();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetCitizenById_WithAdminRole_ShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/citizens/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetCitizenById_WithCitizenRole_ShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("citizen", "test-citizen-uid-1");

        // Act
        var response = await client.GetAsync("/api/citizens/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetCitizenById_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClientWithoutAuth();

        // Act
        var response = await client.GetAsync("/api/citizens/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetCitizenById_ValidId_ShouldReturnCorrectData()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");

        // Act
        var response = await client.GetAsync("/api/citizens/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<CitizenDetailsDto>();
        result.Should().NotBeNull();
        result!.name.Should().Be("John");
        result.surname.Should().Be("Smith");
        result.pesel.Should().Be("12345678901");
    }

    [Fact]
    public async Task GetCitizenById_SecondCitizen_ShouldReturnCorrectData()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");

        // Act
        var response = await client.GetAsync("/api/citizens/2");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<CitizenDetailsDto>();
        result.Should().NotBeNull();
        result!.name.Should().Be("Anna");
        result.surname.Should().Be("Johnson");
        result.pesel.Should().Be("98765432101");
    }

    [Fact]
    public async Task GetCitizenById_NonExistentId_ShouldReturnNotFound()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");

        // Act
        var response = await client.GetAsync("/api/citizens/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetCitizenById_DifferentWorkers_ShouldReturnSameData()
    {
        // Arrange
        var client1 = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");
        var client2 = _factory.CreateClientWithAuth("worker", "test-worker-uid-2");

        // Act
        var response1 = await client1.GetAsync("/api/citizens/1");
        var result1 = await response1.Content.ReadFromJsonAsync<CitizenDetailsDto>();

        var response2 = await client2.GetAsync("/api/citizens/1");
        var result2 = await response2.Content.ReadFromJsonAsync<CitizenDetailsDto>();

        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        response2.StatusCode.Should().Be(HttpStatusCode.OK);
        result1.Should().NotBeNull();
        result2.Should().NotBeNull();
        result1!.pesel.Should().Be(result2!.pesel);
        result1.name.Should().Be(result2.name);
    }

    #endregion

    #region SearchCitizens Tests

    [Fact]
    public async Task SearchCitizens_WithWorkerRole_ShouldReturnOk()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");

        // Act
        var response = await client.GetAsync("/api/citizens/search?PageNumber=1&PageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<SimpleCitizenDto>>();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task SearchCitizens_WithAdminRole_ShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/citizens/search?PageNumber=1&PageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task SearchCitizens_WithCitizenRole_ShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("citizen", "test-citizen-uid-1");

        // Act
        var response = await client.GetAsync("/api/citizens/search?PageNumber=1&PageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task SearchCitizens_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClientWithoutAuth();

        // Act
        var response = await client.GetAsync("/api/citizens/search?PageNumber=1&PageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task SearchCitizens_WithZeroPageNumber_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");

        // Act
        var response = await client.GetAsync("/api/citizens/search?PageNumber=0&PageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SearchCitizens_WithZeroPageSize_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");

        // Act
        var response = await client.GetAsync("/api/citizens/search?PageNumber=1&PageSize=0");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SearchCitizens_WithNegativePageNumber_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");

        // Act
        var response = await client.GetAsync("/api/citizens/search?PageNumber=-1&PageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SearchCitizens_WithPageSizeOver100_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");

        // Act
        var response = await client.GetAsync("/api/citizens/search?PageNumber=1&PageSize=101");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SearchCitizens_ByName_ShouldReturnMatchingResults()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");

        // Act
        var response = await client.GetAsync("/api/citizens/search?PageNumber=1&PageSize=10&Name=John");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<SimpleCitizenDto>>();
        result.Should().NotBeNull();
        result!.Items.Should().Contain(c => c.Name == "John");
    }

    [Fact]
    public async Task SearchCitizens_BySurname_ShouldReturnMatchingResults()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");

        // Act
        var response = await client.GetAsync("/api/citizens/search?PageNumber=1&PageSize=10&Surname=Smith");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<SimpleCitizenDto>>();
        result.Should().NotBeNull();
        result!.Items.Should().Contain(c => c.Surname == "Smith");
    }

    [Fact]
    public async Task SearchCitizens_ByPesel_ShouldReturnMatchingResults()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");

        // Act
        var response = await client.GetAsync("/api/citizens/search?PageNumber=1&PageSize=10&Pesel=12345678901");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<SimpleCitizenDto>>();
        result.Should().NotBeNull();
        result!.Items.Should().Contain(c => c.Pesel == "12345678901");
    }

    [Fact]
    public async Task SearchCitizens_NoMatchingResults_ShouldReturnEmptyList()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");

        // Act
        var response = await client.GetAsync("/api/citizens/search?PageNumber=1&PageSize=10&Name=NonExistent");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<SimpleCitizenDto>>();
        result.Should().NotBeNull();
        result!.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task SearchCitizens_ShouldReturnCorrectPagedResult()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");

        // Act
        var response = await client.GetAsync("/api/citizens/search?PageNumber=1&PageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<SimpleCitizenDto>>();
        result.Should().NotBeNull();
        result!.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalCount.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task SearchCitizens_PageSize1_ShouldReturnOnlyOneItem()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");

        // Act
        var response = await client.GetAsync("/api/citizens/search?PageNumber=1&PageSize=1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<SimpleCitizenDto>>();
        result.Should().NotBeNull();
        result!.Items.Count.Should().BeInRange(0, 1);
    }

    [Fact]
    public async Task SearchCitizens_PartialNameMatch_ShouldReturnResults()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");

        // Act
        var response = await client.GetAsync("/api/citizens/search?PageNumber=1&PageSize=10&Name=Joh");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<SimpleCitizenDto>>();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task SearchCitizens_CaseInsensitiveName_ShouldReturnResults()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");

        // Act
        var responseLower = await client.GetAsync("/api/citizens/search?PageNumber=1&PageSize=10&Name=john");
        var responseUpper = await client.GetAsync("/api/citizens/search?PageNumber=1&PageSize=10&Name=JOHN");

        // Assert
        responseLower.StatusCode.Should().Be(HttpStatusCode.OK);
        responseUpper.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region GetCitizenSubmissionStatus Tests

    [Fact]
    public async Task GetCitizenSubmissionStatus_WithCitizenRole_ShouldReturnOk()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("citizen", "test-citizen-uid-1");

        // Act
        var response = await client.GetAsync("/api/citizens/submission-status");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<CitizenSubmissionStatusDto>();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetCitizenSubmissionStatus_WithAdminRole_ShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/citizens/submission-status");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetCitizenSubmissionStatus_WithWorkerRole_ShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");

        // Act
        var response = await client.GetAsync("/api/citizens/submission-status");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetCitizenSubmissionStatus_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClientWithoutAuth();

        // Act
        var response = await client.GetAsync("/api/citizens/submission-status");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetCitizenSubmissionStatus_ShouldReturnValidStatus()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("citizen", "test-citizen-uid-1");

        // Act
        var response = await client.GetAsync("/api/citizens/submission-status");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<CitizenSubmissionStatusDto>();
        result.Should().NotBeNull();
        result!.submissionStatus.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetCitizenSubmissionStatus_MultipleCallsWithSameToken_ShouldReturnConsistentData()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("citizen", "test-citizen-uid-1");

        // Act
        var response1 = await client.GetAsync("/api/citizens/submission-status");
        var result1 = await response1.Content.ReadFromJsonAsync<CitizenSubmissionStatusDto>();

        var response2 = await client.GetAsync("/api/citizens/submission-status");
        var result2 = await response2.Content.ReadFromJsonAsync<CitizenSubmissionStatusDto>();

        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        response2.StatusCode.Should().Be(HttpStatusCode.OK);
        result1.Should().NotBeNull();
        result2.Should().NotBeNull();
        result1!.submissionStatus.Should().Be(result2!.submissionStatus);
    }

    [Fact]
    public async Task GetCitizenSubmissionStatus_UnknownUid_ShouldReturnOkWithDefaultStatus()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("citizen", "unknown-citizen-uid");

        // Act
        var response = await client.GetAsync("/api/citizens/submission-status");

        // Assert
        // Note: The API returns OK with default status for unknown users
        // instead of NotFound - this is the current behavior
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<CitizenSubmissionStatusDto>();
        result.Should().NotBeNull();
    }

    #endregion

    #region CreateCitizen Tests

    [Fact]
    public async Task CreateCitizen_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClientWithoutAuth();
        var citizenDto = new CitizenDetailsDto
        {
            name = "Test",
            surname = "User",
            email = "test@test.com",
            dateOfBirth = new DateTime(1990, 1, 1),
            creationDate = DateTime.UtcNow,
            pesel = "11111111111",
            facilityName = "Test Clinic 1",
            submissionStatus = "NotUploaded"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/citizens/new-citizen-details", citizenDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateCitizen_WithExistingProfile_ShouldReturnConflict()
    {
        // Arrange - user that already has a citizen profile
        var client = _factory.CreateClientWithAuth("citizen", "test-citizen-uid-1");
        var citizenDto = new CitizenDetailsDto
        {
            name = "Test",
            surname = "User",
            email = "test@test.com",
            dateOfBirth = new DateTime(1990, 1, 1),
            creationDate = DateTime.UtcNow,
            pesel = "11111111111",
            facilityName = "Test Clinic 1",
            submissionStatus = "NotUploaded"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/citizens/new-citizen-details", citizenDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    #endregion

    #region RegisterDeviceToken Tests

    [Fact]
    public async Task RegisterDeviceToken_WithValidData_ShouldReturnOk()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("citizen", "test-citizen-uid-1");
        var deviceTokenRequest = new { deviceToken = "test-device-token-123", language = "pl" };

        // Act
        var response = await client.PostAsJsonAsync("/api/citizens/device-token", deviceTokenRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RegisterDeviceToken_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClientWithoutAuth();
        var deviceTokenRequest = new { deviceToken = "test-device-token-123", language = "pl" };

        // Act
        var response = await client.PostAsJsonAsync("/api/citizens/device-token", deviceTokenRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RegisterDeviceToken_MultipleRegistrations_ShouldSucceed()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("citizen", "test-citizen-uid-1");
        var deviceTokenRequest1 = new { deviceToken = "test-device-token-456", language = "pl" };
        var deviceTokenRequest2 = new { deviceToken = "test-device-token-789", language = "en" };

        // Act
        var response1 = await client.PostAsJsonAsync("/api/citizens/device-token", deviceTokenRequest1);
        var response2 = await client.PostAsJsonAsync("/api/citizens/device-token", deviceTokenRequest2);

        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        response2.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion
}
