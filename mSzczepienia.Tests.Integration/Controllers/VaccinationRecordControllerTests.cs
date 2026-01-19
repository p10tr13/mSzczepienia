using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.DTO.VaccinationRecords;
using mSzczepienia.Tests.Integration.Fixtures;
using System.Net;

namespace mSzczepienia.Tests.Integration.Controllers;

public class VaccinationRecordControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public VaccinationRecordControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    #region SearchVaccinesAsync (History) Tests - [citizen]

    [Fact]
    public async Task SearchVaccinesAsync_WithCitizenRole_ShouldReturnOk()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("citizen", "test-citizen-uid-1");
        var query = new GetVaccinationRecordOverviewQuery
        {
            pageNumber = 1,
            pageSize = 10,
            sortingField = "vaccinationDate",
            sortingDirection = "desc"
        };

        // Act
        var response = await client.GetAsync($"/api/vaccination-records/history?pageNumber={query.pageNumber}&pageSize={query.pageSize}&sortingField={query.sortingField}&sortingDirection={query.sortingDirection}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<VaccinationRecordOverviewDto>>();
        result.Should().NotBeNull();
        result!.Items.Should().NotBeEmpty();
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task SearchVaccinesAsync_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClientWithoutAuth();

        // Act
        var response = await client.GetAsync("/api/vaccination-records/history?pageNumber=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task SearchVaccinesAsync_WithoutCitizenRole_ShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker");

        // Act
        var response = await client.GetAsync("/api/vaccination-records/history?pageNumber=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task SearchVaccinesAsync_WithInvalidPageNumber_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("citizen", "test-citizen-uid-1");

        // Act
        var response = await client.GetAsync("/api/vaccination-records/history?pageNumber=0&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SearchVaccinesAsync_WithPageSizeGreaterThan100_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("citizen", "test-citizen-uid-1");

        // Act
        var response = await client.GetAsync("/api/vaccination-records/history?pageNumber=1&pageSize=101");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SearchVaccinesAsync_ShouldReturnCorrectDtoStructure()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("citizen", "test-citizen-uid-1");

        // Act
        var response = await client.GetAsync("/api/vaccination-records/history?pageNumber=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<VaccinationRecordOverviewDto>>();
        result.Should().NotBeNull();
        
        if (result!.Items.Any())
        {
            var item = result.Items.First();
            item.id.Should().BeGreaterThan(0);
            item.vaccineName.Should().NotBeNullOrEmpty();
            item.vaccinationDate.Should().NotBe(default(DateTime));
            item.dosesNumber.Should().BeGreaterThan(0);
        }
    }

    #endregion

    #region GetVaccinationRecordsDetailsAsync Tests - [citizen]

    [Fact]
    public async Task GetVaccinationRecordsDetailsAsync_WithCitizenRole_ValidVaccineId_ShouldReturnOk()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("citizen", "test-citizen-uid-1");
        var vaccineId = 1;

        // Act
        var response = await client.GetAsync($"/api/vaccination-records/details?vaccineId={vaccineId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<VaccinationRecordDetailsDto>>();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetVaccinationRecordsDetailsAsync_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClientWithoutAuth();

        // Act
        var response = await client.GetAsync("/api/vaccination-records/details?vaccineId=1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetVaccinationRecordsDetailsAsync_WithoutCitizenRole_ShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin");

        // Act
        var response = await client.GetAsync("/api/vaccination-records/details?vaccineId=1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetVaccinationRecordsDetailsAsync_WithInvalidVaccineId_ShouldReturnEmptyList()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("citizen", "test-citizen-uid-1");
        var invalidVaccineId = 9999;

        // Act
        var response = await client.GetAsync($"/api/vaccination-records/details?vaccineId={invalidVaccineId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<VaccinationRecordDetailsDto>>();
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetVaccinationRecordsDetailsAsync_ShouldReturnCorrectDtoStructure()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("citizen", "test-citizen-uid-1");

        // Act
        var response = await client.GetAsync("/api/vaccination-records/details?vaccineId=1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<VaccinationRecordDetailsDto>>();
        result.Should().NotBeNull();
        
        if (result!.Any())
        {
            var item = result.First();
            item.vaccineId.Should().Be(1);
            item.vaccineName.Should().NotBeNullOrEmpty();
            item.manufacturer.Should().NotBeNullOrEmpty();
            item.vaccinationDate.Should().NotBe(default(DateTime));
            item.doseNumber.Should().NotBeNullOrEmpty();
            item.batch.Should().NotBeNullOrEmpty();
        }
    }

    #endregion

    #region GetVaccinationRecordsByCitizenAsync Tests - [worker]

    [Fact]
    public async Task GetVaccinationRecordsByCitizenAsync_WithWorkerRole_ValidCitizenId_ShouldReturnOk()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");
        var citizenId = 1;

        // Act
        var response = await client.GetAsync($"/api/vaccination-records/by-citizen?CitizenId={citizenId}&PageNumber=1&PageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<VaccinationRecordSimpleListItemDto>>();
        result.Should().NotBeNull();
        result!.PageNumber.Should().Be(1);
    }

    [Fact]
    public async Task GetVaccinationRecordsByCitizenAsync_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClientWithoutAuth();

        // Act
        var response = await client.GetAsync("/api/vaccination-records/by-citizen?CitizenId=1&PageNumber=1&PageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetVaccinationRecordsByCitizenAsync_WithoutWorkerRole_ShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("citizen", "test-citizen-uid-1");

        // Act
        var response = await client.GetAsync("/api/vaccination-records/by-citizen?CitizenId=1&PageNumber=1&PageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetVaccinationRecordsByCitizenAsync_WithInvalidPageNumber_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker");

        // Act
        var response = await client.GetAsync("/api/vaccination-records/by-citizen?CitizenId=1&PageNumber=0&PageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetVaccinationRecordsByCitizenAsync_WithPageSizeGreaterThan100_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker");

        // Act
        var response = await client.GetAsync("/api/vaccination-records/by-citizen?CitizenId=1&PageNumber=1&PageSize=101");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetVaccinationRecordsByCitizenAsync_ShouldReturnCorrectDtoStructure()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");

        // Act
        var response = await client.GetAsync("/api/vaccination-records/by-citizen?CitizenId=1&PageNumber=1&PageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<VaccinationRecordSimpleListItemDto>>();
        result.Should().NotBeNull();
        
        if (result!.Items.Any())
        {
            var item = result.Items.First();
            item.RecordId.Should().BeGreaterThan(0);
            item.VaccineName.Should().NotBeNullOrEmpty();
            item.Manufacturer.Should().NotBeNullOrEmpty();
            item.VaccinationDate.Should().NotBe(default(DateTime));
        }
    }

    #endregion

    #region AddVaccinationRecord Tests - [worker]

    [Fact]
    public async Task AddVaccinationRecord_WithWorkerRole_ValidData_ShouldReturnCreated()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");
        var dto = new CreateVaccinationRecordDto
        {
            citizenId = 1,
            vaccineId = 1,
            vaccinationDate = DateTime.UtcNow.AddDays(-5),
            doseNumber = "3",
            batch = "BATCH100",
            nextVaccinationDate = null,
            sourceEntryId = null,
            remarks = "Test record"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/vaccination-records/new", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task AddVaccinationRecord_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClientWithoutAuth();
        var dto = new CreateVaccinationRecordDto
        {
            citizenId = 1,
            vaccineId = 1,
            vaccinationDate = DateTime.UtcNow,
            doseNumber = "1",
            batch = "BATCH"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/vaccination-records/new", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AddVaccinationRecord_WithoutWorkerRole_ShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("citizen", "test-citizen-uid-1");
        var dto = new CreateVaccinationRecordDto
        {
            citizenId = 1,
            vaccineId = 1,
            vaccinationDate = DateTime.UtcNow,
            doseNumber = "1",
            batch = "BATCH"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/vaccination-records/new", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AddVaccinationRecord_WithInvalidCitizenId_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker");
        var dto = new CreateVaccinationRecordDto
        {
            citizenId = 0,
            vaccineId = 1,
            vaccinationDate = DateTime.UtcNow,
            doseNumber = "1",
            batch = "BATCH"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/vaccination-records/new", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddVaccinationRecord_WithInvalidVaccineId_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker");
        var dto = new CreateVaccinationRecordDto
        {
            citizenId = 1,
            vaccineId = 0,
            vaccinationDate = DateTime.UtcNow,
            doseNumber = "1",
            batch = "BATCH"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/vaccination-records/new", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddVaccinationRecord_WithNonexistentCitizen_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker");
        var dto = new CreateVaccinationRecordDto
        {
            citizenId = 9999,
            vaccineId = 1,
            vaccinationDate = DateTime.UtcNow,
            doseNumber = "1",
            batch = "BATCH"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/vaccination-records/new", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddVaccinationRecord_WithNonexistentVaccine_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker");
        var dto = new CreateVaccinationRecordDto
        {
            citizenId = 1,
            vaccineId = 9999,
            vaccinationDate = DateTime.UtcNow,
            doseNumber = "1",
            batch = "BATCH"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/vaccination-records/new", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region GetVaccinationRecordForEditAsync Tests - [worker]

    [Fact]
    public async Task GetVaccinationRecordForEditAsync_WithWorkerRole_ValidId_ShouldReturnOk()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");
        var recordId = 1;

        // Act
        var response = await client.GetAsync($"/api/vaccination-records/{recordId}/edit");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<VaccinationRecordEditDto>();
        result.Should().NotBeNull();
        result!.recordId.Should().Be(recordId);
    }

    [Fact]
    public async Task GetVaccinationRecordForEditAsync_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClientWithoutAuth();

        // Act
        var response = await client.GetAsync("/api/vaccination-records/1/edit");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetVaccinationRecordForEditAsync_WithoutWorkerRole_ShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("citizen", "test-citizen-uid-1");

        // Act
        var response = await client.GetAsync("/api/vaccination-records/1/edit");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetVaccinationRecordForEditAsync_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker");

        // Act
        var response = await client.GetAsync("/api/vaccination-records/9999/edit");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetVaccinationRecordForEditAsync_ShouldReturnCorrectDtoStructure()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");

        // Act
        var response = await client.GetAsync("/api/vaccination-records/1/edit");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<VaccinationRecordEditDto>();
        result.Should().NotBeNull();
        result!.recordId.Should().Be(1);
        result.citizenId.Should().BeGreaterThan(0);
        result.name.Should().NotBeNullOrEmpty();
        result.surname.Should().NotBeNullOrEmpty();
        result.pesel.Should().NotBeNullOrEmpty();
        result.vaccineId.Should().BeGreaterThan(0);
        result.vaccineName.Should().NotBeNullOrEmpty();
        result.doseNumber.Should().NotBeNullOrEmpty();
        result.batch.Should().NotBeNullOrEmpty();
    }

    #endregion

    #region UpdateVaccinationRecord Tests - [worker]

    [Fact]
    public async Task UpdateVaccinationRecord_WithWorkerRole_ValidId_ShouldReturnNoContent()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");
        var recordId = 1;
        var dto = new UpdateVaccinationRecordDto
        {
            vaccinationDate = DateTime.UtcNow.AddDays(-25),
            doseNumber = "1",
            batch = "BATCH_UPDATED",
            nextVaccinationDate = DateTime.UtcNow.AddDays(21),
            remarks = "Updated remarks"
        };

        // Act
        var response = await client.PutAsJsonAsync($"/api/vaccination-records/{recordId}", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task UpdateVaccinationRecord_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClientWithoutAuth();
        var dto = new UpdateVaccinationRecordDto
        {
            vaccinationDate = DateTime.UtcNow,
            doseNumber = "1",
            batch = "BATCH"
        };

        // Act
        var response = await client.PutAsJsonAsync("/api/vaccination-records/1", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateVaccinationRecord_WithoutWorkerRole_ShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("citizen", "test-citizen-uid-1");
        var dto = new UpdateVaccinationRecordDto
        {
            vaccinationDate = DateTime.UtcNow,
            doseNumber = "1",
            batch = "BATCH"
        };

        // Act
        var response = await client.PutAsJsonAsync("/api/vaccination-records/1", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateVaccinationRecord_WithInvalidId_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker");
        var dto = new UpdateVaccinationRecordDto
        {
            vaccinationDate = DateTime.UtcNow,
            doseNumber = "1",
            batch = "BATCH"
        };

        // Act
        var response = await client.PutAsJsonAsync("/api/vaccination-records/9999", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateVaccinationRecord_ShouldUpdateAllFields()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");
        var recordId = 1;
        var newDate = DateTime.UtcNow.AddDays(-50);
        var dto = new UpdateVaccinationRecordDto
        {
            vaccinationDate = newDate,
            doseNumber = "99",
            batch = "BATCH_NEW_99",
            nextVaccinationDate = DateTime.UtcNow.AddDays(14),
            remarks = "Complete update test"
        };

        // Act - Update
        var updateResponse = await client.PutAsJsonAsync($"/api/vaccination-records/{recordId}", dto);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Act - Verify update
        var getResponse = await client.GetAsync($"/api/vaccination-records/{recordId}/edit");

        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await getResponse.Content.ReadFromJsonAsync<VaccinationRecordEditDto>();
        result.Should().NotBeNull();
        result!.doseNumber.Should().Be("99");
        result.batch.Should().Be("BATCH_NEW_99");
        result.remarks.Should().Be("Complete update test");
    }

    #endregion

    #region DeleteVaccinationRecord Tests - [worker]

    [Fact]
    public async Task DeleteVaccinationRecord_WithWorkerRole_ValidId_ShouldReturnNoContent()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");
        var recordId = 5;

        // Act
        var response = await client.DeleteAsync($"/api/vaccination-records/{recordId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteVaccinationRecord_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClientWithoutAuth();

        // Act
        var response = await client.DeleteAsync("/api/vaccination-records/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteVaccinationRecord_WithoutWorkerRole_ShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("citizen", "test-citizen-uid-1");

        // Act
        var response = await client.DeleteAsync("/api/vaccination-records/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteVaccinationRecord_WithInvalidId_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker");

        // Act
        var response = await client.DeleteAsync("/api/vaccination-records/9999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteVaccinationRecord_ShouldDeleteSuccessfully()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");
        var recordId = 3; // Use record 3 instead of 5

        // Act - Delete
        var deleteResponse = await client.DeleteAsync($"/api/vaccination-records/{recordId}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Act - Try to get deleted record
        var getResponse = await client.GetAsync($"/api/vaccination-records/{recordId}/edit");

        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Authorization Tests

    [Fact]
    public async Task AllEndpoints_WithAdminRole_ShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin");

        // Act - Try citizen endpoint
        var response1 = await client.GetAsync("/api/vaccination-records/history?pageNumber=1&pageSize=10");
        response1.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        // Act - Try worker endpoint
        var response2 = await client.GetAsync("/api/vaccination-records/by-citizen?CitizenId=1&PageNumber=1&PageSize=10");
        response2.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    #endregion
}
