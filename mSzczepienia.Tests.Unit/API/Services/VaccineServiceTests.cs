using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Api.Services;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.DTO.Vaccines;

namespace mSzczepienia.Tests.Unit.API.Services;

public class VaccineServiceTests
{
    private readonly IVaccineRepository _vaccineRepositoryMock;
    private readonly VaccineService _sut; // SUT = System Under Test

    public VaccineServiceTests()
    {
        _vaccineRepositoryMock = Substitute.For<IVaccineRepository>();
        _sut = new VaccineService(_vaccineRepositoryMock);
    }

    [Fact]
    public async Task GetManufacturersAutocompleteAsync_ShouldCallRepoAndReturnResult()
    {
        // Arrange
        var query = "Pfi";
        var expectedResult = new List<string> { "Pfizer", "Pfizer-BioNTech" };

        _vaccineRepositoryMock
            .GetManufacturersAutocompleteAsync(query, Arg.Any<CancellationToken>())
            .Returns(expectedResult);

        // Act
        var result = await _sut.GetManufacturersAutocompleteAsync(query, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);

        await _vaccineRepositoryMock.Received(1)
            .GetManufacturersAutocompleteAsync(query, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetNamesAutocompleteAsync_ShouldCallRepoAndReturnResult()
    {
        // Arrange
        var query = "War";
        var expectedResult = new List<string> { "Warszawa", "Warka" };

        _vaccineRepositoryMock
            .GetNamesAutocompleteAsync(query, Arg.Any<CancellationToken>())
            .Returns(expectedResult);

        // Act
        var result = await _sut.GetNamesAutocompleteAsync(query, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);

        await _vaccineRepositoryMock.Received(1)
            .GetNamesAutocompleteAsync(query, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetVaccinesAsync_ShouldPassQueryToRepoAndReturnPagedResult()
    {
        // Arrange
        var query = new GetVaccinesQuery
        {
            PageNumber = 1,
            PageSize = 10,
            Manufacturer = "Moderna"
        };

        var expectedPagedResult = new PagedResult<VaccineDto>
        {
            Items = new List<VaccineDto>
            {
                new VaccineDto { 
                    VaccineName = "Spikevax",
                    Manufacturer = "Moderna",
                    DiseasesEn = new List<string> { "COVID-19" },
                    DiseasesPl = new List<string> { "COVID-19" }
                }
            },
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };

        _vaccineRepositoryMock
            .GetVaccinesAsync(query, Arg.Any<CancellationToken>())
            .Returns(expectedPagedResult);

        // Act
        var result = await _sut.GetVaccinesAsync(query, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedPagedResult);
        result.Items.Should().HaveCount(1);
        result.Items.First().VaccineName.Should().Be("Spikevax");

        await _vaccineRepositoryMock.Received(1)
            .GetVaccinesAsync(query, Arg.Any<CancellationToken>());
    }
}
