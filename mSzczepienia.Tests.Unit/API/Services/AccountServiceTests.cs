using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Api.Services;
using mSzczepienia.Shared.DTO.Accounts;
using mSzczepienia.Shared.Enums;
using mSzczepienia.Shared.Models;

namespace mSzczepienia.Tests.Unit.API.Services;

public class AccountServiceTests
{
    private readonly IAccountProfileRepository _accountProfileRepositoryMock;
    private readonly AccountService _sut; // SUT = System Under Test

    public AccountServiceTests()
    {
        _accountProfileRepositoryMock = Substitute.For<IAccountProfileRepository>();
        _sut = new AccountService(_accountProfileRepositoryMock);
    }

    #region GetProfileAsync Tests

    [Fact]
    public async Task GetProfileAsync_ShouldReturnProfileDto_WhenAccountExists()
    {
        // Arrange
        var firebaseUid = "test-uid-12345";
        var profileDto = new ProfileDto
        {
            Name = "John",
            Surname = "Doe",
            Email = "john.doe@test.pl",
            DateOfBirth = new DateTime(1990, 1, 15),
            CreationDate = DateTime.UtcNow,
            Role = "HealthWorker"
        };

        _accountProfileRepositoryMock.GetProfileByFirebaseUidAsync(firebaseUid, Arg.Any<CancellationToken>())
            .Returns(profileDto);

        // Act
        var result = await _sut.GetProfileAsync(firebaseUid, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("John");
        result.Surname.Should().Be("Doe");
        result.Email.Should().Be("john.doe@test.pl");
        result.DateOfBirth.Should().Be(new DateTime(1990, 1, 15));
        result.Role.Should().Be("HealthWorker");
    }

    [Fact]
    public async Task GetProfileAsync_ShouldReturnNull_WhenAccountDoesNotExist()
    {
        // Arrange
        var firebaseUid = "non-existent-uid";

        _accountProfileRepositoryMock.GetProfileByFirebaseUidAsync(firebaseUid, Arg.Any<CancellationToken>())
            .Returns((ProfileDto?)null);

        // Act
        var result = await _sut.GetProfileAsync(firebaseUid, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    #endregion
}
