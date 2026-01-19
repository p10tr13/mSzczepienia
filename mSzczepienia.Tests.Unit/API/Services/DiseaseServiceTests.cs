using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Api.Services;
using mSzczepienia.Shared.DTO.Diseases;
using mSzczepienia.Shared.Models;

namespace mSzczepienia.Tests.Unit.API.Services;

public class DiseaseServiceTests
{
    private readonly IDiseaseRepository _diseaseRepositoryMock;
    private readonly DiseaseService _sut; // SUT = System Under Test

    public DiseaseServiceTests()
    {
        _diseaseRepositoryMock = Substitute.For<IDiseaseRepository>();
        _sut = new DiseaseService(_diseaseRepositoryMock);
    }

    #region GetDiseasesAutocompleteAsync Tests

    [Fact]
    public async Task GetDiseasesAutocompleteAsync_ShouldReturnDiseaseList_WhenMatchesFound()
    {
        // Arrange
        var query = "COVID";
        var count = 5;
        var diseases = new List<Disease>
        {
            new Disease { diseaseId = 1, diseaseNameEn = "COVID-19", diseaseNamePl = "COVID-19" },
            new Disease { diseaseId = 2, diseaseNameEn = "COVID-20", diseaseNamePl = "COVID-20" }
        };

        _diseaseRepositoryMock.GetByNameAsNoTrackingAsync(query, Arg.Any<CancellationToken>())
            .Returns(diseases);

        // Act
        var result = await _sut.GetDiseasesAutocompleteAsync(query, count, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.First().DiseaseNameEn.Should().Be("COVID-19");
        result.First().DiseaseNamePl.Should().Be("COVID-19");

        await _diseaseRepositoryMock.Received(1)
            .GetByNameAsNoTrackingAsync(query, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetDiseasesAutocompleteAsync_ShouldReturnLimitedResults_WhenCountLessThanResults()
    {
        // Arrange
        var query = "Flu";
        var count = 2;
        var diseases = new List<Disease>
        {
            new Disease { diseaseId = 1, diseaseNameEn = "Influenza A", diseaseNamePl = "Grypa A" },
            new Disease { diseaseId = 2, diseaseNameEn = "Influenza B", diseaseNamePl = "Grypa B" },
            new Disease { diseaseId = 3, diseaseNameEn = "Influenza C", diseaseNamePl = "Grypa C" }
        };

        _diseaseRepositoryMock.GetByNameAsNoTrackingAsync(query, Arg.Any<CancellationToken>())
            .Returns(diseases);

        // Act
        var result = await _sut.GetDiseasesAutocompleteAsync(query, count, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result.First().DiseaseNameEn.Should().Be("Influenza A");
        result.Last().DiseaseNameEn.Should().Be("Influenza B");
    }

    [Fact]
    public async Task GetDiseasesAutocompleteAsync_ShouldReturnEmptyList_WhenNoMatchesFound()
    {
        // Arrange
        var query = "NonExistentDisease";
        var count = 10;

        _diseaseRepositoryMock.GetByNameAsNoTrackingAsync(query, Arg.Any<CancellationToken>())
            .Returns((List<Disease>?)null);

        // Act
        var result = await _sut.GetDiseasesAutocompleteAsync(query, count, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetDiseasesAutocompleteAsync_ShouldReturnEmptyList_WhenRepositoryReturnsEmptyList()
    {
        // Arrange
        var query = "Measles";
        var count = 10;

        _diseaseRepositoryMock.GetByNameAsNoTrackingAsync(query, Arg.Any<CancellationToken>())
            .Returns(new List<Disease>());

        // Act
        var result = await _sut.GetDiseasesAutocompleteAsync(query, count, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetDiseasesAutocompleteAsync_ShouldMapDiseasesToDtos_Correctly()
    {
        // Arrange
        var query = "Mumps";
        var count = 5;
        var disease = new Disease
        {
            diseaseId = 10,
            diseaseNameEn = "Mumps",
            diseaseNamePl = "Świnka"
        };

        _diseaseRepositoryMock.GetByNameAsNoTrackingAsync(query, Arg.Any<CancellationToken>())
            .Returns(new List<Disease> { disease });

        // Act
        var result = await _sut.GetDiseasesAutocompleteAsync(query, count, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        var resultDto = result.First();
        resultDto.DiseaseId.Should().Be(10);
        resultDto.DiseaseNameEn.Should().Be("Mumps");
        resultDto.DiseaseNamePl.Should().Be("Świnka");
    }

    [Fact]
    public async Task GetDiseasesAutocompleteAsync_ShouldRespectCountParameter()
    {
        // Arrange
        var query = "Virus";
        var count = 1;
        var diseases = new List<Disease>
        {
            new Disease { diseaseId = 1, diseaseNameEn = "Viral Infection A", diseaseNamePl = "Infekcja Wirusowa A" },
            new Disease { diseaseId = 2, diseaseNameEn = "Viral Infection B", diseaseNamePl = "Infekcja Wirusowa B" }
        };

        _diseaseRepositoryMock.GetByNameAsNoTrackingAsync(query, Arg.Any<CancellationToken>())
            .Returns(diseases);

        // Act
        var result = await _sut.GetDiseasesAutocompleteAsync(query, count, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.First().DiseaseNameEn.Should().Be("Viral Infection A");
    }

    [Fact]
    public async Task GetDiseasesAutocompleteAsync_ShouldCallRepository_WithCorrectParameters()
    {
        // Arrange
        var query = "Smallpox";
        var count = 10;

        _diseaseRepositoryMock.GetByNameAsNoTrackingAsync(query, Arg.Any<CancellationToken>())
            .Returns(new List<Disease>());

        // Act
        await _sut.GetDiseasesAutocompleteAsync(query, count, CancellationToken.None);

        // Assert
        await _diseaseRepositoryMock.Received(1)
            .GetByNameAsNoTrackingAsync(query, Arg.Any<CancellationToken>());
    }

    #endregion
}
