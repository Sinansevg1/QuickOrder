using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using SignalRApi.Options;

namespace SignalRApi.Services.Recommendations
{
    public class OpenAiLlmProvider : ILlmProvider
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<LlmRecommendationOptions> _options;
        public string ProviderName => "openai";
        public bool IsEnabled
        {
            get
            {
                var enabledText = EnvFileValueReader.Get("LlmRecommendation__OpenAi__Enabled");
                enabledText = string.IsNullOrWhiteSpace(enabledText)
                    ? Environment.GetEnvironmentVariable("LlmRecommendation__OpenAi__Enabled")
                    : enabledText;
                var isEnabled = bool.TryParse(enabledText, out var envEnabled)
                    ? envEnabled
                    : _options.Value.OpenAi.Enabled;

                return isEnabled && !string.IsNullOrWhiteSpace(GetEnvOrDefault("LlmRecommendation__OpenAi__ApiKey", _options.Value.OpenAi.ApiKey));
            }
        }

        public OpenAiLlmProvider(HttpClient httpClient, IOptions<LlmRecommendationOptions> options)
        {
            _httpClient = httpClient;
            _options = options;
        }

        public async Task<string?> GenerateAsync(string prompt, CancellationToken cancellationToken = default)
        {
            if (!IsEnabled)
            {
                return null;
            }

            var cfg = _options.Value.OpenAi;
            cfg.ApiKey = GetEnvOrDefault("LlmRecommendation__OpenAi__ApiKey", cfg.ApiKey);
            cfg.Model = GetEnvOrDefault("LlmRecommendation__OpenAi__Model", cfg.Model);
            cfg.Endpoint = GetEnvOrDefault("LlmRecommendation__OpenAi__Endpoint", cfg.Endpoint);
            var endpoint = string.IsNullOrWhiteSpace(cfg.Endpoint)
                ? "https://api.openai.com/v1/chat/completions"
                : cfg.Endpoint;

            var payload = new
            {
                model = cfg.Model,
                temperature = 0.3,
                messages = new object[]
                {
                    new { role = "system", content = "You are a food recommendation assistant. Return JSON only." },
                    new { role = "user", content = prompt }
                }
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", cfg.ApiKey);
            request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            using var doc = JsonDocument.Parse(content);
            if (!doc.RootElement.TryGetProperty("choices", out var choices) || choices.GetArrayLength() == 0)
            {
                return null;
            }

            return choices[0].GetProperty("message").GetProperty("content").GetString();
        }

        private static string GetEnvOrDefault(string key, string fallback)
        {
            var fileValue = EnvFileValueReader.Get(key);
            if (!string.IsNullOrWhiteSpace(fileValue))
            {
                return fileValue;
            }

            var env = Environment.GetEnvironmentVariable(key);
            return string.IsNullOrWhiteSpace(env) ? fallback : env;
        }
    }
}
