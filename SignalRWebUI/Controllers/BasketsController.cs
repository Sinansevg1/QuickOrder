using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SignalRWebUI.Dtos.BasketDtos;
using SignalRWebUI.Dtos.RecommendationDtos;
using SignalRWebUI.ViewModels;
using System.Text;

namespace SignalRWebUI.Controllers
{
    public class BasketsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public BasketsController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IActionResult> Index(int id)
        {
            if (id <= 0)
            {
                id = HttpContext.Session.GetInt32("ActiveTableId") ?? 0;
            }

            if (id > 0)
            {
                HttpContext.Session.SetInt32("ActiveTableId", id);
            }

            TempData["tableId"] = id;
            TempData.Keep("tableId");
            var pageModel = await BuildBasketPageModelAsync(id);
            return View(pageModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetBasketData(int id)
        {
            if (id <= 0)
            {
                id = HttpContext.Session.GetInt32("ActiveTableId") ?? 0;
            }
            if (id <= 0)
            {
                return Json(new BasketPageViewModel());
            }

            var model = await BuildBasketPageModelAsync(id);
            return Json(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBasketAjax(int basketId, int menuTableId)
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.DeleteAsync($"https://localhost:7201/api/Basket/{basketId}");
            if (responseMessage.IsSuccessStatusCode)
            {
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Urun silinemedi." });
        }

        [HttpPost]
        public async Task<IActionResult> AddSuggestedProduct(int productId, int menuTableId)
        {
            var client = _httpClientFactory.CreateClient();
            var basketCheckResponse = await client.GetAsync("https://localhost:7201/api/Basket/BasketListBtByMenuTablewithProductName?id=" + menuTableId);
            if (basketCheckResponse.IsSuccessStatusCode)
            {
                var currentBasketJson = await basketCheckResponse.Content.ReadAsStringAsync();
                var currentBasket = JsonConvert.DeserializeObject<List<ResultBasketDto>>(currentBasketJson) ?? new List<ResultBasketDto>();
                if (currentBasket.Any(x => x.ProductID == productId))
                {
                    return Json(new { success = false, alreadyExists = true, message = "Bu urun zaten sepetinizde var." });
                }
            }

            CreateBasketDto createBasketDto = new CreateBasketDto
            {
                MenuTableID = menuTableId,
                ProductID = productId
            };

            var jsonData = JsonConvert.SerializeObject(createBasketDto);
            StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var responseMessage = await client.PostAsync("https://localhost:7201/api/Basket", stringContent);
            if (responseMessage.IsSuccessStatusCode)
            {
                return Json(new { success = true, alreadyExists = false });
            }

            return Json(new { success = false, alreadyExists = false, message = "Urun sepete eklenemedi." });
        }

        private async Task<BasketPageViewModel> BuildBasketPageModelAsync(int tableId)
        {
            var pageModel = new BasketPageViewModel
            {
                TableId = tableId
            };

            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync("https://localhost:7201/api/Basket/BasketListBtByMenuTablewithProductName?id=" + tableId);
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultBasketDto>>(jsonData);
                pageModel.Baskets = values ?? new List<ResultBasketDto>();
            }

            var recommendationClient = _httpClientFactory.CreateClient();
            var recommendationResponse = await recommendationClient.GetAsync("https://localhost:7201/api/Recommendation/by-menu-table?id=" + tableId);
            if (recommendationResponse.IsSuccessStatusCode)
            {
                var recommendationJson = await recommendationResponse.Content.ReadAsStringAsync();
                var recommendationValues = JsonConvert.DeserializeObject<BasketRecommendationResponseDto>(recommendationJson);
                pageModel.Recommendations = recommendationValues ?? new BasketRecommendationResponseDto();
            }

            return pageModel;
        }
    }
}
