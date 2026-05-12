namespace SignalR.DtoLayer.RecommendationDto
{
    public class BasketRecommendationResponseDto
    {
        public int MenuTableId { get; set; }
        public string ProviderUsed { get; set; } = string.Empty;
        public bool IsFallback { get; set; }
        public List<RecommendationProductDto> Suggestions { get; set; } = new();
    }
}
