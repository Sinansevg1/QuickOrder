using SignalRWebUI.Dtos.BasketDtos;
using SignalRWebUI.Dtos.RecommendationDtos;

namespace SignalRWebUI.ViewModels
{
    public class BasketPageViewModel
    {
        public int TableId { get; set; }
        public List<ResultBasketDto> Baskets { get; set; } = new();
        public BasketRecommendationResponseDto Recommendations { get; set; } = new();
    }
}
