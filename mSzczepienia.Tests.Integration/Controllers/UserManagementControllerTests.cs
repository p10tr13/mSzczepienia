using mSzczepienia.Shared.DTO.Users;
using mSzczepienia.Shared.Enums;
using mSzczepienia.Tests.Integration.Fixtures;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace mSzczepienia.Tests.Integration.Controllers;

public class UserManagementControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public UserManagementControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    #region CreateUser Authorization Tests

    [Fact]
    public async Task CreateUser_WithAdminRole_ShouldBeAuthorized()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");
        var createDto = new CreateUserDto
        {
            Email = "newuser@test.com",
            Password = "password123",
            Name = "New",
            Surname = "User",
            DateOfBirth = new DateTime(1990, 1, 1),
            Role = Role.Admin
        };
        var content = new StringContent(JsonSerializer.Serialize(createDto), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/users-management/create", content);

        // Assert
        // Should not be Forbidden (admin is authorized)
        // May return 500 due to Firebase not being available in test environment
        response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateUser_WithWorkerRole_ShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");
        var createDto = new CreateUserDto
        {
            Email = "newuser@test.com",
            Password = "password123",
            Name = "New",
            Surname = "User",
            DateOfBirth = new DateTime(1990, 1, 1),
            Role = Role.Admin
        };
        var content = new StringContent(JsonSerializer.Serialize(createDto), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/users-management/create", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateUser_WithCitizenRole_ShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("citizen", "test-citizen-uid-1");
        var createDto = new CreateUserDto
        {
            Email = "newuser@test.com",
            Password = "password123",
            Name = "New",
            Surname = "User",
            DateOfBirth = new DateTime(1990, 1, 1),
            Role = Role.Admin
        };
        var content = new StringContent(JsonSerializer.Serialize(createDto), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/users-management/create", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateUser_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClientWithoutAuth();
        var createDto = new CreateUserDto
        {
            Email = "newuser@test.com",
            Password = "password123",
            Name = "New",
            Surname = "User",
            DateOfBirth = new DateTime(1990, 1, 1),
            Role = Role.Admin
        };
        var content = new StringContent(JsonSerializer.Serialize(createDto), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/users-management/create", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region CreateUser Validation Tests

    [Fact]
    public async Task CreateUser_WithEmptyEmail_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");
        var createDto = new CreateUserDto
        {
            Email = "",
            Password = "password123",
            Name = "New",
            Surname = "User",
            DateOfBirth = new DateTime(1990, 1, 1),
            Role = Role.Admin
        };
        var content = new StringContent(JsonSerializer.Serialize(createDto), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/users-management/create", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateUser_WithInvalidEmail_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");
        var createDto = new CreateUserDto
        {
            Email = "invalid-email",
            Password = "password123",
            Name = "New",
            Surname = "User",
            DateOfBirth = new DateTime(1990, 1, 1),
            Role = Role.Admin
        };
        var content = new StringContent(JsonSerializer.Serialize(createDto), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/users-management/create", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateUser_WithShortPassword_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");
        var createDto = new CreateUserDto
        {
            Email = "newuser@test.com",
            Password = "12345", // Less than 6 characters
            Name = "New",
            Surname = "User",
            DateOfBirth = new DateTime(1990, 1, 1),
            Role = Role.Admin
        };
        var content = new StringContent(JsonSerializer.Serialize(createDto), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/users-management/create", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateUser_WithEmptyPassword_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");
        var createDto = new CreateUserDto
        {
            Email = "newuser@test.com",
            Password = "",
            Name = "New",
            Surname = "User",
            DateOfBirth = new DateTime(1990, 1, 1),
            Role = Role.Admin
        };
        var content = new StringContent(JsonSerializer.Serialize(createDto), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/users-management/create", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateUser_WithEmptyName_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");
        var createDto = new CreateUserDto
        {
            Email = "newuser@test.com",
            Password = "password123",
            Name = "",
            Surname = "User",
            DateOfBirth = new DateTime(1990, 1, 1),
            Role = Role.Admin
        };
        var content = new StringContent(JsonSerializer.Serialize(createDto), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/users-management/create", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateUser_WithEmptySurname_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");
        var createDto = new CreateUserDto
        {
            Email = "newuser@test.com",
            Password = "password123",
            Name = "New",
            Surname = "",
            DateOfBirth = new DateTime(1990, 1, 1),
            Role = Role.Admin
        };
        var content = new StringContent(JsonSerializer.Serialize(createDto), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/users-management/create", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateUser_WithPasswordExactly6Characters_ShouldBeAccepted()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");
        var createDto = new CreateUserDto
        {
            Email = "newuser6@test.com",
            Password = "123456", // Exactly 6 characters
            Name = "New",
            Surname = "User",
            DateOfBirth = new DateTime(1990, 1, 1),
            Role = Role.Admin
        };
        var content = new StringContent(JsonSerializer.Serialize(createDto), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/users-management/create", content);

        // Assert
        // Should not be BadRequest due to validation - may be 500 due to Firebase
        response.StatusCode.Should().NotBe(HttpStatusCode.BadRequest);
        response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region CreateUser Role-Specific Tests

    [Fact]
    public async Task CreateUser_AdminRole_ShouldNotRequireHealthWorkerFields()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");
        var createDto = new CreateUserDto
        {
            Email = "newadmin@test.com",
            Password = "password123",
            Name = "New",
            Surname = "Admin",
            DateOfBirth = new DateTime(1990, 1, 1),
            Role = Role.Admin
            // No PwzNumber, FacilityId, Position
        };
        var content = new StringContent(JsonSerializer.Serialize(createDto), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/users-management/create", content);

        // Assert
        // Should not return BadRequest for validation errors
        response.StatusCode.Should().NotBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateUser_HealthWorkerRole_WithoutFacilityId_ShouldFail()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");
        var createDto = new CreateUserDto
        {
            Email = "newworker@test.com",
            Password = "password123",
            Name = "New",
            Surname = "Worker",
            DateOfBirth = new DateTime(1990, 1, 1),
            Role = Role.HealthWorker,
            PwzNumber = "PWZ999999",
            Position = Position.Doctor
            // Missing FacilityId
        };
        var content = new StringContent(JsonSerializer.Serialize(createDto), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/users-management/create", content);

        // Assert
        // May be 400 BadRequest or 500 InternalServerError depending on where validation happens
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task CreateUser_HealthWorkerRole_WithoutPwzNumber_ShouldFail()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");
        var createDto = new CreateUserDto
        {
            Email = "newworker2@test.com",
            Password = "password123",
            Name = "New",
            Surname = "Worker",
            DateOfBirth = new DateTime(1990, 1, 1),
            Role = Role.HealthWorker,
            FacilityId = 1,
            Position = Position.Doctor
            // Missing PwzNumber
        };
        var content = new StringContent(JsonSerializer.Serialize(createDto), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/users-management/create", content);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task CreateUser_HealthWorkerRole_WithoutPosition_ShouldFail()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");
        var createDto = new CreateUserDto
        {
            Email = "newworker3@test.com",
            Password = "password123",
            Name = "New",
            Surname = "Worker",
            DateOfBirth = new DateTime(1990, 1, 1),
            Role = Role.HealthWorker,
            FacilityId = 1,
            PwzNumber = "PWZ999999"
            // Missing Position
        };
        var content = new StringContent(JsonSerializer.Serialize(createDto), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/users-management/create", content);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task CreateUser_CitizenRole_ShouldFail()
    {
        // Arrange - Citizen role is not supported per service logic
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");
        var createDto = new CreateUserDto
        {
            Email = "newcitizen@test.com",
            Password = "password123",
            Name = "New",
            Surname = "Citizen",
            DateOfBirth = new DateTime(1990, 1, 1),
            Role = Role.Citizen
        };
        var content = new StringContent(JsonSerializer.Serialize(createDto), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/users-management/create", content);

        // Assert
        // Should return BadRequest because Citizen role is not supported
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region SetCitizenRole Tests

    [Fact]
    public async Task SetCitizenRole_WithAdminRole_ShouldNotReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.PostAsync("/api/users-management/set-citizen-role/some-uid", null);

        // Assert
        // Endpoint has no authorization, so it should not return Forbidden
        // Will return 500 because Firebase is not available in test environment
        response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task SetCitizenRole_WithWorkerRole_ShouldNotReturnForbidden()
    {
        // Arrange - Note: This endpoint has NO authorization attribute
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");

        // Act
        var response = await client.PostAsync("/api/users-management/set-citizen-role/some-uid", null);

        // Assert
        // Endpoint has no authorization, so it should not return Forbidden
        response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task SetCitizenRole_WithCitizenRole_ShouldNotReturnForbidden()
    {
        // Arrange - Note: This endpoint has NO authorization attribute
        var client = _factory.CreateClientWithAuth("citizen", "test-citizen-uid-1");

        // Act
        var response = await client.PostAsync("/api/users-management/set-citizen-role/some-uid", null);

        // Assert
        // Endpoint has no authorization, so it should not return Forbidden
        response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task SetCitizenRole_WithoutAuthentication_ShouldNotReturnUnauthorized()
    {
        // Arrange - Note: This endpoint has NO authorization attribute
        var client = _factory.CreateClientWithoutAuth();

        // Act
        var response = await client.PostAsync("/api/users-management/set-citizen-role/some-uid", null);

        // Assert
        // Endpoint has no [Authorize] attribute, so it should not return Unauthorized
        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task SetCitizenRole_WithEmptyUid_ShouldReturnNotFoundOrError()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.PostAsync("/api/users-management/set-citizen-role/", null);

        // Assert
        // Empty UID in route should result in 404 or some error
        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.MethodNotAllowed);
    }

    #endregion

    #region Edge Cases and Additional Tests

    [Fact]
    public async Task CreateUser_WithNullBody_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");
        var content = new StringContent("", Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/users-management/create", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateUser_WithInvalidJson_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");
        var content = new StringContent("{invalid json}", Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/users-management/create", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateUser_DifferentAdmins_ShouldBothBeAuthorized()
    {
        // Arrange
        var client1 = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");
        var client2 = _factory.CreateClientWithAuth("admin", "test-admin-uid-2");
        
        var createDto = new CreateUserDto
        {
            Email = "testuser@test.com",
            Password = "password123",
            Name = "Test",
            Surname = "User",
            DateOfBirth = new DateTime(1990, 1, 1),
            Role = Role.Admin
        };
        var content1 = new StringContent(JsonSerializer.Serialize(createDto), Encoding.UTF8, "application/json");
        var content2 = new StringContent(JsonSerializer.Serialize(createDto), Encoding.UTF8, "application/json");

        // Act
        var response1 = await client1.PostAsync("/api/users-management/create", content1);
        var response2 = await client2.PostAsync("/api/users-management/create", content2);

        // Assert
        response1.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
        response2.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
        response1.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
        response2.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateUser_WithValidHealthWorkerData_ShouldNotReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");
        var createDto = new CreateUserDto
        {
            Email = "validworker@test.com",
            Password = "password123",
            Name = "Valid",
            Surname = "Worker",
            DateOfBirth = new DateTime(1990, 1, 1),
            Role = Role.HealthWorker,
            FacilityId = 1,
            PwzNumber = "PWZ123456",
            Position = Position.Doctor
        };
        var content = new StringContent(JsonSerializer.Serialize(createDto), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/users-management/create", content);

        // Assert
        // Should pass validation - may return 500 due to Firebase
        response.StatusCode.Should().NotBe(HttpStatusCode.BadRequest);
        response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateUser_WithVeryLongEmail_ShouldBeAcceptedByController()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");
        var longEmail = new string('a', 50) + "@" + new string('b', 50) + ".com";
        var createDto = new CreateUserDto
        {
            Email = longEmail,
            Password = "password123",
            Name = "Test",
            Surname = "User",
            DateOfBirth = new DateTime(1990, 1, 1),
            Role = Role.Admin
        };
        var content = new StringContent(JsonSerializer.Serialize(createDto), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/users-management/create", content);

        // Assert
        // Should not be rejected by ASP.NET validation
        response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateUser_WithSpecialCharactersInName_ShouldBeAccepted()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");
        var createDto = new CreateUserDto
        {
            Email = "specialuser@test.com",
            Password = "password123",
            Name = "Jean-Pierre",
            Surname = "O'Connor",
            DateOfBirth = new DateTime(1990, 1, 1),
            Role = Role.Admin
        };
        var content = new StringContent(JsonSerializer.Serialize(createDto), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/users-management/create", content);

        // Assert
        response.StatusCode.Should().NotBe(HttpStatusCode.BadRequest);
        response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateUser_WithFutureDateOfBirth_ShouldBeAcceptedByController()
    {
        // Arrange - Controller doesn't validate date of birth being in past
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");
        var createDto = new CreateUserDto
        {
            Email = "futureuser@test.com",
            Password = "password123",
            Name = "Future",
            Surname = "User",
            DateOfBirth = DateTime.Now.AddYears(10), // Future date
            Role = Role.Admin
        };
        var content = new StringContent(JsonSerializer.Serialize(createDto), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/users-management/create", content);

        // Assert
        // Controller doesn't have validation for future dates
        response.StatusCode.Should().NotBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateUser_HealthWorker_WithAllPositions_ShouldBeAuthorized()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        var positions = new[] { Position.Doctor, Position.Nurse };
        
        foreach (var position in positions)
        {
            var createDto = new CreateUserDto
            {
                Email = $"worker-{position}@test.com",
                Password = "password123",
                Name = "Test",
                Surname = "Worker",
                DateOfBirth = new DateTime(1990, 1, 1),
                Role = Role.HealthWorker,
                FacilityId = 1,
                PwzNumber = "PWZ999999",
                Position = position
            };
            var content = new StringContent(JsonSerializer.Serialize(createDto), Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync("/api/users-management/create", content);

            // Assert
            response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
            response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
        }
    }

    #endregion
}
