using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Api.Services;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Citizens;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.Enums;
using mSzczepienia.Shared.Models;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace mSzczepienia.Tests.Unit.API.Services;

public class CitizenServiceTests
{
    private readonly ICitizenRepository _citizenRepositoryMock;
    private readonly IAccountRepository _accountRepositoryMock;
    private readonly IPersonnelRepository _personnelRepositoryMock;
    private readonly IFacilityRepository _facilityRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly ILogger<CitizenService> _loggerMock;
    private readonly CitizenService _sut; // System Under Test

    public CitizenServiceTests()
    {
        _citizenRepositoryMock = Substitute.For<ICitizenRepository>();
        _accountRepositoryMock = Substitute.For<IAccountRepository>();
        _personnelRepositoryMock = Substitute.For<IPersonnelRepository>();
        _facilityRepositoryMock = Substitute.For<IFacilityRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _loggerMock = Substitute.For<ILogger<CitizenService>>();

        _sut = new CitizenService(
            _citizenRepositoryMock,
            _accountRepositoryMock,
            _personnelRepositoryMock,
            _facilityRepositoryMock,
            _unitOfWorkMock,
            _loggerMock);
    }

    #region GetCitizenDetailsAsync Tests

    [Fact]
    public async Task GetCitizenDetailsAsync_ShouldReturnDto_WhenCitizenExists()
    {
        // Arrange
        var firebaseUid = "user-123";
        var expectedDto = new CitizenDetailsDto
        {
            name = "Joe",
            surname = "Doe",
            email = "joe@test.pl",
            dateOfBirth = new DateTime(1990, 1, 1),
            creationDate = DateTime.UtcNow,
            pesel = "90010112345",
            facilityName = "Przychodnia Rejonowa",
            submissionStatus = "NotUploaded"
        };

        _citizenRepositoryMock.GetCitizenDetailsByUidAsync(firebaseUid, Arg.Any<CancellationToken>())
            .Returns(expectedDto);

        // Act
        var result = await _sut.GetCitizenDetailsAsync(firebaseUid, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedDto);
        await _citizenRepositoryMock.Received(1).GetCitizenDetailsByUidAsync(firebaseUid, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetCitizenDetailsAsync_ShouldReturnNull_WhenCitizenDoesNotExist()
    {
        // Arrange
        var firebaseUid = "unknown-user";

        _citizenRepositoryMock.GetCitizenDetailsByUidAsync(firebaseUid, Arg.Any<CancellationToken>())
            .Returns((CitizenDetailsDto?)null);

        // Act
        var result = await _sut.GetCitizenDetailsAsync(firebaseUid, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region GetCitizenDetailsByIdAsync Tests

    [Fact]
    public async Task GetCitizenDetailsByIdAsync_ShouldReturnDto_WhenCitizenExists()
    {
        // Arrange
        var requesterUid = "admin-123";
        var citizenId = 1;
        var expectedDto = new CitizenDetailsDto
        {
            name = "Jane",
            surname = "Smith",
            email = "jane@test.pl",
            dateOfBirth = new DateTime(1995, 5, 15),
            creationDate = DateTime.UtcNow,
            pesel = "95051512345",
            facilityName = "Szpital Centralny",
            submissionStatus = "Accepted"
        };

        _citizenRepositoryMock.GetCitizenDetailsByIdAsync(citizenId, Arg.Any<CancellationToken>())
            .Returns(expectedDto);

        // Act
        var result = await _sut.GetCitizenDetailsByIdAsync(requesterUid, citizenId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedDto);
        await _citizenRepositoryMock.Received(1).GetCitizenDetailsByIdAsync(citizenId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetCitizenDetailsByIdAsync_ShouldReturnNull_WhenCitizenDoesNotExist()
    {
        // Arrange
        var requesterUid = "admin-123";
        var citizenId = 999;

        _citizenRepositoryMock.GetCitizenDetailsByIdAsync(citizenId, Arg.Any<CancellationToken>())
            .Returns((CitizenDetailsDto?)null);

        // Act
        var result = await _sut.GetCitizenDetailsByIdAsync(requesterUid, citizenId, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region CreateCitizenAsync Tests

    [Fact]
    public async Task CreateCitizenAsync_ShouldCreateCitizen_WhenFacilityExists()
    {
        // Arrange
        var firebaseUid = "new-user-uid";
        var dto = new CitizenDetailsDto
        {
            name = "Joe",
            surname = "Doe",
            email = "joe@test.pl",
            dateOfBirth = new DateTime(2003, 1, 1),
            creationDate = DateTime.UtcNow,
            pesel = "03310512345",
            facilityName = "Szpital Miejski",
            submissionStatus = "NotUploaded"
        };

        var facility = new MedicalFacility 
        { 
            facilityId = 1, 
            facilityName = "Szpital Miejski",
            street = "Main St",
            postalCode = "00-001",
            city = "Warsaw",
            country = "Poland",
            nip = "1234567890",
            phoneNumber = "123456789"
        };

        _facilityRepositoryMock.GetByNameAsync(dto.facilityName, Arg.Any<CancellationToken>())
            .Returns(facility);

        // Act
        await _sut.CreateCitizenAsync(firebaseUid, dto, CancellationToken.None);

        // Assert
        await _accountRepositoryMock.Received(1).AddAsync(Arg.Any<Account>(), Arg.Any<CancellationToken>());
        await _citizenRepositoryMock.Received(1).AddAsync(Arg.Any<Citizen>(), Arg.Any<CancellationToken>());
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateCitizenAsync_ShouldThrowInvalidOperationException_WhenFacilityDoesNotExist()
    {
        // Arrange
        var firebaseUid = "new-user-uid";
        var dto = new CitizenDetailsDto
        {
            name = "Joe",
            surname = "Doe",
            email = "joe@test.pl",
            dateOfBirth = new DateTime(2003, 1, 1),
            creationDate = DateTime.UtcNow,
            pesel = "03310512345",
            facilityName = "NonExistentFacility",
            submissionStatus = "NotUploaded"
        };

        _facilityRepositoryMock.GetByNameAsync(dto.facilityName, Arg.Any<CancellationToken>())
            .Returns((MedicalFacility?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.CreateCitizenAsync(firebaseUid, dto, CancellationToken.None));

        exception.Message.Should().Contain("Facility has not been found");
    }

    [Fact]
    public async Task CreateCitizenAsync_ShouldSetCorrectCitizenProperties()
    {
        // Arrange
        var firebaseUid = "new-user-uid";
        var dto = new CitizenDetailsDto
        {
            name = "John",
            surname = "Kowalski",
            email = "john@test.pl",
            dateOfBirth = new DateTime(2000, 6, 15),
            creationDate = DateTime.UtcNow,
            pesel = "00061512345",
            facilityName = "Klinika Zdrowia",
            submissionStatus = "NotUploaded"
        };

        var facility = new MedicalFacility 
        { 
            facilityId = 2, 
            facilityName = "Klinika Zdrowia",
            street = "Health Ave",
            postalCode = "00-002",
            city = "Warsaw",
            country = "Poland",
            nip = "9876543210",
            phoneNumber = "987654321"
        };

        _facilityRepositoryMock.GetByNameAsync(dto.facilityName, Arg.Any<CancellationToken>())
            .Returns(facility);

        Citizen? capturedCitizen = null;
        await _citizenRepositoryMock.AddAsync(Arg.Do<Citizen>(c => capturedCitizen = c), Arg.Any<CancellationToken>());

        // Act
        await _sut.CreateCitizenAsync(firebaseUid, dto, CancellationToken.None);

        // Assert
        capturedCitizen.Should().NotBeNull();
        capturedCitizen!.pesel.Should().Be(dto.pesel);
        capturedCitizen.submissionStatus.Should().Be(SubmissionStatus.NotUploaded);
        capturedCitizen.medicalFacility.Should().BeEquivalentTo(facility);
    }

    #endregion

    #region RegisterDeviceTokenAsync Tests

    [Fact]
    public async Task RegisterDeviceTokenAsync_ShouldCallRepository_WithCorrectParameters()
    {
        // Arrange
        var firebaseUid = "user-123";
        var deviceToken = "device-token-xyz";
        var language = "pl";

        // Act
        await _sut.RegisterDeviceTokenAsync(firebaseUid, deviceToken, language, CancellationToken.None);

        // Assert
        await _citizenRepositoryMock.Received(1)
            .RegisterDeviceTokenAsync(firebaseUid, deviceToken, language, Arg.Any<CancellationToken>());
    }

    #endregion

    #region SearchCitizensAsync Tests

    [Fact]
    public async Task SearchCitizensAsync_ShouldReturnSearchResults_WhenValidQueryProvided()
    {
        // Arrange
        var user = CreateClaimsPrincipalWithFirebaseUid("user-123");
        var query = new GetCitizensQuery
        {
            Name = "Joe",
            Surname = "",
            Pesel = "",
            PageNumber = 1,
            PageSize = 10,
            SearchInWorkersFacilityOnly = false
        };

        var expectedResult = new PagedResult<SimpleCitizenDto>
        {
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10,
            Items = new List<SimpleCitizenDto>
            {
                new SimpleCitizenDto
                {
                    Id = 1,
                    Name = "Joe",
                    Surname = "Doe",
                    Pesel = "90010112345",
                    DateOfBirth = new DateTime(1990, 1, 1),
                    SubmissionStatus = "NotUploaded"
                }
            }
        };

        _citizenRepositoryMock.SearchCitizensAsync(query, null, Arg.Any<CancellationToken>())
            .Returns(expectedResult);

        // Act
        var result = await _sut.SearchCitizensAsync(user, query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.TotalCount.Should().Be(1);
        result.Items.Should().HaveCount(1);
        result.Items.First().Name.Should().Be("Joe");
    }

    [Fact]
    public async Task SearchCitizensAsync_ShouldReturnEmptyResult_WhenNoUserUidFound()
    {
        // Arrange
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("some-other-claim", "value") }));
        var query = new GetCitizensQuery
        {
            Name = "Joe",
            Surname = "",
            Pesel = "",
            PageNumber = 1,
            PageSize = 10,
            SearchInWorkersFacilityOnly = false
        };

        // Act
        var result = await _sut.SearchCitizensAsync(user, query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task SearchCitizensAsync_ShouldReturnEmptyResult_WhenWorkerHasNoFacility()
    {
        // Arrange
        var user = CreateClaimsPrincipalWithFirebaseUid("worker-uid");
        var query = new GetCitizensQuery
        {
            Name = "",
            Surname = "",
            Pesel = "",
            PageNumber = 1,
            PageSize = 10,
            SearchInWorkersFacilityOnly = true
        };

        _personnelRepositoryMock.GetFacilityIdForWorkerByFirebaseUidAsync("worker-uid", Arg.Any<CancellationToken>())
            .Returns((int?)null);

        // Act
        var result = await _sut.SearchCitizensAsync(user, query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        await _citizenRepositoryMock.DidNotReceive()
            .SearchCitizensAsync(Arg.Any<GetCitizensQuery>(), Arg.Any<int?>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SearchCitizensAsync_ShouldPassFacilityIdToRepository_WhenSearchInWorkersFacilityOnly()
    {
        // Arrange
        var user = CreateClaimsPrincipalWithFirebaseUid("worker-uid");
        var facilityId = 5;
        var query = new GetCitizensQuery
        {
            Name = "",
            Surname = "",
            Pesel = "",
            PageNumber = 1,
            PageSize = 10,
            SearchInWorkersFacilityOnly = true
        };

        var expectedResult = new PagedResult<SimpleCitizenDto>
        {
            TotalCount = 0,
            PageNumber = 1,
            PageSize = 10,
            Items = new List<SimpleCitizenDto>()
        };

        _personnelRepositoryMock.GetFacilityIdForWorkerByFirebaseUidAsync("worker-uid", Arg.Any<CancellationToken>())
            .Returns(facilityId);

        _citizenRepositoryMock.SearchCitizensAsync(query, facilityId, Arg.Any<CancellationToken>())
            .Returns(expectedResult);

        // Act
        var result = await _sut.SearchCitizensAsync(user, query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        await _citizenRepositoryMock.Received(1)
            .SearchCitizensAsync(query, facilityId, Arg.Any<CancellationToken>());
    }

    #endregion

    #region GetCitizenSubmissionStatusAsync Tests

    [Fact]
    public async Task GetCitizenSubmissionStatusAsync_ShouldReturnStatus_WhenCitizenExists()
    {
        // Arrange
        var firebaseUid = "user-123";
        var expectedStatus = new CitizenSubmissionStatusDto { submissionStatus = "Accepted" };

        _citizenRepositoryMock.GetCitizenSubmissionStatusAsync(firebaseUid, Arg.Any<CancellationToken>())
            .Returns(expectedStatus);

        // Act
        var result = await _sut.GetCitizenSubmissionStatusAsync(firebaseUid, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.submissionStatus.Should().Be("Accepted");
    }

    [Fact]
    public async Task GetCitizenSubmissionStatusAsync_ShouldCallRepository_WithCorrectParameters()
    {
        // Arrange
        var firebaseUid = "user-456";
        var expectedStatus = new CitizenSubmissionStatusDto { submissionStatus = "Pending" };

        _citizenRepositoryMock.GetCitizenSubmissionStatusAsync(firebaseUid, Arg.Any<CancellationToken>())
            .Returns(expectedStatus);

        // Act
        await _sut.GetCitizenSubmissionStatusAsync(firebaseUid, CancellationToken.None);

        // Assert
        await _citizenRepositoryMock.Received(1)
            .GetCitizenSubmissionStatusAsync(firebaseUid, Arg.Any<CancellationToken>());
    }

    #endregion

    #region Helper Methods

    private static ClaimsPrincipal CreateClaimsPrincipalWithFirebaseUid(string firebaseUid)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, firebaseUid)
        };
        var identity = new ClaimsIdentity(claims, "test");
        return new ClaimsPrincipal(identity);
    }

    #endregion
}