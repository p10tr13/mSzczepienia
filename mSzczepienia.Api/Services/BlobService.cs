using Azure.Storage.Blobs;
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
    }
}