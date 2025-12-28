namespace mSzczepienia.Api.Services.Interfaces
{
    public interface IBlobService
    {
        Task UploadFileAsync(string blobName, Stream content);
        Task UploadMultipleFilesAsync(string directoryUrl, List<IFormFile> files);
    }
}
