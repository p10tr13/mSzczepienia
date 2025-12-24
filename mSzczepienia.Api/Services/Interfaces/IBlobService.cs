namespace mSzczepienia.Api.Services.Interfaces
{
    public interface IBlobService
    {
        Task UploadFileAsync(string blobName, Stream content);
    }
}
