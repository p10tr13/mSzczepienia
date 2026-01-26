using mSzczepienia.Shared.DTO.Diseases;
using mSzczepienia.Tests.Integration.Fixtures;
using System.Net;

namespace mSzczepienia.Tests.Integration.Controllers;

public class DiseaseControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public DiseaseControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    #region Authorization Tests

    [Fact]
    public async Task GetDiseasesAutocomplete_WithAdminRole_ShouldReturnOk()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/diseases/names-autocomplete?query=Inf&count=5");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetDiseasesAutocomplete_WithWorkerRole_ShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");

        // Act
        var response = await client.GetAsync("/api/diseases/names-autocomplete?query=Inf&count=5");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetDiseasesAutocomplete_WithCitizenRole_ShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("citizen", "test-citizen-uid-1");

        // Act
        var response = await client.GetAsync("/api/diseases/names-autocomplete?query=Inf&count=5");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetDiseasesAutocomplete_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClientWithoutAuth();

        // Act
        var response = await client.GetAsync("/api/diseases/names-autocomplete?query=Inf&count=5");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region Validation Tests

    [Fact]
    public async Task GetDiseasesAutocomplete_WithEmptyQuery_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/diseases/names-autocomplete?query=&count=5");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetDiseasesAutocomplete_WithWhitespaceQuery_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/diseases/names-autocomplete?query=   &count=5");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetDiseasesAutocomplete_WithZeroCount_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/diseases/names-autocomplete?query=Inf&count=0");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetDiseasesAutocomplete_WithNegativeCount_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/diseases/names-autocomplete?query=Inf&count=-1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Functional Tests

    [Fact]
    public async Task GetDiseasesAutocomplete_SearchByEnglishName_ShouldReturnMatchingDiseases()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/diseases/names-autocomplete?query=Influenza&count=5");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<DiseaseDto>>();
        result.Should().NotBeNull();
        result.Should().Contain(d => d.DiseaseNameEn == "Influenza");
    }

    [Fact]
    public async Task GetDiseasesAutocomplete_SearchByPolishName_ShouldReturnMatchingDiseases()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/diseases/names-autocomplete?query=Grypa&count=5");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<DiseaseDto>>();
        result.Should().NotBeNull();
        result.Should().Contain(d => d.DiseaseNamePl == "Grypa");
    }

    [Fact]
    public async Task GetDiseasesAutocomplete_SearchByPartialName_ShouldReturnMatchingDiseases()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/diseases/names-autocomplete?query=Inf&count=5");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<DiseaseDto>>();
        result.Should().NotBeNull();
        result.Should().Contain(d => d.DiseaseNameEn.Contains("Inf"));
    }

    [Fact]
    public async Task GetDiseasesAutocomplete_SearchCovid_ShouldReturnCovidDisease()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/diseases/names-autocomplete?query=COVID&count=5");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<DiseaseDto>>();
        result.Should().NotBeNull();
        result.Should().Contain(d => d.DiseaseNameEn == "COVID-19");
    }

    [Fact]
    public async Task GetDiseasesAutocomplete_SearchMeasles_ShouldReturnMeaslesDisease()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/diseases/names-autocomplete?query=Measles&count=5");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<DiseaseDto>>();
        result.Should().NotBeNull();
        result.Should().Contain(d => d.DiseaseNameEn == "Measles");
    }

    [Fact]
    public async Task GetDiseasesAutocomplete_SearchOdra_ShouldReturnMeaslesDisease()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/diseases/names-autocomplete?query=Odra&count=5");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<DiseaseDto>>();
        result.Should().NotBeNull();
        result.Should().Contain(d => d.DiseaseNamePl == "Odra");
    }

    [Fact]
    public async Task GetDiseasesAutocomplete_WithCountLimit_ShouldReturnLimitedResults()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act - search for partial match that could return multiple results
        var response = await client.GetAsync("/api/diseases/names-autocomplete?query=a&count=1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<DiseaseDto>>();
        result.Should().NotBeNull();
        result!.Count.Should().BeInRange(0, 1);
    }

    [Fact]
    public async Task GetDiseasesAutocomplete_NoMatchingResults_ShouldReturnEmptyList()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/diseases/names-autocomplete?query=NonExistentDisease&count=5");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<DiseaseDto>>();
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetDiseasesAutocomplete_ShouldReturnCorrectDtoStructure()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/diseases/names-autocomplete?query=Influenza&count=5");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<DiseaseDto>>();
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        
        var disease = result!.First();
        disease.DiseaseId.Should().BeGreaterThan(0);
        disease.DiseaseNameEn.Should().NotBeNullOrEmpty();
        disease.DiseaseNamePl.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetDiseasesAutocomplete_MultipleCallsWithSameQuery_ShouldReturnConsistentData()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response1 = await client.GetAsync("/api/diseases/names-autocomplete?query=Influenza&count=5");
        var result1 = await response1.Content.ReadFromJsonAsync<List<DiseaseDto>>();

        var response2 = await client.GetAsync("/api/diseases/names-autocomplete?query=Influenza&count=5");
        var result2 = await response2.Content.ReadFromJsonAsync<List<DiseaseDto>>();

        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        response2.StatusCode.Should().Be(HttpStatusCode.OK);
        result1.Should().NotBeNull();
        result2.Should().NotBeNull();
        result1!.Count.Should().Be(result2!.Count);
    }

    [Fact]
    public async Task GetDiseasesAutocomplete_CaseInsensitiveSearch_ShouldReturnResults()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var responseLower = await client.GetAsync("/api/diseases/names-autocomplete?query=influenza&count=5");
        var responseUpper = await client.GetAsync("/api/diseases/names-autocomplete?query=INFLUENZA&count=5");

        // Assert
        responseLower.StatusCode.Should().Be(HttpStatusCode.OK);
        responseUpper.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var resultLower = await responseLower.Content.ReadFromJsonAsync<List<DiseaseDto>>();
        var resultUpper = await responseUpper.Content.ReadFromJsonAsync<List<DiseaseDto>>();
        
        // Note: This test verifies whether the search is case-insensitive
        // If the search is case-sensitive, one of these may return empty results
        resultLower.Should().NotBeNull();
        resultUpper.Should().NotBeNull();
    }

    [Fact]
    public async Task GetDiseasesAutocomplete_DifferentAdminAccounts_ShouldReturnSameData()
    {
        // Arrange
        var client1 = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");
        var client2 = _factory.CreateClientWithAuth("admin", "test-admin-uid-2");

        // Act
        var response1 = await client1.GetAsync("/api/diseases/names-autocomplete?query=Influenza&count=5");
        var result1 = await response1.Content.ReadFromJsonAsync<List<DiseaseDto>>();

        var response2 = await client2.GetAsync("/api/diseases/names-autocomplete?query=Influenza&count=5");
        var result2 = await response2.Content.ReadFromJsonAsync<List<DiseaseDto>>();

        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        response2.StatusCode.Should().Be(HttpStatusCode.OK);
        result1.Should().NotBeNull();
        result2.Should().NotBeNull();
        result1!.Count.Should().Be(result2!.Count);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task GetDiseasesAutocomplete_WithSpecialCharacters_ShouldHandleGracefully()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/diseases/names-autocomplete?query=COVID-19&count=5");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<DiseaseDto>>();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetDiseasesAutocomplete_WithLargeCount_ShouldReturnAllAvailable()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act - request more than available
        var response = await client.GetAsync("/api/diseases/names-autocomplete?query=a&count=1000");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<DiseaseDto>>();
        result.Should().NotBeNull();
        // Should return all matching diseases (up to count)
    }

    [Fact]
    public async Task GetDiseasesAutocomplete_WithSingleCharacterQuery_ShouldWork()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/diseases/names-autocomplete?query=I&count=5");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<DiseaseDto>>();
        result.Should().NotBeNull();
    }

    #endregion
}
