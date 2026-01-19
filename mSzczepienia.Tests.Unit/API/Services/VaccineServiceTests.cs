using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Api.Services;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.DTO.Vaccines;
using mSzczepienia.Shared.DTO.Diseases;
using mSzczepienia.Shared.Models;

namespace mSzczepienia.Tests.Unit.API.Services;

public class VaccineServiceTests
{
    private readonly IVaccineRepository _vaccineRepositoryMock;
    private readonly IDiseaseRepository _diseaseRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly VaccineService _sut; // SUT = System Under Test

    public VaccineServiceTests()
    {
        _vaccineRepositoryMock = Substitute.For<IVaccineRepository>();
        _diseaseRepositoryMock = Substitute.For<IDiseaseRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _sut = new VaccineService(_vaccineRepositoryMock, _diseaseRepositoryMock, _unitOfWorkMock);
    }

    #region GetManufacturersAutocompleteAsync Tests

    [Fact]
    public async Task GetManufacturersAutocompleteAsync_ShouldCallRepoAndReturnResult()
    {
        // Arrange
        var query = "Pfi";
        var expectedResult = new List<string> { "Pfizer", "Pfizer-BioNTech" };

        _vaccineRepositoryMock.GetManufacturersAutocompleteAsync(query, Arg.Any<CancellationToken>())
            .Returns(expectedResult);

        // Act
        var result = await _sut.GetManufacturersAutocompleteAsync(query, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);

        await _vaccineRepositoryMock.Received(1)
            .GetManufacturersAutocompleteAsync(query, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetManufacturersAutocompleteAsync_ShouldReturnEmptyList_WhenNoMatches()
    {
        // Arrange
        var query = "NonExistent";

        _vaccineRepositoryMock.GetManufacturersAutocompleteAsync(query, Arg.Any<CancellationToken>())
            .Returns(new List<string>());

        // Act
        var result = await _sut.GetManufacturersAutocompleteAsync(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    #endregion

    #region GetNamesAutocompleteAsync Tests

    [Fact]
    public async Task GetNamesAutocompleteAsync_ShouldCallRepoAndReturnResult()
    {
        // Arrange
        var query = "War";
        var expectedResult = new List<string> { "Warszawa", "Warka" };

        _vaccineRepositoryMock.GetNamesAutocompleteAsync(query, Arg.Any<CancellationToken>())
            .Returns(expectedResult);

        // Act
        var result = await _sut.GetNamesAutocompleteAsync(query, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);

        await _vaccineRepositoryMock.Received(1)
            .GetNamesAutocompleteAsync(query, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetNamesAutocompleteAsync_ShouldReturnEmptyList_WhenNoMatches()
    {
        // Arrange
        var query = "NonExistent";

        _vaccineRepositoryMock.GetNamesAutocompleteAsync(query, Arg.Any<CancellationToken>())
            .Returns(new List<string>());

        // Act
        var result = await _sut.GetNamesAutocompleteAsync(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    #endregion

    #region GetVaccinesAsync Tests

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
                    VaccineId = 1,
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

        _vaccineRepositoryMock.GetVaccinesAsync(query, Arg.Any<CancellationToken>())
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

    [Fact]
    public async Task GetVaccinesAsync_ShouldReturnEmptyResult_WhenNoVaccinesFound()
    {
        // Arrange
        var query = new GetVaccinesQuery
        {
            PageNumber = 1,
            PageSize = 10
        };

        var emptyResult = new PagedResult<VaccineDto>
        {
            Items = new List<VaccineDto>(),
            TotalCount = 0,
            PageNumber = 1,
            PageSize = 10
        };

        _vaccineRepositoryMock.GetVaccinesAsync(query, Arg.Any<CancellationToken>())
            .Returns(emptyResult);

        // Act
        var result = await _sut.GetVaccinesAsync(query, CancellationToken.None);

        // Assert
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    #endregion

    #region GetVaccineByIdAsync Tests

    [Fact]
    public async Task GetVaccineByIdAsync_ShouldReturnVaccineDto_WhenVaccineExists()
    {
        // Arrange
        var vaccineId = 5;
        var expectedVaccine = new VaccineDto
        {
            VaccineId = vaccineId,
            VaccineName = "Pfizer-BioNTech",
            Manufacturer = "Pfizer",
            DiseasesEn = new List<string> { "COVID-19" },
            DiseasesPl = new List<string> { "COVID-19" }
        };

        _vaccineRepositoryMock.GetVaccineByIdAsync(vaccineId, Arg.Any<CancellationToken>())
            .Returns(expectedVaccine);

        // Act
        var result = await _sut.GetVaccineByIdAsync(vaccineId, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedVaccine);
    }

    [Fact]
    public async Task GetVaccineByIdAsync_ShouldReturnNull_WhenVaccineDoesNotExist()
    {
        // Arrange
        var vaccineId = 999;

        _vaccineRepositoryMock.GetVaccineByIdAsync(vaccineId, Arg.Any<CancellationToken>())
            .Returns((VaccineDto?)null);

        // Act
        var result = await _sut.GetVaccineByIdAsync(vaccineId, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region CreateVaccineAsync Tests

    [Fact]
    public async Task CreateVaccineAsync_ShouldCreateVaccineWithExistingDisease()
    {
        // Arrange
        var dto = new CreateVaccineDto
        {
            VaccineName = "New Vaccine",
            Manufacturer = "New Pharma",
            VaccineDescriptionEn = "Description EN",
            VaccineDescriptionPl = "Description PL",
            ShortDescriptionEn = "Short EN",
            ShortDescriptionPl = "Short PL",
            Diseases = new List<DiseaseDto>
            {
                new DiseaseDto { DiseaseId = 1, DiseaseNameEn = "Disease 1", DiseaseNamePl = "Choroba 1" }
            }
        };

        var disease = new Disease
        {
            diseaseId = 1,
            diseaseNameEn = "Disease 1",
            diseaseNamePl = "Choroba 1"
        };

        _diseaseRepositoryMock.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(disease);

        // Act
        await _sut.CreateVaccineAsync(dto, CancellationToken.None);

        // Assert
        await _vaccineRepositoryMock.Received(1).AddAsync(Arg.Any<VaccineDefinition>(), Arg.Any<CancellationToken>());
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateVaccineAsync_ShouldCreateVaccineWithNewDisease()
    {
        // Arrange
        var dto = new CreateVaccineDto
        {
            VaccineName = "Another Vaccine",
            Manufacturer = "Another Pharma",
            VaccineDescriptionEn = "Desc EN",
            VaccineDescriptionPl = "Desc PL",
            ShortDescriptionEn = "Short EN",
            ShortDescriptionPl = "Short PL",
            Diseases = new List<DiseaseDto>
            {
                new DiseaseDto { DiseaseId = 0, DiseaseNameEn = "New Disease", DiseaseNamePl = "Nowa Choroba" }
            }
        };

        // Act
        await _sut.CreateVaccineAsync(dto, CancellationToken.None);

        // Assert
        await _vaccineRepositoryMock.Received(1).AddAsync(Arg.Any<VaccineDefinition>(), Arg.Any<CancellationToken>());
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    #endregion

    #region GetDetailedVaccineByIdAsync Tests

    [Fact]
    public async Task GetDetailedVaccineByIdAsync_ShouldReturnDetailedVaccineDto_WhenVaccineExists()
    {
        // Arrange
        var vaccineId = 3;
        var disease = new Disease { diseaseId = 1, diseaseNameEn = "COVID-19", diseaseNamePl = "COVID-19" };
        var vaccine = new VaccineDefinition
        {
            vaccineId = vaccineId,
            vaccineName = "Test Vaccine",
            manufacturer = "Test Pharma",
            vaccineDescriptionEn = "Test Description EN",
            vaccineDescriptionPl = "Test Description PL",
            shortDescriptionEn = "Short EN",
            shortDescriptionPl = "Short PL",
            creationDate = DateTime.UtcNow,
            vaccineDiseases = new List<VaccineDisease>
            {
                new VaccineDisease { disease = disease }
            }
        };

        _vaccineRepositoryMock.GetVaccineDefinitionByIdAsync(vaccineId, Arg.Any<CancellationToken>())
            .Returns(vaccine);

        // Act
        var result = await _sut.GetDetailedVaccineByIdAsync(vaccineId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(vaccineId);
        result.VaccineName.Should().Be("Test Vaccine");
        result.Manufacturer.Should().Be("Test Pharma");
        result.Diseases.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetDetailedVaccineByIdAsync_ShouldReturnNull_WhenVaccineDoesNotExist()
    {
        // Arrange
        var vaccineId = 999;

        _vaccineRepositoryMock.GetVaccineDefinitionByIdAsync(vaccineId, Arg.Any<CancellationToken>())
            .Returns((VaccineDefinition?)null);

        // Act
        var result = await _sut.GetDetailedVaccineByIdAsync(vaccineId, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region UpdateVaccineAsync Tests

    [Fact]
    public async Task UpdateVaccineAsync_ShouldUpdateVaccine_WhenVaccineExists()
    {
        // Arrange
        var vaccineId = 2;
        var updateDto = new CreateVaccineDto
        {
            VaccineName = "Updated Vaccine",
            Manufacturer = "Updated Pharma",
            VaccineDescriptionEn = "Updated EN",
            VaccineDescriptionPl = "Updated PL",
            ShortDescriptionEn = "Updated Short EN",
            ShortDescriptionPl = "Updated Short PL",
            Diseases = new List<DiseaseDto>
            {
                new DiseaseDto { DiseaseId = 2, DiseaseNameEn = "Influenza", DiseaseNamePl = "Grypa" }
            }
        };

        var disease = new Disease { diseaseId = 2, diseaseNameEn = "Influenza", diseaseNamePl = "Grypa" };
        var vaccine = new VaccineDefinition
        {
            vaccineId = vaccineId,
            vaccineName = "Old Name",
            manufacturer = "Old Pharma",
            vaccineDescriptionEn = "Old",
            vaccineDescriptionPl = "Old",
            shortDescriptionEn = "Old",
            shortDescriptionPl = "Old",
            creationDate = DateTime.UtcNow,
            vaccineDiseases = new List<VaccineDisease>()
        };

        _vaccineRepositoryMock.GetVaccineDefinitionByIdAsync(vaccineId, Arg.Any<CancellationToken>())
            .Returns(vaccine);

        _diseaseRepositoryMock.GetByIdAsync(2, Arg.Any<CancellationToken>())
            .Returns(disease);

        // Act
        await _sut.UpdateVaccineAsync(vaccineId, updateDto, CancellationToken.None);

        // Assert
        vaccine.vaccineName.Should().Be(updateDto.VaccineName);
        vaccine.manufacturer.Should().Be(updateDto.Manufacturer);
        vaccine.vaccineDescriptionEn.Should().Be(updateDto.VaccineDescriptionEn);

        _vaccineRepositoryMock.Received(1).Update(vaccine);
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateVaccineAsync_ShouldThrowException_WhenVaccineDoesNotExist()
    {
        // Arrange
        var vaccineId = 999;
        var updateDto = new CreateVaccineDto
        {
            VaccineName = "Test",
            Manufacturer = "Test",
            VaccineDescriptionEn = "Test",
            VaccineDescriptionPl = "Test",
            ShortDescriptionEn = "Test",
            ShortDescriptionPl = "Test",
            Diseases = new List<DiseaseDto>()
        };

        _vaccineRepositoryMock.GetVaccineDefinitionByIdAsync(vaccineId, Arg.Any<CancellationToken>())
            .Returns((VaccineDefinition?)null);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(
            () => _sut.UpdateVaccineAsync(vaccineId, updateDto, CancellationToken.None));
    }

    #endregion

    #region SearchVaccineNamesAsync Tests

    [Fact]
    public async Task SearchVaccineNamesAsync_ShouldReturnVaccineNames_WhenMatchesFound()
    {
        // Arrange
        var query = "Pfizer";
        var expectedNames = new List<VaccineNameDto>
        {
            new VaccineNameDto { VaccineId = 1, VaccineName = "Pfizer-BioNTech" },
            new VaccineNameDto { VaccineId = 2, VaccineName = "Pfizer Original" }
        };

        _vaccineRepositoryMock.SearchVaccineNamesAsync(query, Arg.Any<CancellationToken>())
            .Returns(expectedNames);

        // Act
        var result = await _sut.SearchVaccineNamesAsync(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(expectedNames);
    }

    [Fact]
    public async Task SearchVaccineNamesAsync_ShouldReturnEmptyList_WhenNoMatches()
    {
        // Arrange
        var query = "NonExistent";

        _vaccineRepositoryMock.SearchVaccineNamesAsync(query, Arg.Any<CancellationToken>())
            .Returns(new List<VaccineNameDto>());

        // Act
        var result = await _sut.SearchVaccineNamesAsync(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    #endregion
}
