using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SignalRWebUI.Dtos.RapidApiDtos;

namespace SignalRWebUI.Controllers
{
    public class FoodRapidApiController : Controller
    {
        private readonly IConfiguration _configuration;

        public FoodRapidApiController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var apiKey  = _configuration["RapidApi:Key"]
                ?? throw new InvalidOperationException("RapidApi:Key yapılandırılmamış.");
            var apiHost = _configuration["RapidApi:Host"] ?? "tasty.p.rapidapi.com";

            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://{apiHost}/recipes/list?from=0&size=60&tags=under_30_minutes"),
                Headers =
                {
                    { "x-rapidapi-key",  apiKey  },
                    { "x-rapidapi-host", apiHost },
                },
            };

            using var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            var root = JsonConvert.DeserializeObject<RootTastyApi>(body);
            return View(root!.Results.ToList());
        }
    }
}
