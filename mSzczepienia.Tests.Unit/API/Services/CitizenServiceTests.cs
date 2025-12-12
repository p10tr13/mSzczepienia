using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Api.Services;
using mSzczepienia.Shared.DTO.Citizens;

namespace mSzczepienia.Tests.Unit.API.Services;

public class CitizenServiceTests
{
    private readonly ICitizenRepository _citizenRepositoryMock;
    private readonly CitizenService _sut; // System Under Test

    public CitizenServiceTests()
    {
        _citizenRepositoryMock = Substitute.For<ICitizenRepository>();
        _sut = new CitizenService(_citizenRepositoryMock);
    }

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
            submissionStatus = "Accepted"
        };

        _citizenRepositoryMock.GetCitizenDetailsByUidAsync(firebaseUid, Arg.Any<CancellationToken>()).Returns(expectedDto);

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

        _citizenRepositoryMock.GetCitizenDetailsByUidAsync(firebaseUid, Arg.Any<CancellationToken>()).Returns((CitizenDetailsDto?)null);

        // Act
        var result = await _sut.GetCitizenDetailsAsync(firebaseUid, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateCitizenAsync_ShouldCallRepository_WithCorrectParameters()
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
            submissionStatus = "Pending"
        };

        // Act
        await _sut.CreateCitizenAsync(firebaseUid, dto, CancellationToken.None);

        // Assert
        await _citizenRepositoryMock.Received(1).AddCitizenAsync(firebaseUid, dto, Arg.Any<CancellationToken>());
    }
}