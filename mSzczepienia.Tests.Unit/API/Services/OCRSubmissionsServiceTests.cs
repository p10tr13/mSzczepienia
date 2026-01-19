using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Api.Services;
using mSzczepienia.Api.Services.Interfaces;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.OCRSubmissions;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.DTO.VaccinationEntries;
using mSzczepienia.Shared.Enums;
using mSzczepienia.Shared.Models;
using System.Security.Claims;

namespace mSzczepienia.Tests.Unit.API.Services;

public class OCRSubmissionsServiceTests
{
    private readonly IOCRSubmissionRepository _ocrSubmissionRepositoryMock;
    private readonly IPersonnelRepository _personnelRepositoryMock;
    private readonly ICitizenRepository _citizenRepositoryMock;
    private readonly IFacilityRepository _facilityRepositoryMock;
    private readonly IBlobService _blobServiceMock;
    private readonly ICitizenDeviceTokenRepository _citizenDeviceTokenRepositoryMock;
    private readonly IVaccineRepository _vaccineRepositoryMock;
    private readonly INotificationService _notificationServiceMock;
    private readonly IVaccinationEntryRepository _vaccinationEntryRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly IVaccinationRecordService _vaccinationRecordServiceMock;
    private readonly IAzureOcrService _azureOcrServiceMock;
    private readonly IUtilitiesService _utilitiesServiceMock;
    private readonly OCRSubmissionsService _sut;

    public OCRSubmissionsServiceTests()
    {
        _ocrSubmissionRepositoryMock = Substitute.For<IOCRSubmissionRepository>();
        _personnelRepositoryMock = Substitute.For<IPersonnelRepository>();
        _citizenRepositoryMock = Substitute.For<ICitizenRepository>();
        _facilityRepositoryMock = Substitute.For<IFacilityRepository>();
        _blobServiceMock = Substitute.For<IBlobService>();
        _citizenDeviceTokenRepositoryMock = Substitute.For<ICitizenDeviceTokenRepository>();
        _vaccineRepositoryMock = Substitute.For<IVaccineRepository>();
        _notificationServiceMock = Substitute.For<INotificationService>();
        _vaccinationEntryRepositoryMock = Substitute.For<IVaccinationEntryRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _vaccinationRecordServiceMock = Substitute.For<IVaccinationRecordService>();
        _azureOcrServiceMock = Substitute.For<IAzureOcrService>();
        _utilitiesServiceMock = Substitute.For<IUtilitiesService>();

        _sut = new OCRSubmissionsService(
            _ocrSubmissionRepositoryMock,
            _personnelRepositoryMock,
            _citizenRepositoryMock,
            _facilityRepositoryMock,
            _blobServiceMock,
            _citizenDeviceTokenRepositoryMock,
            _vaccineRepositoryMock,
            _notificationServiceMock,
            _vaccinationEntryRepositoryMock,
            _unitOfWorkMock,
            _vaccinationRecordServiceMock,
            _azureOcrServiceMock,
            _utilitiesServiceMock
        );
    }

    #region GetPendingForWorkerAsync Tests

    [Fact]
    public async Task GetPendingForWorkerAsync_ShouldReturnGlobalPendingList_WhenSearchInWorkersFacilityOnlyIsFalse()
    {
        // Arrange
        var firebaseUid = "test-uid-123";
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] {
            new Claim(ClaimTypes.NameIdentifier, firebaseUid)
        }));
        var query = new SearchOCRSubmissionQueryDto { Page = 1, PageSize = 10, SearchInWorkersFacilityOnly = false };
        var expectedResult = new PagedResult<SearchOCRSubmissionDTO>
        {
            Items = new List<SearchOCRSubmissionDTO>
            {
                new SearchOCRSubmissionDTO { ID = 1, Name = "John", Surname = "Doe", SubmissionDate = DateTime.UtcNow, Pesel = "12345678901" }
            },
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };

        _ocrSubmissionRepositoryMock.GetPendingAsync(query, Arg.Any<CancellationToken>())
            .Returns(expectedResult);

        // Act
        var result = await _sut.GetPendingForWorkerAsync(user, query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        result.Items.First().Name.Should().Be("John");
    }

    [Fact]
    public async Task GetPendingForWorkerAsync_ShouldReturnFacilityPendingList_WhenSearchInWorkersFacilityOnlyIsTrue()
    {
        // Arrange
        var firebaseUid = "test-uid-123";
        var facilityId = 5;
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] {
            new Claim(ClaimTypes.NameIdentifier, firebaseUid)
        }));
        var query = new SearchOCRSubmissionQueryDto { Page = 1, PageSize = 10, SearchInWorkersFacilityOnly = true };
        var expectedResult = new PagedResult<SearchOCRSubmissionDTO>
        {
            Items = new List<SearchOCRSubmissionDTO>(),
            TotalCount = 0,
            PageNumber = 1,
            PageSize = 10
        };

        _personnelRepositoryMock.GetFacilityIdForWorkerByFirebaseUidAsync(firebaseUid, Arg.Any<CancellationToken>())
            .Returns(facilityId);
        _ocrSubmissionRepositoryMock.GetPendingForFacilityAsync(facilityId, Arg.Any<SearchOCRSubmissionQueryDto>(), Arg.Any<CancellationToken>())
            .Returns(expectedResult);

        // Act
        var result = await _sut.GetPendingForWorkerAsync(user, query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetPendingForWorkerAsync_ShouldReturnEmptyResult_WhenNoFirebaseUidFound()
    {
        // Arrange
        var user = new ClaimsPrincipal(new ClaimsIdentity());
        var query = new SearchOCRSubmissionQueryDto();

        // Act
        var result = await _sut.GetPendingForWorkerAsync(user, query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
    }

    #endregion

    #region GetSubmissionPhotoReviewAsync Tests

    [Fact]
    public async Task GetSubmissionPhotoReviewAsync_ShouldReturnPhotoReviewDto_WhenSubmissionExists()
    {
        // Arrange
        var submissionId = 1;
        var citizenId = 100;
        var submission = new OcrSubmission
        {
            submissionId = submissionId,
            citizenId = citizenId,
            submissionDate = DateTime.UtcNow,
            photoUrl = "test/photos/",
            status = OcrSubmissionStatus.Pending
        };

        var photoUrls = new List<string> { "url1", "url2" };

        _ocrSubmissionRepositoryMock.GetByIdAsync(submissionId, Arg.Any<CancellationToken>())
            .Returns(submission);
        _citizenRepositoryMock.GetFacilityIdByCitizenIdAsync(citizenId, Arg.Any<CancellationToken>())
            .Returns((int?)null);
        _blobServiceMock.GetSasUrlsForDirectoryAsync("test/photos/")
            .Returns(photoUrls);

        // Act
        var result = await _sut.GetSubmissionPhotoReviewAsync(submissionId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.SubmissionId.Should().Be(submissionId);
        result.Photos.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetSubmissionPhotoReviewAsync_ShouldReturnNull_WhenSubmissionNotFound()
    {
        // Arrange
        var submissionId = 999;
        _ocrSubmissionRepositoryMock.GetByIdAsync(submissionId, Arg.Any<CancellationToken>())
            .Returns((OcrSubmission?)null);

        // Act
        var result = await _sut.GetSubmissionPhotoReviewAsync(submissionId, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region RejectSubmissionAsync Tests

    [Fact]
    public async Task RejectSubmissionAsync_ShouldRejectSubmission_WhenSubmissionIsPending()
    {
        // Arrange
        var firebaseUid = "worker-uid";
        var submissionId = 1;
        var citizenId = 100;
        var personnelId = 50;

        var submission = new OcrSubmission
        {
            submissionId = submissionId,
            citizenId = citizenId,
            status = OcrSubmissionStatus.Pending,
            photoUrl = "test/photos/",
            submissionDate = DateTime.UtcNow
        };

        var entries = new List<VaccinationEntry>();

        _ocrSubmissionRepositoryMock.GetByIdAsync(submissionId, Arg.Any<CancellationToken>())
            .Returns(submission);
        _citizenRepositoryMock.GetByIdAsync(citizenId, Arg.Any<CancellationToken>())
            .Returns(Substitute.For<Citizen>());
        _personnelRepositoryMock.GetByFirebaseUidAsync(firebaseUid, Arg.Any<CancellationToken>())
            .Returns(Substitute.For<Personnel>());
        _vaccinationEntryRepositoryMock.GetBySubmissionIdAsync(submissionId, Arg.Any<CancellationToken>())
            .Returns(entries);
        _citizenDeviceTokenRepositoryMock.GetTokensByCitizenIdAsync(citizenId, Arg.Any<CancellationToken>())
            .Returns(new List<(string, string)>());

        // Act
        var result = await _sut.RejectSubmissionAsync(firebaseUid, submissionId, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        submission.status.Should().Be(OcrSubmissionStatus.Rejected);
        _ocrSubmissionRepositoryMock.Received(1).Update(submission);
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RejectSubmissionAsync_ShouldThrowInvalidOperationException_WhenSubmissionNotPending()
    {
        // Arrange
        var firebaseUid = "worker-uid";
        var submissionId = 1;
        var citizenId = 100;

        var submission = new OcrSubmission
        {
            submissionId = submissionId,
            citizenId = citizenId,
            status = OcrSubmissionStatus.Accepted,
            photoUrl = "test/photos/"
        };

        _ocrSubmissionRepositoryMock.GetByIdAsync(submissionId, Arg.Any<CancellationToken>())
            .Returns(submission);
        _citizenRepositoryMock.GetByIdAsync(citizenId, Arg.Any<CancellationToken>())
            .Returns(Substitute.For<Citizen>());
        _personnelRepositoryMock.GetByFirebaseUidAsync(firebaseUid, Arg.Any<CancellationToken>())
            .Returns(Substitute.For<Personnel>());

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.RejectSubmissionAsync(firebaseUid, submissionId, CancellationToken.None));
    }

    [Fact]
    public async Task RejectSubmissionAsync_ShouldReturnFalse_WhenSubmissionNotFound()
    {
        // Arrange
        var firebaseUid = "worker-uid";
        var submissionId = 999;

        _ocrSubmissionRepositoryMock.GetByIdAsync(submissionId, Arg.Any<CancellationToken>())
            .Returns((OcrSubmission?)null);

        // Act
        var result = await _sut.RejectSubmissionAsync(firebaseUid, submissionId, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region GetSubmissionEntriesReviewAsync Tests

    [Fact]
    public async Task GetSubmissionEntriesReviewAsync_ShouldReturnNull_WhenSubmissionNotFound()
    {
        // Arrange
        var submissionId = 999;
        _ocrSubmissionRepositoryMock.GetByIdAsync(submissionId, Arg.Any<CancellationToken>())
            .Returns((OcrSubmission?)null);

        // Act
        var result = await _sut.GetSubmissionEntriesReviewAsync(submissionId, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region DeleteOcrSubmissionAsync Tests

    [Fact]
    public async Task DeleteOcrSubmissionAsync_ShouldDeleteSubmissionAndBlob_WhenCalled()
    {
        // Arrange
        var firebaseUid = "citizen-uid";
        var photoUrl = "test/photos/";

        _ocrSubmissionRepositoryMock.DeleteOcrSubmissionAsync(firebaseUid, Arg.Any<CancellationToken>())
            .Returns(photoUrl);

        // Act
        await _sut.DeleteOcrSubmissionAsync(firebaseUid, CancellationToken.None);

        // Assert
        await _ocrSubmissionRepositoryMock.Received(1).DeleteOcrSubmissionAsync(firebaseUid, Arg.Any<CancellationToken>());
        await _blobServiceMock.Received(1).DeleteDirectoryAsync(photoUrl);
    }

    #endregion

    #region UpdateOcrSubmissionWithEntries Tests

    [Fact]
    public async Task UpdateOcrSubmissionWithEntries_ShouldUpdateSubmissionWithEntries_WhenCalled()
    {
        // Arrange
        var firebaseUid = "citizen-uid";
        var entries = new List<VaccinationEntryDto>
        {
            new VaccinationEntryDto
            {
                Id = 0,
                VaccineName = "COVID-19",
                VaccineNameOcr = "COVID",
                DateOcr = "2024-01-15",
                BatchOcr = "ABC123",
                Batch = "ABC123",
                Date = new DateTime(2024, 1, 15),
                Status = VaccinationEntryStatus.WaitingForReview.ToString()
            }
        };

        // Act
        await _sut.UpdateOcrSubmissionWithEntries(firebaseUid, entries, CancellationToken.None);

        // Assert
        await _ocrSubmissionRepositoryMock.Received(1).UpdateOcrSubmissionWithEntries(firebaseUid, entries, Arg.Any<CancellationToken>());
    }

    #endregion
}
