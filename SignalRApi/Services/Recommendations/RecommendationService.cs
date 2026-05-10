using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SignalR.DataAccessLayer.concrete;
using SignalR.DtoLayer.RecommendationDto;
using SignalR.EntityLayer.Entities;
using SignalRApi.Options;

namespace SignalRApi.Services.Recommendations
{
    public class RecommendationService : IRecommendationService
    {
        private readonly SignalRContext _context;
        private readonly IEnumerable<ILlmProvider> _providers;
        private readonly IOptions<LlmRecommendationOptions> _options;
        private readonly ILogger<RecommendationService> _logger;

        public RecommendationService(
            SignalRContext context,
            IEnumerable<ILlmProvider> providers,
            IOptions<LlmRecommendationOptions> options,
            ILogger<RecommendationService> logger)
        {
            _context = context;
            _providers = providers;
            _options = options;
            _logger = logger;
        }

        public async Task<BasketRecommendationResponseDto> GetByMenuTableIdAsync(int menuTableId, CancellationToken cancellationToken = default)
        {
            var basketItems = await _context.Baskets
                .AsNoTracking()
                .Where(x => x.MenuTableID == menuTableId)
                .Include(x => x.Product)
                .ThenInclude(x => x.Category)
                .ToListAsync(cancellationToken);

            if (basketItems.Count == 0)
            {
                return new BasketRecommendationResponseDto { MenuTableId = menuTableId };
            }

            var basketProductIds = basketItems.Select(x => x.ProductID).Distinct().ToHashSet();
            var basketCategoryIds = basketItems.Select(x => x.Product.CategoryID).Distinct().ToHashSet();

            var candidates = await _context.Products
                .AsNoTracking()
                .Where(x => x.ProductStatus && !basketProductIds.Contains(x.ProductID))
                .Include(x => x.Category)
                .ToListAsync(cancellationToken);

            if (candidates.Count == 0)
            {
                return new BasketRecommendationResponseDto { MenuTableId = menuTableId };
            }

            var maxSuggestions = Math.Max(1, _options.Value.MaxSuggestions);
            var candidateMap = candidates.ToDictionary(x => x.ProductID);
            var prompt = BuildPrompt(basketItems.Select(x => x.Product).ToList(), candidates, maxSuggestions);

            var llmResponse = new BasketRecommendationResponseDto
            {
                MenuTableId = menuTableId
            };

            if (_options.Value.Enabled)
            {
                foreach (var provider in ResolveProviderOrder())
                {
                    using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                    timeoutCts.CancelAfter(TimeSpan.FromSeconds(Math.Max(5, _options.Value.TimeoutSeconds)));

                    string? raw;
                    try
                    {
                        raw = await provider.GenerateAsync(prompt, timeoutCts.Token);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Recommendation provider {ProviderName} failed.", provider.ProviderName);
                        continue;
                    }

                    var parsed = RecommendationResponseParser.Parse(raw);
                    if (parsed?.Suggestions is null || parsed.Suggestions.Count == 0)
                    {
                        continue;
                    }

                    var validated = parsed.Suggestions
                        .Where(x => candidateMap.ContainsKey(x.ProductId))
                        .GroupBy(x => x.ProductId)
                        .Select(x => x.First())
                        .Take(maxSuggestions)
                        .ToList();

                    if (validated.Count == 0)
                    {
                        continue;
                    }

                    llmResponse.ProviderUsed = provider.ProviderName;
                    llmResponse.IsFallback = false;
                    llmResponse.Suggestions = validated
                        .Select(x =>
                        {
                            var product = candidateMap[x.ProductId];
                            return new RecommendationProductDto
                            {
                                ProductId = product.ProductID,
                                ProductName = product.ProductName,
                                Price = product.Price,
                                ImageUrl = product.ImageUrl,
                                CategoryName = product.Category?.CategoryName ?? string.Empty,
                                Reason = x.Reason
                            };
                        })
                        .ToList();

                    return llmResponse;
                }
            }

            var fallback = RecommendationFallbackSelector.Select(candidates, basketCategoryIds, maxSuggestions);
            llmResponse.ProviderUsed = "rule_based_fallback";
            llmResponse.IsFallback = true;
            llmResponse.Suggestions = fallback.Select(x => new RecommendationProductDto
            {
                ProductId = x.ProductID,
                ProductName = x.ProductName,
                Price = x.Price,
                ImageUrl = x.ImageUrl,
                CategoryName = x.Category?.CategoryName ?? string.Empty,
                Reason = "Bu urunler sepetinizle uyumlu olabilir."
            }).ToList();

            return llmResponse;
        }

        private IEnumerable<ILlmProvider> ResolveProviderOrder()
        {
            var providerMap = _providers
                .Where(x => x.IsEnabled)
                .ToDictionary(x => x.ProviderName, x => x, StringComparer.OrdinalIgnoreCase);

            foreach (var providerName in _options.Value.ProviderPriority)
            {
                if (providerMap.TryGetValue(providerName, out var provider))
                {
                    yield return provider;
                }
            }
        }

        private static string BuildPrompt(List<Product> basketProducts, List<Product> candidateProducts, int maxSuggestions)
        {
            var builder = new StringBuilder();
            builder.AppendLine("Bir restoran uygulamasi icin urun onerisi uretiyorsun.");
            builder.AppendLine("Sadece aday listeden secim yap.");
            builder.AppendLine("Aciklama metinleri kesinlikle Turkce olmalidir.");
            builder.AppendLine("Ingilizce tek kelime bile kullanma.");
            builder.AppendLine($"Yalnizca su JSON formatinda cevap ver: {{\"suggestions\":[{{\"productId\":number,\"reason\":\"string\"}}]}}.");
            builder.AppendLine($"En fazla {maxSuggestions} oneride bulun.");
            builder.AppendLine("Sadece CANDIDATES listesindeki productId degerlerini kullan.");
            builder.AppendLine();
            builder.AppendLine("BASKET_PRODUCTS:");

            foreach (var product in basketProducts.DistinctBy(x => x.ProductID))
            {
                builder.AppendLine($"- id:{product.ProductID}, name:{product.ProductName}, category:{product.Category?.CategoryName}");
            }

            builder.AppendLine();
            builder.AppendLine("CANDIDATES:");
            foreach (var candidate in candidateProducts)
            {
                builder.AppendLine($"- id:{candidate.ProductID}, name:{candidate.ProductName}, category:{candidate.Category?.CategoryName}, price:{candidate.Price}");
            }

            return builder.ToString();
        }
    }
}
