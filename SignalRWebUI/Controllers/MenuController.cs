using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SignalRWebUI.Dtos.BasketDtos;
using SignalRWebUI.Dtos.ProductDtos;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace SignalRWebUI.Controllers
{
    [AllowAnonymous]
    public class MenuController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public MenuController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IActionResult> Index(int id)
        {
            if (id > 0)
            {
                HttpContext.Session.SetInt32("ActiveTableId", id);
            }

            ViewBag.v = HttpContext.Session.GetInt32("ActiveTableId") ?? 0;
            
            var client = _httpClientFactory.CreateClient("SignalRApi");
            var responseMessage = await client.GetAsync("api/Product/ProductListWithCategory");
            var jsonData = await responseMessage.Content.ReadAsStringAsync();
            var values = JsonConvert.DeserializeObject<List<ResultProductDto>>(jsonData);
            return View(values);
        }
        [HttpPost]
        public async Task<IActionResult> AddBasket(int id, int menuTableId)
        {
            if (menuTableId == 0) 
            {
                return BadRequest("MenuTableID 0 geliyor.");
            }
            CreateBasketDto createBasketDto = new CreateBasketDto()
            {
                MenuTableID = menuTableId,
                ProductID = id,
            };

            var client = _httpClientFactory.CreateClient("SignalRApi");
            var jsonData = JsonConvert.SerializeObject(createBasketDto);
            StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var responseMessage = await client.PostAsync("api/Basket", stringContent);

            await client.GetAsync("api/MenuTables/ChangeMenuTableStatusToTrue?id=" + menuTableId);

            if (responseMessage.IsSuccessStatusCode)
            {
                HttpContext.Session.SetInt32("ActiveTableId", menuTableId);
                return RedirectToAction("Index", new { id = menuTableId });
            }
            return View();
        }
    }
}
