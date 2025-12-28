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

    }
}