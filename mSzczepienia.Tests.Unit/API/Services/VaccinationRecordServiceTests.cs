using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Api.Services;
using mSzczepienia.Api.Services.Interfaces;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.DTO.VaccinationRecords;
using mSzczepienia.Shared.Models;
using Microsoft.Extensions.Logging;

namespace mSzczepienia.Tests.Unit.API.Services;

public class VaccinationRecordServiceTests
{
    private readonly IVaccinationRecordRepository _vaccinationRecordRepositoryMock;
    private readonly ICitizenRepository _citizenRepositoryMock;
    private readonly IVaccineRepository _vaccineRepositoryMock;
    private readonly ICitizenDeviceTokenRepository _citizenDeviceTokenRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly INotificationService _notificationServiceMock;
    private readonly ILogger<VaccinationRecordService> _loggerMock;
    private readonly VaccinationRecordService _sut;

    public VaccinationRecordServiceTests()
    {
        _vaccinationRecordRepositoryMock = Substitute.For<IVaccinationRecordRepository>();
        _citizenRepositoryMock = Substitute.For<ICitizenRepository>();
        _vaccineRepositoryMock = Substitute.For<IVaccineRepository>();
        _citizenDeviceTokenRepositoryMock = Substitute.For<ICitizenDeviceTokenRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _notificationServiceMock = Substitute.For<INotificationService>();
        _loggerMock = Substitute.For<ILogger<VaccinationRecordService>>();

        _sut = new VaccinationRecordService(
            _vaccinationRecordRepositoryMock,
            _citizenRepositoryMock,
            _vaccineRepositoryMock,
            _loggerMock,
            _unitOfWorkMock,
            _citizenDeviceTokenRepositoryMock,
            _notificationServiceMock
        );
    }

    #region GetVaccinationRecordsOverviewAsync Tests

    [Fact]
    public async Task GetVaccinationRecordsOverviewAsync_ShouldReturnPagedResult_WhenCalled()
    {
        // Arrange
        var firebaseUid = "citizen-uid";
        var query = Substitute.For<GetVaccinationRecordOverviewQuery>();
        var expectedResult = new PagedResult<VaccinationRecordOverviewDto>
        {
            Items = new List<VaccinationRecordOverviewDto>(),
            TotalCount = 0,
            PageNumber = 1,
            PageSize = 10
        };

        _vaccinationRecordRepositoryMock.GetVaccinationRecordOverviewAsync(firebaseUid, query, Arg.Any<CancellationToken>())
            .Returns(expectedResult);

        // Act
        var result = await _sut.GetVaccinationRecordsOverviewAsync(firebaseUid, query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
    }

    #endregion

    #region GetVaccinationRecordsDetailsAsync Tests

    [Fact]
    public async Task GetVaccinationRecordsDetailsAsync_ShouldReturnList_WhenCalled()
    {
        // Arrange
        var firebaseUid = "citizen-uid";
        var query = Substitute.For<GetVaccinationRecordDetailsQuery>();
        var expectedResult = new List<VaccinationRecordDetailsDto>();

        _vaccinationRecordRepositoryMock.GetVaccinationRecordsDetailsAsync(firebaseUid, query, Arg.Any<CancellationToken>())
            .Returns(expectedResult);

        // Act
        var result = await _sut.GetVaccinationRecordsDetailsAsync(firebaseUid, query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    #endregion

    #region GetVaccinationRecordsByCitizenIdAsync Tests

    [Fact]
    public async Task GetVaccinationRecordsByCitizenIdAsync_ShouldReturnPagedResult_WhenRecordsExist()
    {
        // Arrange
        var firebaseUid = "requester-uid";
        var citizenId = 100;
        var query = Substitute.For<GetVaccinationRecordsByCitizenIdQuery>();
        query.CitizenId = citizenId;
        
        var expectedResult = new PagedResult<VaccinationRecordSimpleListItemDto>
        {
            Items = new List<VaccinationRecordSimpleListItemDto>(),
            TotalCount = 0,
            PageNumber = 1,
            PageSize = 10
        };

        _vaccinationRecordRepositoryMock.GetVaccinationRecordsByCitizenIdAsync(query, Arg.Any<CancellationToken>())
            .Returns(expectedResult);

        // Act
        var result = await _sut.GetVaccinationRecordsByCitizenIdAsync(firebaseUid, query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
    }

    #endregion

    #region GetVaccinationRecordByIdAsync Tests

    [Fact]
    public async Task GetVaccinationRecordByIdAsync_ShouldReturnEditDto_WhenRecordExists()
    {
        // Arrange
        var firebaseUid = "requester-uid";
        var recordId = 1;
        var citizenId = 100;
        var vaccineId = 10;

        var record = new VaccinationRecord
        {
            recordId = recordId,
            citizenId = citizenId,
            vaccineId = vaccineId,
            vaccinationDate = new DateTime(2024, 1, 15),
            doseNumber = "1",
            batch = "ABC123",
            remarks = "No issues"
        };

        var vaccine = Substitute.For<VaccineDefinition>();
        vaccine.vaccineId = vaccineId;
        vaccine.vaccineName = "COVID-19";
        vaccine.manufacturer = "Pfizer";

        var citizen = new Citizen
        {
            citizenId = citizenId,
            pesel = "12345678901",
            account = new Account
            {
                name = "John",
                surname = "Doe",
                dateOfBirth = new DateTime(1990, 1, 1),
                firebaseUid = "uid",
                email = "john@test.pl",
                role = Shared.Enums.Role.Citizen,
                creationDate = DateTime.UtcNow
            }
        };

        _vaccinationRecordRepositoryMock.GetById(recordId, Arg.Any<CancellationToken>())
            .Returns(record);
        _vaccineRepositoryMock.GetVaccineDefinitionByIdNoTrackingAsync(vaccineId, Arg.Any<CancellationToken>())
            .Returns(vaccine);
        _citizenRepositoryMock.GetByIdNoTrackingAsync(citizenId, Arg.Any<CancellationToken>())
            .Returns(citizen);

        // Act
        var result = await _sut.GetVaccinationRecordByIdAsync(firebaseUid, recordId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.recordId.Should().Be(recordId);
        result.name.Should().Be("John");
        result.vaccineName.Should().Be("COVID-19");
    }

    [Fact]
    public async Task GetVaccinationRecordByIdAsync_ShouldReturnNull_WhenRecordNotFound()
    {
        // Arrange
        var firebaseUid = "requester-uid";
        var recordId = 999;

        _vaccinationRecordRepositoryMock.GetById(recordId, Arg.Any<CancellationToken>())
            .Returns((VaccinationRecord?)null);

        // Act
        var result = await _sut.GetVaccinationRecordByIdAsync(firebaseUid, recordId, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetVaccinationRecordByIdAsync_ShouldReturnNull_WhenVaccineNotFound()
    {
        // Arrange
        var firebaseUid = "requester-uid";
        var recordId = 1;
        var citizenId = 100;
        var vaccineId = 10;

        var record = new VaccinationRecord
        {
            recordId = recordId,
            citizenId = citizenId,
            vaccineId = vaccineId,
            vaccinationDate = new DateTime(2024, 1, 15),
            doseNumber = "1",
            batch = "ABC123"
        };

        _vaccinationRecordRepositoryMock.GetById(recordId, Arg.Any<CancellationToken>())
            .Returns(record);
        _vaccineRepositoryMock.GetVaccineDefinitionByIdNoTrackingAsync(vaccineId, Arg.Any<CancellationToken>())
            .Returns((VaccineDefinition?)null);

        // Act
        var result = await _sut.GetVaccinationRecordByIdAsync(firebaseUid, recordId, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region UpdateVaccinationRecordAsync Tests

    [Fact]
    public async Task UpdateVaccinationRecordAsync_ShouldUpdateRecord_WhenRecordExists()
    {
        // Arrange
        var firebaseUid = "requester-uid";
        var recordId = 1;
        var updateDto = new UpdateVaccinationRecordDto
        {
            vaccinationDate = new DateTime(2024, 1, 20),
            doseNumber = "2",
            batch = "XYZ789",
            remarks = "Updated remarks"
        };

        var record = new VaccinationRecord
        {
            recordId = recordId,
            citizenId = 100,
            vaccineId = 10,
            vaccinationDate = new DateTime(2024, 1, 15),
            doseNumber = "1",
            batch = "ABC123"
        };

        _vaccinationRecordRepositoryMock.GetById(recordId, Arg.Any<CancellationToken>())
            .Returns(record);

        // Act
        await _sut.UpdateVaccinationRecordAsync(firebaseUid, recordId, updateDto, CancellationToken.None);

        // Assert
        record.vaccinationDate.Should().Be(new DateTime(2024, 1, 20));
        record.doseNumber.Should().Be("2");
        record.batch.Should().Be("XYZ789");
        record.remarks.Should().Be("Updated remarks");
        _vaccinationRecordRepositoryMock.Received(1).Update(record);
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateVaccinationRecordAsync_ShouldThrowInvalidOperationException_WhenRecordNotFound()
    {
        // Arrange
        var firebaseUid = "requester-uid";
        var recordId = 999;
        var updateDto = new UpdateVaccinationRecordDto
        {
            vaccinationDate = new DateTime(2024, 1, 20),
            doseNumber = "2",
            batch = "XYZ789"
        };

        _vaccinationRecordRepositoryMock.GetById(recordId, Arg.Any<CancellationToken>())
            .Returns((VaccinationRecord?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.UpdateVaccinationRecordAsync(firebaseUid, recordId, updateDto, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateVaccinationRecordAsync_ShouldThrowArgumentNullException_WhenVaccinationDateIsNull()
    {
        // Arrange
        var firebaseUid = "requester-uid";
        var recordId = 1;
        var updateDto = new UpdateVaccinationRecordDto
        {
            vaccinationDate = null,
            doseNumber = "2",
            batch = "XYZ789"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => _sut.UpdateVaccinationRecordAsync(firebaseUid, recordId, updateDto, CancellationToken.None));
    }

    #endregion

    #region DeleteVaccinationRecordAsync Tests

    [Fact]
    public async Task DeleteVaccinationRecordAsync_ShouldDeleteRecord_WhenRecordExists()
    {
        // Arrange
        var firebaseUid = "requester-uid";
        var recordId = 1;
        var record = new VaccinationRecord
        {
            recordId = recordId,
            citizenId = 100,
            vaccineId = 10,
            vaccinationDate = new DateTime(2024, 1, 15),
            doseNumber = "1",
            batch = "ABC123"
        };

        _vaccinationRecordRepositoryMock.GetById(recordId, Arg.Any<CancellationToken>())
            .Returns(record);

        // Act
        await _sut.DeleteVaccinationRecordAsync(firebaseUid, recordId, CancellationToken.None);

        // Assert
        _vaccinationRecordRepositoryMock.Received(1).Delete(record);
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteVaccinationRecordAsync_ShouldThrowInvalidOperationException_WhenRecordNotFound()
    {
        // Arrange
        var firebaseUid = "requester-uid";
        var recordId = 999;

        _vaccinationRecordRepositoryMock.GetById(recordId, Arg.Any<CancellationToken>())
            .Returns((VaccinationRecord?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.DeleteVaccinationRecordAsync(firebaseUid, recordId, CancellationToken.None));
    }

    #endregion

    #region AddVaccinationRecordAsync Tests

    [Fact]
    public async Task AddVaccinationRecordAsync_ShouldAddRecord_WhenCitizenAndVaccineExist()
    {
        // Arrange
        var firebaseUid = "requester-uid";
        var citizenId = 100;
        var vaccineId = 10;
        var createDto = new CreateVaccinationRecordDto
        {
            citizenId = citizenId,
            vaccineId = vaccineId,
            vaccinationDate = new DateTime(2024, 1, 15),
            doseNumber = "1",
            batch = "ABC123"
        };

        var citizen = new Citizen
        {
            citizenId = citizenId,
            pesel = "12345678901",
            account = new Account
            {
                name = "John",
                surname = "Doe",
                dateOfBirth = new DateTime(1990, 1, 1),
                firebaseUid = "uid",
                email = "john@test.pl",
                role = Shared.Enums.Role.Citizen,
                creationDate = DateTime.UtcNow
            }
        };

        var vaccine = Substitute.For<VaccineDefinition>();
        vaccine.vaccineId = vaccineId;
        vaccine.vaccineName = "COVID-19";
        vaccine.manufacturer = "Pfizer";

        _citizenRepositoryMock.GetByIdNoTrackingAsync(citizenId, Arg.Any<CancellationToken>())
            .Returns(citizen);
        _vaccineRepositoryMock.GetVaccineDefinitionByIdNoTrackingAsync(vaccineId, Arg.Any<CancellationToken>())
            .Returns(vaccine);
        _citizenDeviceTokenRepositoryMock.GetTokensByCitizenIdAsync(citizenId, Arg.Any<CancellationToken>())
            .Returns(new List<(string, string)>());

        // Act
        await _sut.AddVaccinationRecordAsync(firebaseUid, createDto, CancellationToken.None);

        // Assert
        await _vaccinationRecordRepositoryMock.Received(1).AddAsync(Arg.Is<VaccinationRecord>(r =>
            r.citizenId == citizenId &&
            r.vaccineId == vaccineId &&
            r.doseNumber == "1" &&
            r.batch == "ABC123"), Arg.Any<CancellationToken>());
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AddVaccinationRecordAsync_ShouldThrowInvalidOperationException_WhenCitizenNotFound()
    {
        // Arrange
        var firebaseUid = "requester-uid";
        var createDto = new CreateVaccinationRecordDto
        {
            citizenId = 999,
            vaccineId = 10,
            vaccinationDate = new DateTime(2024, 1, 15),
            doseNumber = "1",
            batch = "ABC123"
        };

        _citizenRepositoryMock.GetByIdNoTrackingAsync(999, Arg.Any<CancellationToken>())
            .Returns((Citizen?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.AddVaccinationRecordAsync(firebaseUid, createDto, CancellationToken.None));
    }

    [Fact]
    public async Task AddVaccinationRecordAsync_ShouldThrowInvalidOperationException_WhenVaccineNotFound()
    {
        // Arrange
        var firebaseUid = "requester-uid";
        var citizenId = 100;
        var createDto = new CreateVaccinationRecordDto
        {
            citizenId = citizenId,
            vaccineId = 999,
            vaccinationDate = new DateTime(2024, 1, 15),
            doseNumber = "1",
            batch = "ABC123"
        };

        var citizen = new Citizen
        {
            citizenId = citizenId,
            pesel = "12345678901",
            account = new Account
            {
                name = "John",
                surname = "Doe",
                dateOfBirth = new DateTime(1990, 1, 1),
                firebaseUid = "uid",
                email = "john@test.pl",
                role = Shared.Enums.Role.Citizen,
                creationDate = DateTime.UtcNow
            }
        };

        _citizenRepositoryMock.GetByIdNoTrackingAsync(citizenId, Arg.Any<CancellationToken>())
            .Returns(citizen);
        _vaccineRepositoryMock.GetVaccineDefinitionByIdNoTrackingAsync(999, Arg.Any<CancellationToken>())
            .Returns((VaccineDefinition?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.AddVaccinationRecordAsync(firebaseUid, createDto, CancellationToken.None));
    }

    #endregion

    #region CreateRecordsFromEntriesAsync Tests

    [Fact]
    public async Task CreateRecordsFromEntriesAsync_ShouldCreateRecordsFromEntries_WhenEntriesProvided()
    {
        // Arrange
        var citizenId = 100;
        var entry = Substitute.For<VaccinationEntry>();
        entry.vaccinationEntryId = 1;
        entry.vaccineId = 10;
        entry.date = new DateTime(2024, 1, 15);
        entry.batch = "ABC123";
        
        var entries = new List<VaccinationEntry> { entry };

        // Act
        await _sut.CreateRecordsFromEntriesAsync(citizenId, entries, CancellationToken.None);

        // Assert
        await _vaccinationRecordRepositoryMock.Received(1).AddAsync(Arg.Is<VaccinationRecord>(r =>
            r.citizenId == citizenId &&
            r.vaccineId == 10 &&
            r.doseNumber == "unknown" &&
            r.batch == "ABC123"), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateRecordsFromEntriesAsync_ShouldThrowException_WhenVaccineIdNull()
    {
        // Arrange
        var citizenId = 100;
        var entry = Substitute.For<VaccinationEntry>();
        entry.vaccinationEntryId = 1;
        entry.vaccineId = null;
        entry.date = new DateTime(2024, 1, 15);
        entry.batch = "ABC123";
        
        var entries = new List<VaccinationEntry> { entry };

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(
            () => _sut.CreateRecordsFromEntriesAsync(citizenId, entries, CancellationToken.None));
    }

    [Fact]
    public async Task CreateRecordsFromEntriesAsync_ShouldThrowException_WhenDateNull()
    {
        // Arrange
        var citizenId = 100;
        var entry = Substitute.For<VaccinationEntry>();
        entry.vaccinationEntryId = 1;
        entry.vaccineId = 10;
        entry.date = null;
        entry.batch = "ABC123";
        
        var entries = new List<VaccinationEntry> { entry };

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(
            () => _sut.CreateRecordsFromEntriesAsync(citizenId, entries, CancellationToken.None));
    }

    #endregion

    #region RemindAboutNextVaccinationsAsync Tests

    [Fact]
    public async Task RemindAboutNextVaccinationsAsync_ShouldRetrieveTokens_WhenCalled()
    {
        // Arrange
        _citizenDeviceTokenRepositoryMock.GetDeviceTokensForNextVaccinationsAsync(Arg.Any<CancellationToken>())
            .Returns(new List<UpcomingVaccinationDto>());

        // Act
        await _sut.RemindAboutNextVaccinationsAsync(CancellationToken.None);

        // Assert
        await _citizenDeviceTokenRepositoryMock.Received(1).GetDeviceTokensForNextVaccinationsAsync(Arg.Any<CancellationToken>());
    }

    #endregion
}
