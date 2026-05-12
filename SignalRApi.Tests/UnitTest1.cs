using SignalR.EntityLayer.Entities;
using SignalRApi.Services.Recommendations;

namespace SignalRApi.Tests;

public class UnitTest1
{
    [Fact]
    public void Parse_ShouldHandleJsonWrappedInMarkdown()
    {
        var raw = """
                  ```json
                  {"suggestions":[{"productId":12,"reason":"Drinks pair well"}]}
                  ```
                  """;

        var parsed = RecommendationResponseParser.Parse(raw);

        Assert.NotNull(parsed);
        Assert.Single(parsed!.Suggestions);
        Assert.Equal(12, parsed.Suggestions[0].ProductId);
    }

    [Fact]
    public void Fallback_ShouldPrioritizeDifferentCategoriesFirst()
    {
        var candidates = new List<Product>
        {
            new() { ProductID = 1, ProductName = "Cola", CategoryID = 2, Price = 40, ImageUrl = "", Desctiption = "", ProductStatus = true, Category = new Category { CategoryID = 2, CategoryName = "Drink", CategoryStatus = true } },
            new() { ProductID = 2, ProductName = "Cake", CategoryID = 3, Price = 80, ImageUrl = "", Desctiption = "", ProductStatus = true, Category = new Category { CategoryID = 3, CategoryName = "Dessert", CategoryStatus = true } },
            new() { ProductID = 3, ProductName = "Burger2", CategoryID = 1, Price = 120, ImageUrl = "", Desctiption = "", ProductStatus = true, Category = new Category { CategoryID = 1, CategoryName = "Main", CategoryStatus = true } }
        };

        var selected = RecommendationFallbackSelector.Select(candidates, new HashSet<int> { 1 }, 2);

        Assert.Equal(2, selected.Count);
        Assert.Contains(selected, x => x.CategoryID == 2);
        Assert.Contains(selected, x => x.CategoryID == 3);
    }
}
