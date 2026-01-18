using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Api.Services;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Personnel;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.Enums;
using mSzczepienia.Shared.Models;

namespace mSzczepienia.Tests.Unit.API.Services;

public class PersonnelServiceTests
{
    private readonly IPersonnelRepository _personnelRepositoryMock;
    private readonly IFacilityRepository _facilityRepositoryMock;
    private readonly IAccountRepository _accountRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly PersonnelService _sut;

    public PersonnelServiceTests()
    {
        _personnelRepositoryMock = Substitute.For<IPersonnelRepository>();
        _facilityRepositoryMock = Substitute.For<IFacilityRepository>();
        _accountRepositoryMock = Substitute.For<IAccountRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _sut = new PersonnelService(_personnelRepositoryMock, _facilityRepositoryMock, _accountRepositoryMock, _unitOfWorkMock);
    }

    #region GetPersonnelAsync Tests

    [Fact]
    public async Task GetPersonnelAsync_ShouldReturnPagedPersonnel_WhenRepositoryReturnsData()
    {
        // Arrange
        var query = new GetPersonnelQuery { PageNumber = 1, PageSize = 10 };
        var expectedResult = new PagedResult<PersonnelDto>
        {
            Items = new List<PersonnelDto>
            {
                new PersonnelDto 
                { 
                    Id = 1, 
                    Name = "John",
                    Surname = "Doe",
                    Email = "john@test.pl"
                }
            },
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };

        _personnelRepositoryMock.GetFacilityPersonnelAsync(query, Arg.Any<CancellationToken>())
            .Returns(expectedResult);

        // Act
        var result = await _sut.GetPersonnelAsync(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        result.Items.First().Name.Should().Be("John");
    }

    [Fact]
    public async Task GetPersonnelAsync_ShouldReturnEmptyList_WhenNoPersonnelFound()
    {
        // Arrange
        var query = new GetPersonnelQuery { PageNumber = 1, PageSize = 10 };
        var emptyResult = new PagedResult<PersonnelDto>
        {
            Items = new List<PersonnelDto>(),
            TotalCount = 0,
            PageNumber = 1,
            PageSize = 10
        };

        _personnelRepositoryMock.GetFacilityPersonnelAsync(query, Arg.Any<CancellationToken>())
            .Returns(emptyResult);

        // Act
        var result = await _sut.GetPersonnelAsync(query, CancellationToken.None);

        // Assert
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    #endregion

    #region SearchPersonnelAsync Tests

    [Fact]
    public async Task SearchPersonnelAsync_ShouldMapAndReturnPersonnelDtos_WhenResultsFound()
    {
        // Arrange
        var query = new SearchPersonnelQuery { Page = 1, PageSize = 10, FirstName = "Jane" };
        
        var personnel = new List<Personnel>
        {
            new Personnel
            {
                personnelId = 1,
                accountId = Guid.NewGuid(),
                facilityId = 1,
                pwzNumber = "PWZ123456",
                position = Position.Doctor,
                account = new Account
                {
                    accountId = Guid.NewGuid(),
                    name = "Jane",
                    surname = "Doe",
                    email = "jane@test.pl",
                    dateOfBirth = new DateTime(1990, 1, 1),
                    creationDate = DateTime.UtcNow,
                    firebaseUid = "uid-jane",
                    role = Role.HealthWorker
                }
            }
        };

        var pagedResult = new PagedResult<Personnel>
        {
            Items = personnel,
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };

        _personnelRepositoryMock.SearchPersonnelAsync(query, Arg.Any<CancellationToken>())
            .Returns(pagedResult);

        // Act
        var result = await _sut.SearchPersonnelAsync(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        result.Items.First().Name.Should().Be("Jane");
    }

    [Fact]
    public async Task SearchPersonnelAsync_ShouldReturnEmptyList_WhenNoMatchesFound()
    {
        // Arrange
        var query = new SearchPersonnelQuery { Page = 1, PageSize = 10 };
        var emptyResult = new PagedResult<Personnel>
        {
            Items = new List<Personnel>(),
            TotalCount = 0,
            PageNumber = 1,
            PageSize = 10
        };

        _personnelRepositoryMock.SearchPersonnelAsync(query, Arg.Any<CancellationToken>())
            .Returns(emptyResult);

        // Act
        var result = await _sut.SearchPersonnelAsync(query, CancellationToken.None);

        // Assert
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    #endregion

    #region GetDetailedPersonnelByIdAsync Tests

    [Fact]
    public async Task GetDetailedPersonnelByIdAsync_ShouldReturnDetailedDto_WhenPersonnelExists()
    {
        // Arrange
        var personnelId = 1;
        var accountId = Guid.NewGuid();
        var facility = new MedicalFacility
        {
            facilityId = 1,
            facilityName = "Test Hospital",
            city = "Warsaw",
            street = "Main St",
            postalCode = "00-001",
            country = "Poland",
            nip = "1234567890",
            phoneNumber = "123456789"
        };
        
        var personnel = new Personnel
        {
            personnelId = personnelId,
            accountId = accountId,
            facilityId = 1,
            pwzNumber = "PWZ123456",
            position = Position.Doctor,
            account = new Account
            {
                accountId = accountId,
                name = "John",
                surname = "Smith",
                email = "john.smith@test.pl",
                dateOfBirth = new DateTime(1985, 5, 15),
                creationDate = DateTime.UtcNow,
                firebaseUid = "uid-john",
                role = Role.HealthWorker
            },
            medicalFacility = facility
        };

        _personnelRepositoryMock.GetByIdAsync(personnelId, Arg.Any<CancellationToken>())
            .Returns(personnel);
        _facilityRepositoryMock.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(facility);

        // Act
        var result = await _sut.GetDetailedPersonnelByIdAsync(personnelId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(accountId);
        result.Name.Should().Be("John");
        result.Surname.Should().Be("Smith");
    }

    [Fact]
    public async Task GetDetailedPersonnelByIdAsync_ShouldThrowKeyNotFoundException_WhenPersonnelNotFound()
    {
        // Arrange
        var personnelId = 999;

        _personnelRepositoryMock.GetByIdAsync(personnelId, Arg.Any<CancellationToken>())
            .Returns((Personnel?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _sut.GetDetailedPersonnelByIdAsync(personnelId, CancellationToken.None));
    }

    #endregion

    #region UpdatePersonnelAsync Tests

    [Fact]
    public async Task UpdatePersonnelAsync_ShouldThrowKeyNotFoundException_WhenPersonnelNotFound()
    {
        // Arrange
        var personnelId = 999;
        var updateDto = new UpdatePersonnelDto
        {
            Name = "Test",
            Surname = "Test",
            Email = "test@test.pl",
            DateOfBirth = new DateTime(1990, 1, 1),
            PwzNumber = "PWZ123",
            Position = Position.Doctor
        };

        _personnelRepositoryMock.GetByIdAsync(personnelId, Arg.Any<CancellationToken>())
            .Returns((Personnel?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _sut.UpdatePersonnelAsync(personnelId, updateDto, CancellationToken.None));
    }

    [Fact]
    public async Task UpdatePersonnelAsync_ShouldThrowInvalidOperationException_WhenEmailAlreadyInUse()
    {
        // Arrange
        var personnelId = 1;
        var updateDto = new UpdatePersonnelDto
        {
            Email = "existing@test.pl",
            Position = Position.Doctor
        };

        var personnel = new Personnel
        {
            personnelId = personnelId,
            accountId = Guid.NewGuid(),
            facilityId = 1,
            pwzNumber = "PWZ123456",
            position = Position.Nurse,
            account = new Account
            {
                accountId = Guid.NewGuid(),
                name = "John",
                surname = "Doe",
                email = "john@test.pl",
                dateOfBirth = new DateTime(1990, 1, 1),
                creationDate = DateTime.UtcNow,
                firebaseUid = "uid-john",
                role = Role.HealthWorker
            }
        };

        var existingAccount = new Account
        {
            accountId = Guid.NewGuid(),
            name = "Jane",
            surname = "Smith",
            email = "existing@test.pl",
            dateOfBirth = new DateTime(1990, 1, 1),
            creationDate = DateTime.UtcNow,
            firebaseUid = "uid-jane",
            role = Role.HealthWorker
        };

        _personnelRepositoryMock.GetByIdAsync(personnelId, Arg.Any<CancellationToken>())
            .Returns(personnel);

        _accountRepositoryMock.GetByEmailAsync(updateDto.Email, Arg.Any<CancellationToken>())
            .Returns(existingAccount);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.UpdatePersonnelAsync(personnelId, updateDto, CancellationToken.None));
    }

    #endregion
}
