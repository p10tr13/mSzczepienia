using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Api.Services;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Facilities;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.Models;

namespace mSzczepienia.Tests.Unit.API.Services;

public class FacilityServiceTests
{
    private readonly IFacilityRepository _facilityRepositoryMock;
    private readonly FacilityService _sut; // SUT = System Under Test

    public FacilityServiceTests()
    {
        _facilityRepositoryMock = Substitute.For<IFacilityRepository>();
        _sut = new FacilityService(_facilityRepositoryMock);
    }

    [Fact]
    public async Task GetCitiesAutocompleteAsync_ShouldReturnList_WhenRepoReturnsData()
    {
        // Arrange
        var query = "War";
        var expectedCities = new List<string> { "Warszawa", "Warka" };

        _facilityRepositoryMock
            .GetCitiesAutocompleteAsync(query, Arg.Any<CancellationToken>())
            .Returns(expectedCities);

        // Act
        var result = await _sut.GetCitiesAutocompleteAsync(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain("Warszawa");

        await _facilityRepositoryMock.Received(1)
            .GetCitiesAutocompleteAsync(query, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetFacilityNamesAsync_ShouldReturnDtoList_WhenRepoReturnsData()
    {
        // Arrange
        var expectedDtos = new List<FacilityListDto>
        {
            new FacilityListDto { facilityId = 1, facilityName = "Przychodnia A" },
            new FacilityListDto { facilityId = 2, facilityName = "Szpital B" }
        };

        _facilityRepositoryMock
            .GetFacilityNamesAsync(Arg.Any<CancellationToken>())
            .Returns(expectedDtos);

        // Act
        var result = await _sut.GetFacilityNamesAsync(CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedDtos);
    }

    [Fact]
    public async Task SearchFacilitiesAsync_ShouldMapEntityToDtoCorrectly()
    {
        // Arrange
        var query = new GetFacilitiesQuery { PageNumber = 1, PageSize = 10 };

        var entitiesFromDb = new List<MedicalFacility>
        {
            new MedicalFacility
            {
                facilityId = 10,
                facilityName = "Medyk",
                street = "Długa 5",
                city = "Kraków",
                postalCode = "30-001",
                country = "Polska",
                nip = "123-456-32-18",
            }
        };

        var pagedResultFromRepo = new PagedResult<MedicalFacility>
        {
            Items = entitiesFromDb,
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };

        _facilityRepositoryMock
            .SearchFacilitiesAsync(query, Arg.Any<CancellationToken>())
            .Returns(pagedResultFromRepo);

        // Act
        var result = await _sut.SearchFacilitiesAsync(query, CancellationToken.None);

        // Assert
        result.TotalCount.Should().Be(1);
        result.PageSize.Should().Be(10);

        var dto = result.Items.First();

        dto.Id.Should().Be(10);
        dto.Name.Should().Be("Medyk");
        dto.Street.Should().Be("Długa 5");
        dto.City.Should().Be("Kraków");
        dto.PostalCode.Should().Be("30-001");
    }

    [Fact]
    public async Task SearchFacilitiesAsync_ShouldReturnEmptyList_WhenRepoReturnsEmpty()
    {
        // Arrange
        var query = new GetFacilitiesQuery();

        var emptyRepoResult = new PagedResult<MedicalFacility>
        {
            Items = new List<MedicalFacility>(),
            TotalCount = 0
        };

        _facilityRepositoryMock
            .SearchFacilitiesAsync(query, Arg.Any<CancellationToken>())
            .Returns(emptyRepoResult);

        // Act
        var result = await _sut.SearchFacilitiesAsync(query, CancellationToken.None);

        // Assert
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }
}
