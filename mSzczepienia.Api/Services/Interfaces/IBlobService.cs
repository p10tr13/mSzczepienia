namespace mSzczepienia.Api.Services.Interfaces
{
    public interface IBlobService
    {
        Task UploadFileAsync(string blobName, Stream content);
        Task UploadMultipleFilesAsync(string directoryUrl, List<IFormFile> files);
        Task<List<string>> GetSasUrlsForDirectoryAsync(string directoryPath, int expirationMinutes = 30);
        Task DeleteDirectoryAsync(string directoryUrl);
    }
}
