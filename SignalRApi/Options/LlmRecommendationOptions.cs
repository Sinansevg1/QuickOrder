namespace SignalRApi.Options
{
    public class LlmRecommendationOptions
    {
        public bool Enabled { get; set; } = true;
        public int MaxSuggestions { get; set; } = 4;
        public int TimeoutSeconds { get; set; } = 12;
        public List<string> ProviderPriority { get; set; } = new() { "openai", "gemini" };
        public LlmProviderOptions OpenAi { get; set; } = new();
        public LlmProviderOptions Gemini { get; set; } = new();
    }

    public class LlmProviderOptions
    {
        public bool Enabled { get; set; } = false;
        public string ApiKey { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string Endpoint { get; set; } = string.Empty;
    }
}
