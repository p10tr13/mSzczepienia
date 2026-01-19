using Azure;
using Azure.AI.OpenAI;
using mSzczepienia.Api.Services.Interfaces;
using mSzczepienia.Shared.DTO.VaccinationEntries;
using OpenAI.Chat;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace mSzczepienia.Api.Services;

public class AzureOpenAIService : IAzureOcrService
{
    private readonly ChatClient _chatClient;

    private class GptResponseWrapper
    {
        public List<VaccinationEntryOcrDto> Entries { get; set; } = new();
    }

    public AzureOpenAIService(IOptions<AzureOpenAiSettings> settings)
    {
        var config = settings.Value;

        var azureClient = new AzureOpenAIClient(
            new Uri(config.Endpoint),
            new AzureKeyCredential(config.ApiKey));

        _chatClient = azureClient.GetChatClient(config.DeploymentName);
    }

    public async Task<List<VaccinationEntryOcrDto>> ProcessFilesAsync(List<IFormFile> files, CancellationToken ct)
    {
        var allResults = new List<VaccinationEntryOcrDto>();

        foreach (var file in files)
        {
            if (file == null || file.Length == 0) continue;

            try
            {
                using var stream = file.OpenReadStream();
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream, ct);
                var imageBytes = memoryStream.ToArray();
                var imageContent = BinaryData.FromBytes(imageBytes);

                var messages = new List<ChatMessage>
                {
                    new SystemChatMessage(
                       "Jesteś asystentem AI specjalizującym się w ekstrakcji danych z dokumentów medycznych. " +
                        "Twoim zadaniem jest odczytanie danych ze zdjęcia książeczki szczepień. " +
                        "Zwróć dane TYLKO w formacie JSON. Nie dodawaj żadnych znaczników markdown (```json). " +
                        "Struktura JSON ma zawierać listę obiektów: " +
                        "{ \"entries\": [ { \"vaccineName\": \"...\", \"batch\": \"...\", \"vaccinationDate\": \"YYYY-MM-DD\" } ] }. " +
                        "Jeśli jakaś wartość jest bardzo nieczytelna, wpisz null, a jeżeli nie jesteś pewny paru liter wpisuj co ci się wydaje słuszne. " +
                        "Lepiej żeby nazwy szczepionek i numery seryjne szczepionek były w połowie poprawne niż całkowicie puste. " +
                        "vaccineName to nazwa szczepionki, a nie choroby także szukaj nazw szczepionek medycznych." +
                        "Jeśli data jest niepełna, spróbuj ją uzupełnić lub wpisz null. Ignoruj puste wiersze. " +
                        "Często numery seryjne zapisane są jako \"S.numer\" ale to \"S.\" należy pominąć."),

                    new UserChatMessage(
                        ChatMessageContentPart.CreateTextPart("Odczytaj dane ze zdjęcia tej strony książeczki szczepień."),
                        ChatMessageContentPart.CreateImagePart(imageContent, "image/jpeg")
                    )
                };

                var options = new ChatCompletionOptions
                {
                    ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat()
                };

                ChatCompletion completion = await _chatClient.CompleteChatAsync(messages, options, ct);

                if (completion.Content != null && completion.Content.Count > 0)
                {
                    var jsonContent = completion.Content[0].Text;

                    var parseOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var result = JsonSerializer.Deserialize<GptResponseWrapper>(jsonContent, parseOptions);

                    if (result?.Entries != null)
                    {
                        allResults.AddRange(result.Entries);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"OCR error for file: {file.FileName}: {ex.Message}");
            }
        }

        return allResults;
    }
}
