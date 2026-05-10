namespace SignalRApi.Services.Recommendations
{
    public interface ILlmProvider
    {
        string ProviderName { get; }
        bool IsEnabled { get; }
        Task<string?> GenerateAsync(string prompt, CancellationToken cancellationToken = default);
    }
}
