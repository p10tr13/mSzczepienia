using System.Net;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Vaccines;
using mSzczepienia.Tests.Integration.Fixtures;

namespace mSzczepienia.Tests.Integration.Controllers;

[Collection("Vaccines Controller Collection")]
public class VaccinesControllerTests
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public VaccinesControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    #region GetManufacturersAutocomplete Tests

    [Fact]
    public async Task GetManufacturersAutocomplete_WithAdminRole_ShouldReturnOk()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin");

        // Act
        var response = await client.GetAsync("/api/vaccines/manufacturers-autocomplete?query=Pf");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<string>>();
        result.Should().NotBeNull();
        result.Should().Contain(m => m.Contains("Pfizer"));
    }

    [Fact]
    public async Task GetManufacturersAutocomplete_WithoutRole_ShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("user");

        // Act
        var response = await client.GetAsync("/api/vaccines/manufacturers-autocomplete?query=Pf");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetManufacturersAutocomplete_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClientWithoutAuth();

        // Act
        var response = await client.GetAsync("/api/vaccines/manufacturers-autocomplete?query=Pf");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetManufacturersAutocomplete_NoQuery_ShouldReturnAllManufacturers()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin");

        // Act
        var response = await client.GetAsync("/api/vaccines/manufacturers-autocomplete");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<string>>();
        result.Should().NotBeNull();
        result.Should().Contain("Pfizer", "Moderna", "Sanofi", "Merck");
    }

    [Fact]
    public async Task GetManufacturersAutocomplete_FilterByQuery_ShouldReturnFilteredResults()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin");

        // Act
        var response = await client.GetAsync("/api/vaccines/manufacturers-autocomplete?query=Mod");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<string>>();
        result.Should().NotBeNull();
        result.Should().Contain("Moderna");
        result.Should().NotContain("Pfizer");
    }

    #endregion

    #region GetNamesAutocomplete Tests

    [Fact]
    public async Task GetNamesAutocomplete_WithAdminRole_ShouldReturnOk()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin");

        // Act
        var response = await client.GetAsync("/api/vaccines/names-autocomplete?query=Pf");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<string>>();
        result.Should().NotBeNull();
        result.Should().Contain(n => n.Contains("Pfizer"));
    }

    [Fact]
    public async Task GetNamesAutocomplete_WithoutRole_ShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("user");

        // Act
        var response = await client.GetAsync("/api/vaccines/names-autocomplete?query=Pf");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetNamesAutocomplete_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClientWithoutAuth();

        // Act
        var response = await client.GetAsync("/api/vaccines/names-autocomplete?query=Pf");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetNamesAutocomplete_NoQuery_ShouldReturnAllNames()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin");

        // Act
        var response = await client.GetAsync("/api/vaccines/names-autocomplete");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<string>>();
        result.Should().NotBeNull();
        result.Should().HaveCountGreaterThanOrEqualTo(4);
    }

    [Fact]
    public async Task GetNamesAutocomplete_FilterByQuery_ShouldReturnFilteredResults()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin");

        // Act
        var response = await client.GetAsync("/api/vaccines/names-autocomplete?query=Flu");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<string>>();
        result.Should().NotBeNull();
        result.Should().Contain("Fluzone");
    }

    #endregion

    #region SearchVaccines Tests

    [Fact]
    public async Task SearchVaccines_WithAdminRole_ValidQuery_ShouldReturnOk()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin");

        // Act
        var response = await client.GetAsync("/api/vaccines/search?pageNumber=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<VaccineDto>>();
        result.Should().NotBeNull();
        result!.Items.Should().NotBeEmpty();
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task SearchVaccines_WithoutRole_ShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("user");

        // Act
        var response = await client.GetAsync("/api/vaccines/search?pageNumber=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task SearchVaccines_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClientWithoutAuth();

        // Act
        var response = await client.GetAsync("/api/vaccines/search?pageNumber=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task SearchVaccines_InvalidPageNumber_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin");

        // Act
        var response = await client.GetAsync("/api/vaccines/search?pageNumber=0&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SearchVaccines_InvalidPageSize_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin");

        // Act
        var response = await client.GetAsync("/api/vaccines/search?pageNumber=1&pageSize=101");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SearchVaccines_MaxPageSize_ShouldReturnOk()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin");

        // Act
        var response = await client.GetAsync("/api/vaccines/search?pageNumber=1&pageSize=100");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<VaccineDto>>();
        result.Should().NotBeNull();
        result!.PageSize.Should().Be(100);
    }

    [Fact]
    public async Task SearchVaccines_ByName_ShouldReturnFilteredResults()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin");

        // Act
        var response = await client.GetAsync("/api/vaccines/search?pageNumber=1&pageSize=10&name=Pfizer");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<VaccineDto>>();
        result.Should().NotBeNull();
        result!.Items.Should().NotBeEmpty();
        result.Items.Should().AllSatisfy(v => v.VaccineName.Should().Contain("Pfizer"));
    }

    [Fact]
    public async Task SearchVaccines_ByManufacturer_ShouldReturnFilteredResults()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin");

        // Act
        var response = await client.GetAsync("/api/vaccines/search?pageNumber=1&pageSize=10&manufacturer=Moderna");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<VaccineDto>>();
        result.Should().NotBeNull();
        result!.Items.Should().AllSatisfy(v => v.Manufacturer.Should().Be("Moderna"));
    }

    [Fact]
    public async Task SearchVaccines_ByDisease_ShouldReturnFilteredResults()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin");

        // Act
        var response = await client.GetAsync("/api/vaccines/search?pageNumber=1&pageSize=10&disease=COVID-19");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<VaccineDto>>();
        result.Should().NotBeNull();
        result!.Items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task SearchVaccines_ById_ShouldReturnFilteredResults()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin");

        // Act
        var response = await client.GetAsync("/api/vaccines/search?pageNumber=1&pageSize=10&id=1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<VaccineDto>>();
        result.Should().NotBeNull();
        result!.Items.Should().Contain(v => v.VaccineId == 1);
    }

    [Fact]
    public async Task SearchVaccines_MultipleFilters_ShouldReturnFilteredResults()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin");

        // Act
        var response = await client.GetAsync("/api/vaccines/search?pageNumber=1&pageSize=10&manufacturer=Pfizer&disease=COVID-19");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<VaccineDto>>();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task SearchVaccines_Pagination_ShouldContainCorrectMetadata()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin");

        // Act
        var response = await client.GetAsync("/api/vaccines/search?pageNumber=1&pageSize=2");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<VaccineDto>>();
        result.Should().NotBeNull();
        result!.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(2);
        result.TotalCount.Should().BeGreaterThanOrEqualTo(4);
        result.TotalPages.Should().Be((result.TotalCount + result.PageSize - 1) / result.PageSize);
    }

    [Fact]
    public async Task SearchVaccines_SecondPage_ShouldReturnDifferentResults()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin");

        // Act - Get first page
        var response1 = await client.GetAsync("/api/vaccines/search?pageNumber=1&pageSize=2");
        var result1 = await response1.Content.ReadFromJsonAsync<PagedResult<VaccineDto>>();

        // Act - Get second page
        var response2 = await client.GetAsync("/api/vaccines/search?pageNumber=2&pageSize=2");
        var result2 = await response2.Content.ReadFromJsonAsync<PagedResult<VaccineDto>>();

        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        response2.StatusCode.Should().Be(HttpStatusCode.OK);
        result1.Should().NotBeNull();
        result2.Should().NotBeNull();

        if (result1!.TotalCount > 2)
        {
            result1!.Items.First().VaccineId.Should().NotBe(result2!.Items.First().VaccineId);
        }
    }

    #endregion

    #region Authorization Tests

    [Fact]
    public async Task AllEndpoints_WithoutToken_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClientWithoutAuth();

        // Act & Assert
        var response1 = await client.GetAsync("/api/vaccines/manufacturers-autocomplete");
        var response2 = await client.GetAsync("/api/vaccines/names-autocomplete");
        var response3 = await client.GetAsync("/api/vaccines/search");

        response1.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        response2.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        response3.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AllEndpoints_WithUserRole_ShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("user");

        // Act & Assert
        var response1 = await client.GetAsync("/api/vaccines/manufacturers-autocomplete");
        var response2 = await client.GetAsync("/api/vaccines/names-autocomplete");
        var response3 = await client.GetAsync("/api/vaccines/search");

        response1.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        response2.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        response3.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AllEndpoints_WithAdminRole_ShouldReturnSuccess()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin");

        // Act & Assert
        var response1 = await client.GetAsync("/api/vaccines/manufacturers-autocomplete");
        var response2 = await client.GetAsync("/api/vaccines/names-autocomplete");
        var response3 = await client.GetAsync("/api/vaccines/search");

        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        response2.StatusCode.Should().Be(HttpStatusCode.OK);
        response3.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion
}
