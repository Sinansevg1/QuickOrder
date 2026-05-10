using SignalR.EntityLayer.Entities;

namespace SignalRApi.Services.Recommendations
{
    public static class RecommendationFallbackSelector
    {
        public static List<Product> Select(
            List<Product> candidates,
            HashSet<int> basketCategoryIds,
            int maxSuggestions)
        {
            if (candidates.Count == 0 || maxSuggestions <= 0)
            {
                return new List<Product>();
            }

            var random = Random.Shared;
            var prioritized = candidates
                .Where(x => !basketCategoryIds.Contains(x.CategoryID))
                .OrderBy(_ => random.Next())
                .Take(maxSuggestions)
                .ToList();

            if (prioritized.Count >= maxSuggestions)
            {
                return prioritized;
            }

            var remain = candidates
                .Where(x => prioritized.All(p => p.ProductID != x.ProductID))
                .OrderBy(_ => random.Next())
                .Take(maxSuggestions - prioritized.Count)
                .ToList();

            prioritized.AddRange(remain);
            return prioritized;
        }
    }
}
