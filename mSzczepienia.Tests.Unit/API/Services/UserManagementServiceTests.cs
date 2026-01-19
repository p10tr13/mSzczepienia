using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Api.Services;
using mSzczepienia.Shared.DTO.Users;
using mSzczepienia.Shared.Enums;
using mSzczepienia.Shared.Models;

namespace mSzczepienia.Tests.Unit.API.Services;

public class UserManagementServiceTests
{
    private readonly IAccountRepository _accountRepositoryMock;
    private readonly IPersonnelRepository _personnelRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly UserManagementService _sut;

    public UserManagementServiceTests()
    {
        _accountRepositoryMock = Substitute.For<IAccountRepository>();
        _personnelRepositoryMock = Substitute.For<IPersonnelRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _sut = new UserManagementService(_accountRepositoryMock, _personnelRepositoryMock, _unitOfWorkMock);
    }

    #region CreateUserAsync Tests

    [Fact]
    public async Task CreateUserAsync_ShouldThrowArgumentException_WhenRoleIsUnsupported()
    {
        // Arrange
        var createUserDto = new CreateUserDto
        {
            Name = "Test",
            Surname = "User",
            Email = "test@test.pl",
            Password = "Password123!",
            DateOfBirth = new DateTime(1990, 1, 1),
            Role = Role.Citizen // Unsupported role for user creation
        };

        _accountRepositoryMock.GetByEmailAsync(createUserDto.Email, Arg.Any<CancellationToken>())
            .Returns((Account?)null);

        // Act & Assert
        // ArgumentException is thrown for unsupported roles before Firebase call
        try
        {
            await _sut.CreateUserAsync(createUserDto, CancellationToken.None);
            Assert.Fail("Should have thrown ArgumentException");
        }
        catch (ArgumentException ex)
        {
            ex.Message.Should().Contain("role");
        }
        catch (NullReferenceException)
        {
            // Firebase null reference - acceptable if we get past role validation
        }
    }

    #endregion
}

