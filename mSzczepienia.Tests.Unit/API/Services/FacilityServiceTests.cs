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
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly FacilityService _sut; // SUT = System Under Test

    public FacilityServiceTests()
    {
        _facilityRepositoryMock = Substitute.For<IFacilityRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _sut = new FacilityService(_facilityRepositoryMock, _unitOfWorkMock);
    }

    #region GetCitiesAutocompleteAsync Tests

    [Fact]
    public async Task GetCitiesAutocompleteAsync_ShouldReturnList_WhenRepoReturnsData()
    {
        // Arrange
        var query = "War";
        var expectedCities = new List<string> { "Warszawa", "Warka" };

        _facilityRepositoryMock.GetCitiesAutocompleteAsync(query, Arg.Any<CancellationToken>())
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
    public async Task GetCitiesAutocompleteAsync_ShouldReturnEmptyList_WhenNoMatchesFound()
    {
        // Arrange
        var query = "NonExistentCity";
        var expectedCities = new List<string>();

        _facilityRepositoryMock.GetCitiesAutocompleteAsync(query, Arg.Any<CancellationToken>())
            .Returns(expectedCities);

        // Act
        var result = await _sut.GetCitiesAutocompleteAsync(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    #endregion

    #region GetFacilityNamesAutocompleteAsync Tests

    [Fact]
    public async Task GetFacilityNamesAutocompleteAsync_ShouldReturnDtoList_WhenRepoReturnsData()
    {
        // Arrange
        var query = "Przychodnia";
        var expectedDtos = new List<FacilityListDto>
        {
            new FacilityListDto { facilityId = 1, facilityName = "Przychodnia A" },
            new FacilityListDto { facilityId = 2, facilityName = "Przychodnia B" }
        };

        _facilityRepositoryMock.GetFacilityNamesAutocompleteAsync(query, Arg.Any<CancellationToken>())
            .Returns(expectedDtos);

        // Act
        var result = await _sut.GetFacilityNamesAutocompleteAsync(query, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedDtos);
        await _facilityRepositoryMock.Received(1)
            .GetFacilityNamesAutocompleteAsync(query, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetFacilityNamesAutocompleteAsync_ShouldReturnEmptyList_WhenNoMatches()
    {
        // Arrange
        var query = "NonExistent";

        _facilityRepositoryMock.GetFacilityNamesAutocompleteAsync(query, Arg.Any<CancellationToken>())
            .Returns(new List<FacilityListDto>());

        // Act
        var result = await _sut.GetFacilityNamesAutocompleteAsync(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    #endregion

    #region SearchFacilitiesAsync Tests

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
                phoneNumber = "123456789"
            }
        };

        var pagedResultFromRepo = new PagedResult<MedicalFacility>
        {
            Items = entitiesFromDb,
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };

        _facilityRepositoryMock.SearchFacilitiesAsync(query, Arg.Any<CancellationToken>())
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

        _facilityRepositoryMock.SearchFacilitiesAsync(query, Arg.Any<CancellationToken>())
            .Returns(emptyRepoResult);

        // Act
        var result = await _sut.SearchFacilitiesAsync(query, CancellationToken.None);

        // Assert
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task SearchFacilitiesAsync_ShouldCallRepository_WithCorrectQuery()
    {
        // Arrange
        var query = new GetFacilitiesQuery 
        { 
            PageNumber = 2, 
            PageSize = 20,
            Name = "Szpital",
            City = "Warsaw"
        };

        var emptyRepoResult = new PagedResult<MedicalFacility>
        {
            Items = new List<MedicalFacility>(),
            TotalCount = 0,
            PageNumber = 2,
            PageSize = 20
        };

        _facilityRepositoryMock.SearchFacilitiesAsync(query, Arg.Any<CancellationToken>())
            .Returns(emptyRepoResult);

        // Act
        var result = await _sut.SearchFacilitiesAsync(query, CancellationToken.None);

        // Assert
        await _facilityRepositoryMock.Received(1)
            .SearchFacilitiesAsync(query, Arg.Any<CancellationToken>());
    }

    #endregion

    #region GetFacilityByIdAsync Tests

    [Fact]
    public async Task GetFacilityByIdAsync_ShouldReturnFacilityDto_WhenFacilityExists()
    {
        // Arrange
        var facilityId = 5;
        var facility = new MedicalFacility
        {
            facilityId = facilityId,
            facilityName = "Central Hospital",
            street = "Main St 123",
            city = "Warsaw",
            postalCode = "00-001",
            country = "Poland",
            nip = "1234567890",
            phoneNumber = "123456789",
            creationDate = DateTime.UtcNow
        };

        _facilityRepositoryMock.GetByIdAsNoTrackingAsync(facilityId, Arg.Any<CancellationToken>())
            .Returns(facility);

        // Act
        var result = await _sut.GetFacilityByIdAsync(facilityId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(facilityId);
        result.Name.Should().Be("Central Hospital");
        result.Street.Should().Be("Main St 123");
        result.City.Should().Be("Warsaw");
        result.PhoneNumber.Should().Be("123456789");
    }

    [Fact]
    public async Task GetFacilityByIdAsync_ShouldReturnNull_WhenFacilityDoesNotExist()
    {
        // Arrange
        var facilityId = 999;

        _facilityRepositoryMock.GetByIdAsNoTrackingAsync(facilityId, Arg.Any<CancellationToken>())
            .Returns((MedicalFacility?)null);

        // Act
        var result = await _sut.GetFacilityByIdAsync(facilityId, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region CreateFacilityAsync Tests

    [Fact]
    public async Task CreateFacilityAsync_ShouldCreateFacility_WithValidDto()
    {
        // Arrange
        var dto = new CreateFacilityDto
        {
            FacilityName = "New Clinic",
            Street = "Park Ave 456",
            City = "Kraków",
            PostalCode = "30-001",
            Country = "Poland",
            Nip = "9876543210",
            PhoneNumber = "987654321"
        };

        var createdFacility = new MedicalFacility
        {
            facilityId = 1,
            facilityName = dto.FacilityName,
            street = dto.Street,
            city = dto.City,
            postalCode = dto.PostalCode,
            country = dto.Country,
            nip = dto.Nip,
            phoneNumber = dto.PhoneNumber,
            creationDate = DateTime.UtcNow
        };

        MedicalFacility? capturedFacility = null;
        await _facilityRepositoryMock.AddAsync(Arg.Do<MedicalFacility>(f => capturedFacility = f), Arg.Any<CancellationToken>());

        // Act
        var result = await _sut.CreateFacilityAsync(dto, CancellationToken.None);

        // Assert
        result.Should().Be(0); // The facility won't have a real ID until saved
        await _facilityRepositoryMock.Received(1).AddAsync(Arg.Any<MedicalFacility>(), Arg.Any<CancellationToken>());
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateFacilityAsync_ShouldSetDefaultCountry_WhenCountryNotProvided()
    {
        // Arrange
        var dto = new CreateFacilityDto
        {
            FacilityName = "Clinic Without Country",
            Street = "Tree St",
            City = "Gdańsk",
            PostalCode = "80-001",
            Nip = "1111111111",
            PhoneNumber = "111111111",
            Country = null
        };

        MedicalFacility? capturedFacility = null;
        await _facilityRepositoryMock.AddAsync(Arg.Do<MedicalFacility>(f => capturedFacility = f), Arg.Any<CancellationToken>());

        // Act
        await _sut.CreateFacilityAsync(dto, CancellationToken.None);

        // Assert
        capturedFacility.Should().NotBeNull();
        capturedFacility!.country.Should().Be("Polska");
    }

    #endregion

    #region UpdateFacilityAsync Tests

    [Fact]
    public async Task UpdateFacilityAsync_ShouldUpdateFacility_WhenFacilityExists()
    {
        // Arrange
        var facilityId = 3;
        var updateDto = new CreateFacilityDto
        {
            FacilityName = "Updated Clinic",
            Street = "New Street 999",
            City = "Wrocław",
            PostalCode = "50-001",
            Country = "Poland",
            Nip = "5555555555",
            PhoneNumber = "555555555"
        };

        var existingFacility = new MedicalFacility
        {
            facilityId = facilityId,
            facilityName = "Old Name",
            street = "Old Street",
            city = "Old City",
            postalCode = "00-000",
            country = "Poland",
            nip = "1111111111",
            phoneNumber = "111111111",
            creationDate = DateTime.UtcNow
        };

        _facilityRepositoryMock.GetByIdAsync(facilityId, Arg.Any<CancellationToken>())
            .Returns(existingFacility);

        // Act
        await _sut.UpdateFacilityAsync(facilityId, updateDto, CancellationToken.None);

        // Assert
        existingFacility.facilityName.Should().Be(updateDto.FacilityName);
        existingFacility.street.Should().Be(updateDto.Street);
        existingFacility.city.Should().Be(updateDto.City);
        existingFacility.postalCode.Should().Be(updateDto.PostalCode);
        existingFacility.nip.Should().Be(updateDto.Nip);
        existingFacility.phoneNumber.Should().Be(updateDto.PhoneNumber);

        _facilityRepositoryMock.Received(1).Update(existingFacility);
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateFacilityAsync_ShouldThrowKeyNotFoundException_WhenFacilityDoesNotExist()
    {
        // Arrange
        var facilityId = 999;
        var updateDto = new CreateFacilityDto
        {
            FacilityName = "Any Clinic",
            Street = "Any Street",
            City = "Any City",
            PostalCode = "00-000",
            Nip = "1111111111",
            PhoneNumber = "111111111"
        };

        _facilityRepositoryMock.GetByIdAsync(facilityId, Arg.Any<CancellationToken>())
            .Returns((MedicalFacility?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _sut.UpdateFacilityAsync(facilityId, updateDto, CancellationToken.None));

        exception.Message.Should().Contain("Facility not found");
    }

    [Fact]
    public async Task UpdateFacilityAsync_ShouldPreserveCountry_WhenCountryNotProvided()
    {
        // Arrange
        var facilityId = 7;
        var updateDto = new CreateFacilityDto
        {
            FacilityName = "Updated Facility",
            Street = "Updated St",
            City = "Updated City",
            PostalCode = "00-000",
            Nip = "7777777777",
            PhoneNumber = "777777777",
            Country = null
        };

        var existingFacility = new MedicalFacility
        {
            facilityId = facilityId,
            facilityName = "Old",
            street = "Old",
            city = "Old",
            postalCode = "00-000",
            country = "France",
            nip = "1111111111",
            phoneNumber = "111111111",
            creationDate = DateTime.UtcNow
        };

        _facilityRepositoryMock.GetByIdAsync(facilityId, Arg.Any<CancellationToken>())
            .Returns(existingFacility);

        // Act
        await _sut.UpdateFacilityAsync(facilityId, updateDto, CancellationToken.None);

        // Assert
        existingFacility.country.Should().Be("France");
    }

    #endregion
}
