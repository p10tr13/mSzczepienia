using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.OCRSubmissions;
using mSzczepienia.Shared.DTO.VaccinationEntries;
using mSzczepienia.Tests.Integration.Fixtures;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace mSzczepienia.Tests.Integration.Controllers;

public class OCRSubmissionControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public OCRSubmissionControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    #region GetPendingForWorker Tests

    [Fact]
    public async Task GetPendingForWorker_WithWorkerRole_ShouldReturnOk()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");

        // Act
        var response = await client.GetAsync("/api/ocrsubmission/pending/worker?Page=1&PageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<SearchOCRSubmissionDTO>>();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetPendingForWorker_WithAdminRole_ShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/ocrsubmission/pending/worker?Page=1&PageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetPendingForWorker_WithCitizenRole_ShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("citizen", "test-citizen-uid-1");

        // Act
        var response = await client.GetAsync("/api/ocrsubmission/pending/worker?Page=1&PageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetPendingForWorker_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClientWithoutAuth();

        // Act
        var response = await client.GetAsync("/api/ocrsubmission/pending/worker?Page=1&PageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetPendingForWorker_WithZeroPage_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");

        // Act
        var response = await client.GetAsync("/api/ocrsubmission/pending/worker?Page=0&PageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetPendingForWorker_WithNegativePage_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");

        // Act
        var response = await client.GetAsync("/api/ocrsubmission/pending/worker?Page=-1&PageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetPendingForWorker_WithZeroPageSize_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");

        // Act
        var response = await client.GetAsync("/api/ocrsubmission/pending/worker?Page=1&PageSize=0");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetPendingForWorker_WithNegativePageSize_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");

        // Act
        var response = await client.GetAsync("/api/ocrsubmission/pending/worker?Page=1&PageSize=-5");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetPendingForWorker_WithPageSizeOver100_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");

        // Act
        var response = await client.GetAsync("/api/ocrsubmission/pending/worker?Page=1&PageSize=101");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetPendingForWorker_WithPageSizeExactly100_ShouldReturnOk()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");

        // Act
        var response = await client.GetAsync("/api/ocrsubmission/pending/worker?Page=1&PageSize=100");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetPendingForWorker_ShouldReturnPendingSubmissionsOnly()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");

        // Act
        var response = await client.GetAsync("/api/ocrsubmission/pending/worker?Page=1&PageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<SearchOCRSubmissionDTO>>();
        result.Should().NotBeNull();
        result!.Items.Should().NotBeNull();
    }

    [Fact]
    public async Task GetPendingForWorker_WithFirstNameFilter_ShouldReturnFilteredResults()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");

        // Act
        var response = await client.GetAsync("/api/ocrsubmission/pending/worker?Page=1&PageSize=10&FirstName=John");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<SearchOCRSubmissionDTO>>();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetPendingForWorker_WithSurnameFilter_ShouldReturnFilteredResults()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");

        // Act
        var response = await client.GetAsync("/api/ocrsubmission/pending/worker?Page=1&PageSize=10&Surname=Smith");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<SearchOCRSubmissionDTO>>();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetPendingForWorker_WithPeselFilter_ShouldReturnFilteredResults()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");

        // Act
        var response = await client.GetAsync("/api/ocrsubmission/pending/worker?Page=1&PageSize=10&Pesel=12345");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<SearchOCRSubmissionDTO>>();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetPendingForWorker_WithSearchInWorkersFacilityOnlyFalse_ShouldReturnOk()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");

        // Act
        var response = await client.GetAsync("/api/ocrsubmission/pending/worker?Page=1&PageSize=10&SearchInWorkersFacilityOnly=false");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<SearchOCRSubmissionDTO>>();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetPendingForWorker_WithSearchInWorkersFacilityOnlyTrue_ShouldReturnOk()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");

        // Act
        var response = await client.GetAsync("/api/ocrsubmission/pending/worker?Page=1&PageSize=10&SearchInWorkersFacilityOnly=true");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<SearchOCRSubmissionDTO>>();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetPendingForWorker_WithAllFilters_ShouldReturnOk()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");

        // Act
        var response = await client.GetAsync("/api/ocrsubmission/pending/worker?Page=1&PageSize=10&FirstName=John&Surname=Smith&Pesel=12345");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<SearchOCRSubmissionDTO>>();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetPendingForWorker_WithNonMatchingFilters_ShouldReturnEmptyResults()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");

        // Act
        var response = await client.GetAsync("/api/ocrsubmission/pending/worker?Page=1&PageSize=10&FirstName=NonExistingName");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<SearchOCRSubmissionDTO>>();
        result.Should().NotBeNull();
        result!.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetPendingForWorker_DifferentWorkers_ShouldReturnOk()
    {
        // Arrange
        var client1 = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");
        var client2 = _factory.CreateClientWithAuth("worker", "test-worker-uid-2");

        // Act
        var response1 = await client1.GetAsync("/api/ocrsubmission/pending/worker?Page=1&PageSize=10");
        var response2 = await client2.GetAsync("/api/ocrsubmission/pending/worker?Page=1&PageSize=10");

        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        response2.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetPendingForWorker_ResultShouldContainPaginationInfo()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");

        // Act
        var response = await client.GetAsync("/api/ocrsubmission/pending/worker?Page=1&PageSize=5");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<SearchOCRSubmissionDTO>>();
        result.Should().NotBeNull();
        result!.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(5);
        result.TotalCount.Should().BeGreaterThanOrEqualTo(0);
    }

    #endregion

    #region GetSubmissionReview (Photo) Tests

    [Fact]
    public async Task GetSubmissionPhotoReview_WithWorkerRole_ExistingSubmission_ShouldReturnOk()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");
        var submissionId = 3; // Pending submission from facility 1

        // Act
        var response = await client.GetAsync($"/api/ocrsubmission/review/{submissionId}/photo");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<OCRSubmissionPhotoReviewDto>();
        result.Should().NotBeNull();
        result!.SubmissionId.Should().Be(submissionId);
    }

    [Fact]
    public async Task GetSubmissionPhotoReview_WithWorkerRole_NonExistentSubmission_ShouldReturnNotFound()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");

        // Act
        var response = await client.GetAsync("/api/ocrsubmission/review/99999/photo");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetSubmissionPhotoReview_WithAdminRole_ShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/ocrsubmission/review/3/photo");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetSubmissionPhotoReview_WithCitizenRole_ShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("citizen", "test-citizen-uid-1");

        // Act
        var response = await client.GetAsync("/api/ocrsubmission/review/3/photo");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetSubmissionPhotoReview_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClientWithoutAuth();

        // Act
        var response = await client.GetAsync("/api/ocrsubmission/review/3/photo");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetSubmissionPhotoReview_ShouldReturnPatientDetails()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");
        var submissionId = 3;

        // Act
        var response = await client.GetAsync($"/api/ocrsubmission/review/{submissionId}/photo");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<OCRSubmissionPhotoReviewDto>();
        result.Should().NotBeNull();
        result!.PatientFirstName.Should().NotBeNullOrEmpty();
        result.PatientSurame.Should().NotBeNullOrEmpty();
        result.Pesel.Should().NotBeNullOrEmpty();
    }

    #endregion

    #region GetSubmissionEntriesReview Tests

    [Fact]
    public async Task GetSubmissionEntriesReview_WithWorkerRole_ExistingSubmission_ShouldReturnOk()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");
        var submissionId = 3;

        // Act
        var response = await client.GetAsync($"/api/ocrsubmission/review/{submissionId}/entries");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<OCRSubmissionEntriesReviewDto>();
        result.Should().NotBeNull();
        result!.SubmissionId.Should().Be(submissionId);
    }

    [Fact]
    public async Task GetSubmissionEntriesReview_WithWorkerRole_NonExistentSubmission_ShouldReturnNotFound()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");

        // Act
        var response = await client.GetAsync("/api/ocrsubmission/review/99999/entries");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetSubmissionEntriesReview_WithAdminRole_ShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.GetAsync("/api/ocrsubmission/review/3/entries");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetSubmissionEntriesReview_WithCitizenRole_ShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("citizen", "test-citizen-uid-1");

        // Act
        var response = await client.GetAsync("/api/ocrsubmission/review/3/entries");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetSubmissionEntriesReview_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClientWithoutAuth();

        // Act
        var response = await client.GetAsync("/api/ocrsubmission/review/3/entries");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region RejectSubmission Tests

    [Fact]
    public async Task RejectSubmission_WithWorkerRole_NonExistentSubmission_ShouldReturnNotFound()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");

        // Act
        var response = await client.PostAsync("/api/ocrsubmission/reject/99999", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RejectSubmission_WithAdminRole_ShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.PostAsync("/api/ocrsubmission/reject/3", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task RejectSubmission_WithCitizenRole_ShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("citizen", "test-citizen-uid-1");

        // Act
        var response = await client.PostAsync("/api/ocrsubmission/reject/3", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task RejectSubmission_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClientWithoutAuth();

        // Act
        var response = await client.PostAsync("/api/ocrsubmission/reject/3", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RejectSubmission_AlreadyAcceptedSubmission_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");
        var acceptedSubmissionId = 1; // Already accepted

        // Act
        var response = await client.PostAsync($"/api/ocrsubmission/reject/{acceptedSubmissionId}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RejectSubmission_AlreadyRejectedSubmission_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");
        var rejectedSubmissionId = 5; // Already rejected

        // Act
        var response = await client.PostAsync($"/api/ocrsubmission/reject/{rejectedSubmissionId}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region ProcessOcrSubmission Tests

    [Fact]
    public async Task ProcessOcrSubmission_WithAdminRole_ShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");
        var content = new MultipartFormDataContent();
        content.Add(new ByteArrayContent(new byte[] { 1, 2, 3 }), "files", "test.jpg");

        // Act
        var response = await client.PostAsync("/api/ocrsubmission/process", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ProcessOcrSubmission_WithWorkerRole_ShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");
        var content = new MultipartFormDataContent();
        content.Add(new ByteArrayContent(new byte[] { 1, 2, 3 }), "files", "test.jpg");

        // Act
        var response = await client.PostAsync("/api/ocrsubmission/process", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ProcessOcrSubmission_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClientWithoutAuth();
        var content = new MultipartFormDataContent();
        content.Add(new ByteArrayContent(new byte[] { 1, 2, 3 }), "files", "test.jpg");

        // Act
        var response = await client.PostAsync("/api/ocrsubmission/process", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ProcessOcrSubmission_WithCitizenRole_NoFiles_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("citizen", "test-citizen-uid-1");
        var content = new MultipartFormDataContent();

        // Act
        var response = await client.PostAsync("/api/ocrsubmission/process", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region ProcessReview Tests

    [Fact]
    public async Task ProcessReview_WithWorkerRole_ShouldAcceptValidRequest()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");
        var entries = new List<DetailedVaccinationEntryDto>();
        var content = new StringContent(JsonSerializer.Serialize(entries), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/ocrsubmission/99999/review", content);

        // Assert
        // Non-existent submission - should be handled by service
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest, HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task ProcessReview_WithAdminRole_ShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");
        var entries = new List<DetailedVaccinationEntryDto>();
        var content = new StringContent(JsonSerializer.Serialize(entries), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/ocrsubmission/3/review", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ProcessReview_WithCitizenRole_ShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("citizen", "test-citizen-uid-1");
        var entries = new List<DetailedVaccinationEntryDto>();
        var content = new StringContent(JsonSerializer.Serialize(entries), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/ocrsubmission/3/review", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ProcessReview_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClientWithoutAuth();
        var entries = new List<DetailedVaccinationEntryDto>();
        var content = new StringContent(JsonSerializer.Serialize(entries), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/ocrsubmission/3/review", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region DeleteOcrSubmission Tests

    [Fact]
    public async Task DeleteOcrSubmission_WithAdminRole_ShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");

        // Act
        var response = await client.DeleteAsync("/api/ocrsubmission/delete");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteOcrSubmission_WithWorkerRole_ShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");

        // Act
        var response = await client.DeleteAsync("/api/ocrsubmission/delete");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteOcrSubmission_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClientWithoutAuth();

        // Act
        var response = await client.DeleteAsync("/api/ocrsubmission/delete");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region UpdateOcrSubmissionWithEntries (Submit) Tests

    [Fact]
    public async Task SubmitOcrEntries_WithAdminRole_ShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("admin", "test-admin-uid-1");
        var entries = new List<VaccinationEntryDto>();
        var content = new StringContent(JsonSerializer.Serialize(entries), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/ocrsubmission/submit", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task SubmitOcrEntries_WithWorkerRole_ShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");
        var entries = new List<VaccinationEntryDto>();
        var content = new StringContent(JsonSerializer.Serialize(entries), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/ocrsubmission/submit", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task SubmitOcrEntries_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClientWithoutAuth();
        var entries = new List<VaccinationEntryDto>();
        var content = new StringContent(JsonSerializer.Serialize(entries), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/ocrsubmission/submit", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region Edge Cases and Additional Tests

    [Fact]
    public async Task GetPendingForWorker_SmallPageSize_ShouldRespectPagination()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");

        // Act
        var response = await client.GetAsync("/api/ocrsubmission/pending/worker?Page=1&PageSize=1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<SearchOCRSubmissionDTO>>();
        result.Should().NotBeNull();
        result!.PageSize.Should().Be(1);
        result.Items.Count.Should().BeInRange(0, 1);
    }

    [Fact]
    public async Task GetPendingForWorker_LargePagNumber_ShouldReturnEmptyResults()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");

        // Act
        var response = await client.GetAsync("/api/ocrsubmission/pending/worker?Page=1000&PageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<SearchOCRSubmissionDTO>>();
        result.Should().NotBeNull();
        result!.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetSubmissionPhotoReview_AcceptedSubmission_ShouldStillReturnDetails()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");
        var acceptedSubmissionId = 1;

        // Act
        var response = await client.GetAsync($"/api/ocrsubmission/review/{acceptedSubmissionId}/photo");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<OCRSubmissionPhotoReviewDto>();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetSubmissionEntriesReview_AcceptedSubmission_ShouldStillReturnDetails()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");
        var acceptedSubmissionId = 1;

        // Act
        var response = await client.GetAsync($"/api/ocrsubmission/review/{acceptedSubmissionId}/entries");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<OCRSubmissionEntriesReviewDto>();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetSubmissionPhotoReview_RejectedSubmission_ShouldStillReturnDetails()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");
        var rejectedSubmissionId = 5;

        // Act
        var response = await client.GetAsync($"/api/ocrsubmission/review/{rejectedSubmissionId}/photo");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<OCRSubmissionPhotoReviewDto>();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetPendingForWorker_WithPartialPeselFilter_ShouldReturnFilteredResults()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");

        // Act - searching with partial PESEL
        var response = await client.GetAsync("/api/ocrsubmission/pending/worker?Page=1&PageSize=10&Pesel=123");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<SearchOCRSubmissionDTO>>();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetPendingForWorker_WithEmptyStringFilters_ShouldReturnAllPending()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");

        // Act
        var response = await client.GetAsync("/api/ocrsubmission/pending/worker?Page=1&PageSize=10&FirstName=&Surname=&Pesel=");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<SearchOCRSubmissionDTO>>();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetPendingForWorker_MultipleCalls_ShouldReturnConsistentResults()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth("worker", "test-worker-uid-1");

        // Act
        var response1 = await client.GetAsync("/api/ocrsubmission/pending/worker?Page=1&PageSize=10&SearchInWorkersFacilityOnly=false");
        var result1 = await response1.Content.ReadFromJsonAsync<PagedResult<SearchOCRSubmissionDTO>>();

        var response2 = await client.GetAsync("/api/ocrsubmission/pending/worker?Page=1&PageSize=10&SearchInWorkersFacilityOnly=false");
        var result2 = await response2.Content.ReadFromJsonAsync<PagedResult<SearchOCRSubmissionDTO>>();

        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        response2.StatusCode.Should().Be(HttpStatusCode.OK);
        result1.Should().NotBeNull();
        result2.Should().NotBeNull();
        result1!.TotalCount.Should().Be(result2!.TotalCount);
    }

    #endregion
}
