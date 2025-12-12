using System.Net;
using System.Text.Json;
using mSzczepienia.Api;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Facilities;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Tests.Integration.Fixtures;

namespace mSzczepienia.Tests.Integration.Controllers;

[Collection("Facility Controller Collection")]
public class FacilityControllerTests
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public FacilityControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    #region GetCitiesAutocomplete Tests

    [Fact]
    public async Task GetCitiesAutocomplete_WithAdminRole_ShouldReturnOk()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin");

        // Act
        var response = await client.GetAsync("/api/facilities/cities-autocomplete?query=War");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<string>>();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetCitiesAutocomplete_WithoutRole_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("user");

        // Act
        var response = await client.GetAsync("/api/facilities/cities-autocomplete?query=War");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetCitiesAutocomplete_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClientWithoutAuth();

        // Act
        var response = await client.GetAsync("/api/facilities/cities-autocomplete?query=War");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetCitiesAutocomplete_WithAdminRole_NoQuery_ShouldReturnOk()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin");

        // Act
        var response = await client.GetAsync("/api/facilities/cities-autocomplete");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetCitiesAutocomplete_WithAdminRole_EmptyQuery_ShouldReturnOk()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin");

        // Act
        var response = await client.GetAsync("/api/facilities/cities-autocomplete?query=");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region GetFacilityNames Tests

    [Fact]
    public async Task GetFacilityNames_WithoutAuthentication_ShouldReturnOk()
    {
        // Arrange
        var client = _factory.CreateClientWithoutAuth();

        // Act
        var response = await client.GetAsync("/api/facilities/facility-names");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<FacilityListDto>>();
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetFacilityNames_WithAuthentication_ShouldReturnOk()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin");

        // Act
        var response = await client.GetAsync("/api/facilities/facility-names");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<FacilityListDto>>();
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetFacilityNames_ShouldReturnCorrectFacilityData()
    {
        // Arrange
        var client = _factory.CreateClientWithoutAuth();

        // Act
        var response = await client.GetAsync("/api/facilities/facility-names");
        var result = await response.Content.ReadFromJsonAsync<List<FacilityListDto>>();

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().BeGreaterThanOrEqualTo(3);
        result.Should().Contain(f => f.facilityName == "Test Clinic 1");
        result.Should().Contain(f => f.facilityName == "Test Clinic 2");
        result.Should().Contain(f => f.facilityName == "Test Hospital");
    }

    #endregion

    #region SearchFacilities Tests

    [Fact]
    public async Task SearchFacilities_WithAdminRole_ValidQuery_ShouldReturnOk()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin");
        var query = new GetFacilitiesQuery
        {
            PageNumber = 1,
            PageSize = 10,
            City = "Warsaw"
        };

        // Act
        var response = await client.GetAsync($"/api/facilities/search?pageNumber={query.PageNumber}&pageSize={query.PageSize}&city={query.City}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<SearchFacilityDto>>();
        result.Should().NotBeNull();
        result!.Items.Should().NotBeEmpty();
        result.Items.Should().AllSatisfy(f => f.City.Should().Be("Warsaw"));
    }

    [Fact]
    public async Task SearchFacilities_WithoutRole_ShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("user");

        // Act
        var response = await client.GetAsync("/api/facilities/search?pageNumber=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task SearchFacilities_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClientWithoutAuth();

        // Act
        var response = await client.GetAsync("/api/facilities/search?pageNumber=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task SearchFacilities_InvalidPageNumber_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin");

        // Act
        var response = await client.GetAsync("/api/facilities/search?pageNumber=0&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SearchFacilities_InvalidPageSize_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin");

        // Act
        var response = await client.GetAsync("/api/facilities/search?pageNumber=1&pageSize=0");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SearchFacilities_PageSizeExceeds100_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin");

        // Act
        var response = await client.GetAsync("/api/facilities/search?pageNumber=1&pageSize=101");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SearchFacilities_DefaultPagination_ShouldReturnFirstPage()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin");

        // Act
        var response = await client.GetAsync("/api/facilities/search?pageNumber=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<SearchFacilityDto>>();
        result.Should().NotBeNull();
        result!.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task SearchFacilities_ByCity_ShouldReturnFilteredResults()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin");

        // Act
        var response = await client.GetAsync("/api/facilities/search?pageNumber=1&pageSize=10&city=Warsaw");
        var result = await response.Content.ReadFromJsonAsync<PagedResult<SearchFacilityDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.Items.Should().AllSatisfy(f => f.City.Should().Be("Warsaw"));
        result.TotalCount.Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task SearchFacilities_ByStreet_ShouldReturnFilteredResults()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin");

        // Act
        var response = await client.GetAsync("/api/facilities/search?pageNumber=1&pageSize=10&street=Main%20St");
        var result = await response.Content.ReadFromJsonAsync<PagedResult<SearchFacilityDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.Items.Should().AllSatisfy(f => f.Street.Should().Contain("Main St"));
    }

    [Fact]
    public async Task SearchFacilities_ByPostalCode_ShouldReturnFilteredResults()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin");

        // Act
        var response = await client.GetAsync("/api/facilities/search?pageNumber=1&pageSize=10&zipCode=00-001");
        var result = await response.Content.ReadFromJsonAsync<PagedResult<SearchFacilityDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.Items.Should().AllSatisfy(f => f.PostalCode.Should().Be("00-001"));
    }

    [Fact]
    public async Task SearchFacilities_ByFacilityId_ShouldReturnFilteredResults()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin");

        // Act
        var response = await client.GetAsync("/api/facilities/search?pageNumber=1&pageSize=10&id=1");
        var result = await response.Content.ReadFromJsonAsync<PagedResult<SearchFacilityDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.Items.Should().Contain(f => f.Id == 1);
    }

    [Fact]
    public async Task SearchFacilities_MultipleFilters_ShouldReturnFilteredResults()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin");

        // Act
        var response = await client.GetAsync("/api/facilities/search?pageNumber=1&pageSize=10&city=Warsaw&street=Main%20St");
        var result = await response.Content.ReadFromJsonAsync<PagedResult<SearchFacilityDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.Items.Should().AllSatisfy(f => 
        {
            f.City.Should().Be("Warsaw");
            f.Street.Should().Contain("Main St");
        });
    }

    [Fact]
    public async Task SearchFacilities_WithMaxPageSize_ShouldReturnOk()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin");

        // Act
        var response = await client.GetAsync("/api/facilities/search?pageNumber=1&pageSize=100");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SearchFacilities_PagedResults_ShouldContainCorrectMetadata()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin");

        // Act
        var response = await client.GetAsync("/api/facilities/search?pageNumber=1&pageSize=2");
        var result = await response.Content.ReadFromJsonAsync<PagedResult<SearchFacilityDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(2);
        result.TotalCount.Should().BeGreaterThanOrEqualTo(3);
        result.TotalPages.Should().BeGreaterThanOrEqualTo(2);
    }

    #endregion

    #region Authorization Tests

    [Fact]
    public async Task AdminEndpoints_WithoutToken_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClientWithoutAuth();

        // Act
        var citiesResponse = await client.GetAsync("/api/facilities/cities-autocomplete");
        var searchResponse = await client.GetAsync("/api/facilities/search?pageNumber=1&pageSize=10");

        // Assert
        citiesResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        searchResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AdminEndpoints_WithUserRole_ShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("user");

        // Act
        var citiesResponse = await client.GetAsync("/api/facilities/cities-autocomplete");
        var searchResponse = await client.GetAsync("/api/facilities/search?pageNumber=1&pageSize=10");

        // Assert
        citiesResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        searchResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AdminEndpoints_WithAdminRole_ShouldReturnSuccess()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin");

        // Act
        var citiesResponse = await client.GetAsync("/api/facilities/cities-autocomplete");
        var searchResponse = await client.GetAsync("/api/facilities/search?pageNumber=1&pageSize=10");

        // Assert
        citiesResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        searchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion
}
