using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Personnel;
using mSzczepienia.Shared.Enums;
using mSzczepienia.Tests.Integration.Fixtures;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace mSzczepienia.Tests.Integration.Controllers;

public class PersonnelControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public PersonnelControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    #region GetPersonnel Tests

    [Fact]
    public async Task GetPersonnel_WithAdminRole_ShouldReturnOk()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/personnel?FacilityId=1&PageNumber=1&PageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<PersonnelDto>>();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetPersonnel_WithWorkerRole_ShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");

        // Act
        var response = await client.GetAsync("/api/personnel?FacilityId=1&PageNumber=1&PageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetPersonnel_WithCitizenRole_ShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("citizen", "test-citizen-uid-1");

        // Act
        var response = await client.GetAsync("/api/personnel?FacilityId=1&PageNumber=1&PageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetPersonnel_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClientWithoutAuth();

        // Act
        var response = await client.GetAsync("/api/personnel?FacilityId=1&PageNumber=1&PageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetPersonnel_WithZeroPageNumber_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/personnel?FacilityId=1&PageNumber=0&PageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetPersonnel_WithNegativePageNumber_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/personnel?FacilityId=1&PageNumber=-1&PageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetPersonnel_WithZeroPageSize_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/personnel?FacilityId=1&PageNumber=1&PageSize=0");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetPersonnel_WithNegativePageSize_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/personnel?FacilityId=1&PageNumber=1&PageSize=-5");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetPersonnel_WithPageSizeOver100_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/personnel?FacilityId=1&PageNumber=1&PageSize=101");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetPersonnel_WithPageSizeExactly100_ShouldReturnOk()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/personnel?FacilityId=1&PageNumber=1&PageSize=100");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetPersonnel_WithZeroFacilityId_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/personnel?FacilityId=0&PageNumber=1&PageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetPersonnel_WithNegativeFacilityId_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/personnel?FacilityId=-1&PageNumber=1&PageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetPersonnel_ForFacility1_ShouldReturnPersonnelFromFacility1()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/personnel?FacilityId=1&PageNumber=1&PageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<PersonnelDto>>();
        result.Should().NotBeNull();
        result!.Items.Should().AllSatisfy(p => p.FacilityId.Should().Be(1));
    }

    [Fact]
    public async Task GetPersonnel_ForFacility2_ShouldReturnPersonnelFromFacility2()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/personnel?FacilityId=2&PageNumber=1&PageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<PersonnelDto>>();
        result.Should().NotBeNull();
        result!.Items.Should().AllSatisfy(p => p.FacilityId.Should().Be(2));
    }

    [Fact]
    public async Task GetPersonnel_ForNonExistentFacility_ShouldReturnEmptyList()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/personnel?FacilityId=999&PageNumber=1&PageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<PersonnelDto>>();
        result.Should().NotBeNull();
        result!.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task GetPersonnel_ShouldContainPaginationInfo()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/personnel?FacilityId=1&PageNumber=1&PageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<PersonnelDto>>();
        result.Should().NotBeNull();
        result!.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalCount.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task GetPersonnel_ResultShouldContainRequiredFields()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/personnel?FacilityId=1&PageNumber=1&PageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<PersonnelDto>>();
        result.Should().NotBeNull();
        if (result!.Items.Any())
        {
            var firstPersonnel = result.Items.First();
            firstPersonnel.Id.Should().BeGreaterThan(0);
            firstPersonnel.AccountId.Should().NotBeEmpty();
            firstPersonnel.Name.Should().NotBeNullOrEmpty();
            firstPersonnel.Surname.Should().NotBeNullOrEmpty();
            firstPersonnel.Email.Should().NotBeNullOrEmpty();
            firstPersonnel.PwzNumber.Should().NotBeNullOrEmpty();
        }
    }

    [Fact]
    public async Task GetPersonnel_LargePageNumber_ShouldReturnEmptyList()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/personnel?FacilityId=1&PageNumber=100&PageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<PersonnelDto>>();
        result.Should().NotBeNull();
        result!.Items.Should().BeEmpty();
    }

    #endregion

    #region SearchPersonnel Tests

    [Fact]
    public async Task SearchPersonnel_WithAdminRole_ShouldReturnOk()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/personnel/search?Page=1&PageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<PersonnelDto>>();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task SearchPersonnel_WithWorkerRole_ShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");

        // Act
        var response = await client.GetAsync("/api/personnel/search?Page=1&PageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task SearchPersonnel_WithCitizenRole_ShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("citizen", "test-citizen-uid-1");

        // Act
        var response = await client.GetAsync("/api/personnel/search?Page=1&PageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task SearchPersonnel_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClientWithoutAuth();

        // Act
        var response = await client.GetAsync("/api/personnel/search?Page=1&PageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task SearchPersonnel_WithZeroPage_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/personnel/search?Page=0&PageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SearchPersonnel_WithNegativePage_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/personnel/search?Page=-1&PageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SearchPersonnel_WithZeroPageSize_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/personnel/search?Page=1&PageSize=0");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SearchPersonnel_WithNegativePageSize_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/personnel/search?Page=1&PageSize=-5");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SearchPersonnel_WithPageSizeOver100_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/personnel/search?Page=1&PageSize=101");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SearchPersonnel_WithPageSizeExactly100_ShouldReturnOk()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/personnel/search?Page=1&PageSize=100");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SearchPersonnel_ByFirstName_ShouldReturnFilteredResults()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/personnel/search?Page=1&PageSize=10&FirstName=Jan");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<PersonnelDto>>();
        result.Should().NotBeNull();
        result!.Items.Should().AllSatisfy(p => p.Name.Should().Contain("Jan"));
    }

    [Fact]
    public async Task SearchPersonnel_BySurname_ShouldReturnFilteredResults()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/personnel/search?Page=1&PageSize=10&Surname=Kowalski");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<PersonnelDto>>();
        result.Should().NotBeNull();
        result!.Items.Should().AllSatisfy(p => p.Surname.Should().Contain("Kowalski"));
    }

    [Fact]
    public async Task SearchPersonnel_ByEmail_ShouldReturnFilteredResults()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/personnel/search?Page=1&PageSize=10&Email=worker1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<PersonnelDto>>();
        result.Should().NotBeNull();
        result!.Items.Should().AllSatisfy(p => p.Email.Should().Contain("worker1"));
    }

    [Fact]
    public async Task SearchPersonnel_ByFacilityCity_ShouldReturnFilteredResults()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/personnel/search?Page=1&PageSize=10&FacilityCity=Warsaw");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<PersonnelDto>>();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task SearchPersonnel_ByFacilityName_ShouldReturnFilteredResults()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/personnel/search?Page=1&PageSize=10&FacilityName=Test%20Clinic");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<PersonnelDto>>();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task SearchPersonnel_WithNonMatchingFilters_ShouldReturnEmptyResults()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/personnel/search?Page=1&PageSize=10&FirstName=NonExistent");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<PersonnelDto>>();
        result.Should().NotBeNull();
        result!.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task SearchPersonnel_WithMultipleFilters_ShouldReturnOk()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/personnel/search?Page=1&PageSize=10&FirstName=Jan&Surname=Kowalski");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<PersonnelDto>>();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task SearchPersonnel_ResultShouldContainPaginationInfo()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/personnel/search?Page=1&PageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<PersonnelDto>>();
        result.Should().NotBeNull();
        result!.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalCount.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task SearchPersonnel_LargePageNumber_ShouldReturnEmptyResults()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/personnel/search?Page=100&PageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<PersonnelDto>>();
        result.Should().NotBeNull();
        result!.Items.Should().BeEmpty();
    }

    #endregion

    #region GetDetailedPersonnelById Tests

    [Fact]
    public async Task GetDetailedPersonnelById_WithAdminRole_ExistingPersonnel_ShouldReturnOk()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/personnel/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<DetailedPersonnelDto>();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetDetailedPersonnelById_WithWorkerRole_ShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");

        // Act
        var response = await client.GetAsync("/api/personnel/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetDetailedPersonnelById_WithCitizenRole_ShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("citizen", "test-citizen-uid-1");

        // Act
        var response = await client.GetAsync("/api/personnel/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetDetailedPersonnelById_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClientWithoutAuth();

        // Act
        var response = await client.GetAsync("/api/personnel/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetDetailedPersonnelById_WithZeroId_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/personnel/0");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetDetailedPersonnelById_WithNegativeId_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/personnel/-1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetDetailedPersonnelById_NonExistentPersonnel_ShouldReturnNotFound()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/personnel/9999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetDetailedPersonnelById_ShouldReturnCorrectData()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/personnel/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<DetailedPersonnelDto>();
        result.Should().NotBeNull();
        result!.Name.Should().Be("Jan");
        result.Surname.Should().Be("Kowalski");
        result.Email.Should().Be("worker1@test.com");
        result.PwzNumber.Should().Be("PWZ123456");
    }

    [Fact]
    public async Task GetDetailedPersonnelById_ShouldContainFacilityInfo()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/personnel/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<DetailedPersonnelDto>();
        result.Should().NotBeNull();
        result!.FacilityId.Should().BeGreaterThan(0);
        result.FacilityName.Should().NotBeNullOrEmpty();
        result.FacilityCity.Should().NotBeNullOrEmpty();
        result.FacilityStreet.Should().NotBeNullOrEmpty();
        result.FacilityPostalCode.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetDetailedPersonnelById_ShouldReturnAllFields()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/personnel/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<DetailedPersonnelDto>();
        result.Should().NotBeNull();
        result!.Id.Should().NotBeEmpty();
        result.Name.Should().NotBeNullOrEmpty();
        result.Surname.Should().NotBeNullOrEmpty();
        result.Email.Should().NotBeNullOrEmpty();
        result.CreationDate.Should().NotBe(default(DateTime));
        result.DateOfBirth.Should().NotBe(default(DateTime));
        result.PwzNumber.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetDetailedPersonnelById_Personnel2_ShouldReturnCorrectData()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/personnel/2");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<DetailedPersonnelDto>();
        result.Should().NotBeNull();
        result!.Name.Should().Be("Maria");
        result.Surname.Should().Be("Nowak");
        result.Email.Should().Be("worker2@test.com");
        result.PwzNumber.Should().Be("PWZ654321");
    }

    #endregion

    #region UpdatePersonnel Tests

    [Fact]
    public async Task UpdatePersonnel_WithAdminRole_ExistingPersonnel_ShouldReturnNoContent()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");
        var updateDto = new UpdatePersonnelDto
        {
            Name = "JanUpdated",
            Surname = "KowalskiUpdated",
            Email = "worker1@test.com",
            DateOfBirth = new DateTime(1985, 5, 15),
            PwzNumber = "PWZ123456",
            Position = Position.Nurse
        };
        var content = new StringContent(JsonSerializer.Serialize(updateDto), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PutAsync("/api/personnel/1", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task UpdatePersonnel_WithWorkerRole_ShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");
        var updateDto = new UpdatePersonnelDto
        {
            Name = "JanUpdated",
            Surname = "KowalskiUpdated",
            Email = "worker1@test.com",
            DateOfBirth = new DateTime(1985, 5, 15),
            PwzNumber = "PWZ123456",
            Position = Position.Nurse
        };
        var content = new StringContent(JsonSerializer.Serialize(updateDto), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PutAsync("/api/personnel/1", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdatePersonnel_WithCitizenRole_ShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("citizen", "test-citizen-uid-1");
        var updateDto = new UpdatePersonnelDto
        {
            Name = "JanUpdated",
            Surname = "KowalskiUpdated",
            Email = "worker1@test.com",
            DateOfBirth = new DateTime(1985, 5, 15),
            PwzNumber = "PWZ123456",
            Position = Position.Nurse
        };
        var content = new StringContent(JsonSerializer.Serialize(updateDto), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PutAsync("/api/personnel/1", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdatePersonnel_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClientWithoutAuth();
        var updateDto = new UpdatePersonnelDto
        {
            Name = "JanUpdated",
            Surname = "KowalskiUpdated",
            Email = "worker1@test.com",
            DateOfBirth = new DateTime(1985, 5, 15),
            PwzNumber = "PWZ123456",
            Position = Position.Nurse
        };
        var content = new StringContent(JsonSerializer.Serialize(updateDto), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PutAsync("/api/personnel/1", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdatePersonnel_WithZeroId_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");
        var updateDto = new UpdatePersonnelDto
        {
            Name = "JanUpdated",
            Surname = "KowalskiUpdated",
            Email = "worker1@test.com",
            DateOfBirth = new DateTime(1985, 5, 15),
            PwzNumber = "PWZ123456",
            Position = Position.Nurse
        };
        var content = new StringContent(JsonSerializer.Serialize(updateDto), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PutAsync("/api/personnel/0", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdatePersonnel_WithNegativeId_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");
        var updateDto = new UpdatePersonnelDto
        {
            Name = "JanUpdated",
            Surname = "KowalskiUpdated",
            Email = "worker1@test.com",
            DateOfBirth = new DateTime(1985, 5, 15),
            PwzNumber = "PWZ123456",
            Position = Position.Nurse
        };
        var content = new StringContent(JsonSerializer.Serialize(updateDto), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PutAsync("/api/personnel/-1", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdatePersonnel_NonExistentPersonnel_ShouldReturnNotFound()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");
        var updateDto = new UpdatePersonnelDto
        {
            Name = "JanUpdated",
            Surname = "KowalskiUpdated",
            Email = "nonexistent@test.com",
            DateOfBirth = new DateTime(1985, 5, 15),
            PwzNumber = "PWZ999999",
            Position = Position.Nurse
        };
        var content = new StringContent(JsonSerializer.Serialize(updateDto), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PutAsync("/api/personnel/9999", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdatePersonnel_ShouldUpdateNameAndSurname()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");
        var updateDto = new UpdatePersonnelDto
        {
            Name = "UpdatedFirstName",
            Surname = "UpdatedLastName",
            Email = "worker2@test.com",
            DateOfBirth = new DateTime(1990, 8, 20),
            PwzNumber = "PWZ654321",
            Position = Position.Doctor
        };
        var content = new StringContent(JsonSerializer.Serialize(updateDto), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PutAsync("/api/personnel/2", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify the update
        var getResponse = await client.GetAsync("/api/personnel/2");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await getResponse.Content.ReadFromJsonAsync<DetailedPersonnelDto>();
        result.Should().NotBeNull();
        result!.Name.Should().Be("UpdatedFirstName");
        result.Surname.Should().Be("UpdatedLastName");
    }

    [Fact]
    public async Task UpdatePersonnel_ShouldUpdatePwzNumber()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // First get current data
        var getResponse = await client.GetAsync("/api/personnel/1");
        var currentData = await getResponse.Content.ReadFromJsonAsync<DetailedPersonnelDto>();

        var updateDto = new UpdatePersonnelDto
        {
            Name = currentData!.Name,
            Surname = currentData.Surname,
            Email = currentData.Email,
            DateOfBirth = currentData.DateOfBirth,
            PwzNumber = "NEWPWZ123",
            Position = currentData.Position
        };
        var content = new StringContent(JsonSerializer.Serialize(updateDto), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PutAsync("/api/personnel/1", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify the update
        var verifyResponse = await client.GetAsync("/api/personnel/1");
        verifyResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await verifyResponse.Content.ReadFromJsonAsync<DetailedPersonnelDto>();
        result.Should().NotBeNull();
        result!.PwzNumber.Should().Be("NEWPWZ123");
    }

    [Fact]
    public async Task UpdatePersonnel_ShouldUpdatePosition()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // First get current data
        var getResponse = await client.GetAsync("/api/personnel/1");
        var currentData = await getResponse.Content.ReadFromJsonAsync<DetailedPersonnelDto>();

        var updateDto = new UpdatePersonnelDto
        {
            Name = currentData!.Name,
            Surname = currentData.Surname,
            Email = currentData.Email,
            DateOfBirth = currentData.DateOfBirth,
            PwzNumber = currentData.PwzNumber,
            Position = Position.Doctor
        };
        var content = new StringContent(JsonSerializer.Serialize(updateDto), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PutAsync("/api/personnel/1", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify the update
        var verifyResponse = await client.GetAsync("/api/personnel/1");
        verifyResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await verifyResponse.Content.ReadFromJsonAsync<DetailedPersonnelDto>();
        result.Should().NotBeNull();
        result!.Position.Should().Be(Position.Doctor);
    }

    [Fact]
    public async Task UpdatePersonnel_WithDuplicateEmail_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");
        var updateDto = new UpdatePersonnelDto
        {
            Name = "Jan",
            Surname = "Kowalski",
            Email = "worker2@test.com", // This email belongs to personnel 2
            DateOfBirth = new DateTime(1985, 5, 15),
            PwzNumber = "PWZ123456",
            Position = Position.Nurse
        };
        var content = new StringContent(JsonSerializer.Serialize(updateDto), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PutAsync("/api/personnel/1", content);

        // Assert
        // Should return BadRequest because email is already in use by another account
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdatePersonnel_WithSameEmail_ShouldReturnNoContent()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // First get current data
        var getResponse = await client.GetAsync("/api/personnel/1");
        var currentData = await getResponse.Content.ReadFromJsonAsync<DetailedPersonnelDto>();

        var updateDto = new UpdatePersonnelDto
        {
            Name = "NewName",
            Surname = "NewSurname",
            Email = currentData!.Email, // Same email as before
            DateOfBirth = currentData.DateOfBirth,
            PwzNumber = currentData.PwzNumber,
            Position = currentData.Position
        };
        var content = new StringContent(JsonSerializer.Serialize(updateDto), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PutAsync("/api/personnel/1", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    #endregion

    #region Edge Cases and Additional Tests

    [Fact]
    public async Task GetPersonnel_MultipleCallsWithSameParameters_ShouldReturnConsistentResults()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response1 = await client.GetAsync("/api/personnel?FacilityId=1&PageNumber=1&PageSize=10");
        var result1 = await response1.Content.ReadFromJsonAsync<PagedResult<PersonnelDto>>();

        var response2 = await client.GetAsync("/api/personnel?FacilityId=1&PageNumber=1&PageSize=10");
        var result2 = await response2.Content.ReadFromJsonAsync<PagedResult<PersonnelDto>>();

        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        response2.StatusCode.Should().Be(HttpStatusCode.OK);
        result1.Should().NotBeNull();
        result2.Should().NotBeNull();
        result1!.TotalCount.Should().Be(result2!.TotalCount);
    }

    [Fact]
    public async Task SearchPersonnel_WithPartialFirstName_ShouldReturnFilteredResults()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/personnel/search?Page=1&PageSize=10&FirstName=Ja");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<PersonnelDto>>();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task SearchPersonnel_WithEmptyFilters_ShouldReturnAllPersonnel()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/personnel/search?Page=1&PageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<PersonnelDto>>();
        result.Should().NotBeNull();
        result!.TotalCount.Should().BeGreaterThanOrEqualTo(2); // At least 2 personnel in test data
    }

    [Fact]
    public async Task GetDetailedPersonnelById_DifferentAdmins_ShouldReturnSameData()
    {
        // Arrange
        var client1 = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");
        var client2 = _factory.CreateClientWithAuth("admin", "test-admin-uid-2");

        // Act
        var response1 = await client1.GetAsync("/api/personnel/1");
        var result1 = await response1.Content.ReadFromJsonAsync<DetailedPersonnelDto>();

        var response2 = await client2.GetAsync("/api/personnel/1");
        var result2 = await response2.Content.ReadFromJsonAsync<DetailedPersonnelDto>();

        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        response2.StatusCode.Should().Be(HttpStatusCode.OK);
        result1.Should().NotBeNull();
        result2.Should().NotBeNull();
        result1!.Email.Should().Be(result2!.Email);
        result1.Name.Should().Be(result2.Name);
    }

    [Fact]
    public async Task GetPersonnel_SmallPageSize_ShouldRespectPagination()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/personnel?FacilityId=1&PageNumber=1&PageSize=1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<PersonnelDto>>();
        result.Should().NotBeNull();
        result!.Items.Count.Should().BeInRange(0, 1);
        result.PageSize.Should().Be(1);
    }

    [Fact]
    public async Task SearchPersonnel_SmallPageSize_ShouldRespectPagination()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/personnel/search?Page=1&PageSize=1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<PersonnelDto>>();
        result.Should().NotBeNull();
        result!.Items.Count.Should().BeInRange(0, 1);
        result.PageSize.Should().Be(1);
    }

    #endregion
}
