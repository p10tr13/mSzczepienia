using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Api.Services;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Accounts;
using mSzczepienia.Shared.DTO.Admins;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.Enums;
using mSzczepienia.Shared.Models;

namespace mSzczepienia.Tests.Unit.API.Services;

public class AdminServiceTests
{
    private readonly IAdminRepository _adminRepositoryMock;
    private readonly IAccountRepository _accountRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly AdminService _sut; // SUT = System Under Test

    public AdminServiceTests()
    {
        _adminRepositoryMock = Substitute.For<IAdminRepository>();
        _accountRepositoryMock = Substitute.For<IAccountRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _sut = new AdminService(_adminRepositoryMock, _accountRepositoryMock, _unitOfWorkMock);
    }

    #region SearchAdminsAsync Tests

    [Fact]
    public async Task SearchAdminsAsync_ShouldReturnAdminDtos_WhenAdminsFound()
    {
        // Arrange
        var query = new SearchAdminQuery { Page = 1, PageSize = 10, FirstName = "Admin" };
        
        var adminAccounts = new List<Account>
        {
            new Account
            {
                accountId = Guid.NewGuid(),
                name = "Admin",
                surname = "User",
                email = "admin@test.pl",
                dateOfBirth = new DateTime(1990, 1, 1),
                creationDate = DateTime.UtcNow,
                firebaseUid = "uid-admin-1",
                role = Role.Admin
            }
        };

        var pagedResult = new PagedResult<Account>
        {
            Items = adminAccounts,
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };

        _adminRepositoryMock.SearchAdminsAsync(query, Arg.Any<CancellationToken>())
            .Returns(pagedResult);

        // Act
        var result = await _sut.SearchAdminsAsync(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        result.Items.First().Name.Should().Be("Admin");
        result.Items.First().Email.Should().Be("admin@test.pl");
        result.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task SearchAdminsAsync_ShouldReturnEmptyList_WhenNoAdminsFound()
    {
        // Arrange
        var query = new SearchAdminQuery { Page = 1, PageSize = 10 };
        var emptyResult = new PagedResult<Account>
        {
            Items = new List<Account>(),
            TotalCount = 0,
            PageNumber = 1,
            PageSize = 10
        };

        _adminRepositoryMock.SearchAdminsAsync(query, Arg.Any<CancellationToken>())
            .Returns(emptyResult);

        // Act
        var result = await _sut.SearchAdminsAsync(query, CancellationToken.None);

        // Assert
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    #endregion

    #region GetAdminByIdAsync Tests

    [Fact]
    public async Task GetAdminByIdAsync_ShouldReturnDetailedAccountDto_WhenAdminExists()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var admin = new Account
        {
            accountId = adminId,
            name = "Admin",
            surname = "Test",
            email = "admin@test.pl",
            dateOfBirth = new DateTime(1990, 1, 1),
            creationDate = DateTime.UtcNow,
            firebaseUid = "uid-admin",
            role = Role.Admin
        };

        _accountRepositoryMock.GetByIdAsync(adminId, Arg.Any<CancellationToken>())
            .Returns(admin);

        // Act
        var result = await _sut.GetAdminByIdAsync(adminId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(adminId);
        result.Name.Should().Be("Admin");
        result.Email.Should().Be("admin@test.pl");
        result.Role.Should().Be(Role.Admin);
    }

    [Fact]
    public async Task GetAdminByIdAsync_ShouldThrowKeyNotFoundException_WhenAdminDoesNotExist()
    {
        // Arrange
        var adminId = Guid.NewGuid();

        _accountRepositoryMock.GetByIdAsync(adminId, Arg.Any<CancellationToken>())
            .Returns((Account?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _sut.GetAdminByIdAsync(adminId, CancellationToken.None));
    }

    [Fact]
    public async Task GetAdminByIdAsync_ShouldThrowKeyNotFoundException_WhenAccountIsNotAdmin()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var account = new Account
        {
            accountId = adminId,
            name = "Worker",
            surname = "Test",
            email = "worker@test.pl",
            dateOfBirth = new DateTime(1990, 1, 1),
            creationDate = DateTime.UtcNow,
            firebaseUid = "uid-worker",
            role = Role.HealthWorker
        };

        _accountRepositoryMock.GetByIdAsync(adminId, Arg.Any<CancellationToken>())
            .Returns(account);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _sut.GetAdminByIdAsync(adminId, CancellationToken.None));
    }

    #endregion

    #region UpdateAdminAsync Tests

    [Fact]
    public async Task UpdateAdminAsync_ShouldThrowKeyNotFoundException_WhenAdminDoesNotExist()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var updateDto = new UpdateAdminDto
        {
            Name = "Test",
            Surname = "Test",
            Email = "test@test.pl",
            DateOfBirth = new DateTime(1990, 1, 1)
        };

        _accountRepositoryMock.GetByIdAsync(adminId, Arg.Any<CancellationToken>())
            .Returns((Account?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _sut.UpdateAdminAsync(adminId, updateDto, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateAdminAsync_ShouldThrowInvalidOperationException_WhenEmailAlreadyInUse()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var updateDto = new UpdateAdminDto
        {
            Name = "Updated",
            Surname = "Admin",
            Email = "existing@test.pl",
            DateOfBirth = new DateTime(1990, 1, 1)
        };

        var admin = new Account
        {
            accountId = adminId,
            name = "Old Name",
            surname = "Old Surname",
            email = "old@test.pl",
            dateOfBirth = new DateTime(1990, 1, 1),
            creationDate = DateTime.UtcNow,
            firebaseUid = "uid-admin",
            role = Role.Admin
        };

        var existingAdmin = new Account
        {
            accountId = Guid.NewGuid(),
            name = "Existing",
            surname = "Admin",
            email = "existing@test.pl",
            dateOfBirth = new DateTime(1990, 1, 1),
            creationDate = DateTime.UtcNow,
            firebaseUid = "uid-other",
            role = Role.Admin
        };

        _accountRepositoryMock.GetByIdAsync(adminId, Arg.Any<CancellationToken>())
            .Returns(admin);

        _accountRepositoryMock.GetByEmailAsync(updateDto.Email, Arg.Any<CancellationToken>())
            .Returns(existingAdmin);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.UpdateAdminAsync(adminId, updateDto, CancellationToken.None));
    }

    #endregion
}
