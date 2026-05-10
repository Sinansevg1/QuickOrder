using System.Text.Json;
using SignalR.DtoLayer.RecommendationDto;

namespace SignalRApi.Services.Recommendations
{
    public static class RecommendationResponseParser
    {
        public static LlmRecommendationResultDto? Parse(string? rawResponse)
        {
            if (string.IsNullOrWhiteSpace(rawResponse))
            {
                return null;
            }

            var normalized = rawResponse.Trim();
            if (normalized.StartsWith("```"))
            {
                normalized = normalized.Replace("```json", string.Empty, StringComparison.OrdinalIgnoreCase);
                normalized = normalized.Replace("```", string.Empty);
                normalized = normalized.Trim();
            }

            try
            {
                return JsonSerializer.Deserialize<LlmRecommendationResultDto>(
                    normalized,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
            }
            catch
            {
                return null;
            }
        }
    }
}
