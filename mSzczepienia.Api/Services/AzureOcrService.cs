using Azure;
using Microsoft.Extensions.Options;
using mSzczepienia.Api.Configurations;
using mSzczepienia.Api.Services.Interfaces;
using mSzczepienia.Shared.DTO.VaccinationEntries;
using Azure.AI.FormRecognizer.DocumentAnalysis;

namespace mSzczepienia.Api.Services
{
    public class AzureOcrService: IAzureOcrService
    {
        private readonly AzureOcrSettings _settings;
        private readonly DocumentAnalysisClient _client;
        private readonly string _modelId;

        public AzureOcrService(IOptions<AzureOcrSettings> settings)
        {
            _settings = settings.Value;
            _client = new DocumentAnalysisClient(
                new Uri(_settings.Endpoint),
                new AzureKeyCredential(_settings.ApiKey)
            );
            _modelId = _settings.ModelId;
        }

        public async Task<List<VaccinationEntryOcrDto>> ProcessFilesAsync(List<IFormFile> files, CancellationToken ct = default)
        {
            var results = new List<VaccinationEntryOcrDto>();

            foreach (var file in files)
            {
                if (file == null || file.Length == 0) continue;

                await using var stream = file.OpenReadStream();

                try
                {
                    Operation<AnalyzeResult> operation = await _client.AnalyzeDocumentAsync(
                        WaitUntil.Completed,
                        _modelId,
                        stream,
                        cancellationToken: ct
                    );

                    AnalyzeResult result = operation.Value;

                    foreach (var document in result.Documents)
                    {
                        if (document.Fields.TryGetValue("vaccinationTable", out var tableField))
                        {
                            if (tableField.FieldType == DocumentFieldType.List)
                            {
                                foreach (var rowItem in tableField.Value.AsList())
                                {
                                    if (rowItem.FieldType == DocumentFieldType.Dictionary)
                                    {
                                        var columns = rowItem.Value.AsDictionary();

                                        var entry = new VaccinationEntryOcrDto();

                                        // Vaccine Name
                                        if (columns.TryGetValue("vaccineName", out var vaccineNameField))
                                        {
                                            entry.vaccineName = vaccineNameField.Content;
                                        }

                                        // Batch
                                        if (columns.TryGetValue("batch", out var batchField))
                                        {
                                            entry.batch = batchField.Content;
                                        }

                                        // Vaccination Date
                                        if (columns.TryGetValue("vaccinationDate", out var dateField))
                                        {
                                            entry.vaccinationDate = dateField.Content;
                                        }

                                        // Add only if vaccineName is not null
                                        if (!string.IsNullOrEmpty(entry.vaccineName))
                                        {
                                            results.Add(entry);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                   throw new Exception($"OCR error for file: {file.FileName}: {ex.Message}");
                }
            }

            return results;
        }
    }
}