using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using SignalRApi.Options;

namespace SignalRApi.Services.Recommendations
{
    public class GeminiLlmProvider : ILlmProvider
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<LlmRecommendationOptions> _options;
        private readonly ILogger<GeminiLlmProvider> _logger;

        public string ProviderName => "gemini";
        public bool IsEnabled
        {
            get
            {
                var enabledText = EnvFileValueReader.Get("LlmRecommendation__Gemini__Enabled");
                enabledText = string.IsNullOrWhiteSpace(enabledText)
                    ? Environment.GetEnvironmentVariable("LlmRecommendation__Gemini__Enabled")
                    : enabledText;
                var isEnabled = bool.TryParse(enabledText, out var envEnabled)
                    ? envEnabled
                    : _options.Value.Gemini.Enabled;

                return isEnabled && !string.IsNullOrWhiteSpace(GetApiKey());
            }
        }

        public GeminiLlmProvider(HttpClient httpClient, IOptions<LlmRecommendationOptions> options, ILogger<GeminiLlmProvider> logger)
        {
            _httpClient = httpClient;
            _options = options;
            _logger = logger;
        }

        public async Task<string?> GenerateAsync(string prompt, CancellationToken cancellationToken = default)
        {
            if (!IsEnabled)
            {
                return null;
            }

            var cfg = _options.Value.Gemini;
            cfg.ApiKey = GetApiKey();
            cfg.Model = GetEnvOrDefault("LlmRecommendation__Gemini__Model", cfg.Model);
            cfg.Endpoint = GetEnvOrDefault("LlmRecommendation__Gemini__Endpoint", cfg.Endpoint);
            var endpoint = BuildEndpoint(cfg);

            var payload = new
            {
                generationConfig = new
                {
                    temperature = 0.3,
                    responseMimeType = "application/json"
                },
                contents = new object[]
                {
                    new
                    {
                        role = "user",
                        parts = new object[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning("Gemini request failed. Status: {StatusCode}. Body: {Body}", (int)response.StatusCode, errorBody);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            using var doc = JsonDocument.Parse(content);
            if (!doc.RootElement.TryGetProperty("candidates", out var candidates) || candidates.GetArrayLength() == 0)
            {
                return null;
            }

            return candidates[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();
        }

        private static string BuildEndpoint(LlmProviderOptions cfg)
        {
            if (string.IsNullOrWhiteSpace(cfg.Endpoint))
            {
                return $"https://generativelanguage.googleapis.com/v1beta/models/{cfg.Model}:generateContent?key={cfg.ApiKey}";
            }

            if (cfg.Endpoint.Contains("{apiKey}", StringComparison.OrdinalIgnoreCase))
            {
                return cfg.Endpoint.Replace("{apiKey}", cfg.ApiKey, StringComparison.OrdinalIgnoreCase);
            }

            if (cfg.Endpoint.Contains("key=", StringComparison.OrdinalIgnoreCase))
            {
                return cfg.Endpoint;
            }

            var separator = cfg.Endpoint.Contains('?') ? "&" : "?";
            return $"{cfg.Endpoint}{separator}key={cfg.ApiKey}";
        }

        private string GetApiKey()
        {
            return GetEnvOrDefault("LlmRecommendation__Gemini__ApiKey", _options.Value.Gemini.ApiKey);
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
