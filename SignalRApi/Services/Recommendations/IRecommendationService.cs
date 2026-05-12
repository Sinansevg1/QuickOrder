using SignalR.DtoLayer.RecommendationDto;

namespace SignalRApi.Services.Recommendations
{
    public interface IRecommendationService
    {
        Task<BasketRecommendationResponseDto> GetByMenuTableIdAsync(int menuTableId, CancellationToken cancellationToken = default);
    }
}
