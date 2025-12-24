using Serilog;

namespace mSzczepienia.Api.Configurations
{
    public class BlobStorageSettings
    {
        public const string SectionName = "BlobStorage";
        public string ConnectionString { get; set; }
        public string ContainerName { get; set; }
    }
}