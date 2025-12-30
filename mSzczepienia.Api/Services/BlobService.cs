using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Extensions.Options;
using mSzczepienia.Api.Configurations;
using mSzczepienia.Api.Services.Interfaces;

namespace mSzczepienia.Api.Services
{
    public class BlobService : IBlobService
    {
        private readonly BlobContainerClient _containerClient;

        public BlobService(IOptions<BlobStorageSettings> settings)
        {
            var config = settings.Value;
            var blobServiceClient = new BlobServiceClient(config.ConnectionString);

            _containerClient = blobServiceClient.GetBlobContainerClient(config.ContainerName);
        }

        public async Task UploadFileAsync(string blobName, Stream content)
        {
            await _containerClient.CreateIfNotExistsAsync();
            var blobClient = _containerClient.GetBlobClient(blobName);
            await blobClient.UploadAsync(content, overwrite: true);
        }

        public async Task UploadMultipleFilesAsync(string directoryUrl, List<IFormFile> files)
        {
            int i = 1;
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    string extension = Path.GetExtension(file.FileName);
                    if (string.IsNullOrEmpty(extension)) extension = ".jpg";

                    string blobName = $"{directoryUrl}{i}{extension}";

                    using var stream = file.OpenReadStream();
                    await UploadFileAsync(blobName, stream);

                    i++;
                }
            }
        }

        public async Task<List<string>> GetSasUrlsForDirectoryAsync(string directoryPath, int expirationMinutes = 30)
        {
            var sasUrls = new List<string>();
            await foreach (var blobItem in _containerClient.GetBlobsAsync(prefix: directoryPath))
            {
                var blobClient = _containerClient.GetBlobClient(blobItem.Name);

                if (!blobClient.CanGenerateSasUri)
                {
                    throw new InvalidOperationException("BlobClient cannot generate SAS URI. Ensure that the client is authorized with Shared Key credentials.");
                }

                var sasBuilder = new BlobSasBuilder
                {
                    BlobContainerName = _containerClient.Name,
                    BlobName = blobItem.Name,
                    Resource = "b",
                    ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(expirationMinutes)
                };
                sasBuilder.SetPermissions(BlobSasPermissions.Read);
                var sasUri = blobClient.GenerateSasUri(sasBuilder);
                sasUrls.Add(sasUri.ToString());
            }
            return sasUrls;
        }
    }
}