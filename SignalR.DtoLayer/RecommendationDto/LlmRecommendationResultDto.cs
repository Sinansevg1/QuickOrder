namespace SignalR.DtoLayer.RecommendationDto
{
    public class LlmRecommendationResultDto
    {
        public List<LlmRecommendationItemDto> Suggestions { get; set; } = new();
    }

    public class LlmRecommendationItemDto
    {
        public int ProductId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
