using Microsoft.AspNetCore.Mvc;
using SignalRApi.Services.Recommendations;

namespace SignalRApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecommendationController : ControllerBase
    {
        private readonly IRecommendationService _recommendationService;

        public RecommendationController(IRecommendationService recommendationService)
        {
            _recommendationService = recommendationService;
        }

        [HttpGet("by-menu-table")]
        public async Task<IActionResult> GetByMenuTableId(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
            {
                return BadRequest("Gecerli bir menuTableId gonderilmelidir.");
            }

            var result = await _recommendationService.GetByMenuTableIdAsync(id, cancellationToken);
            return Ok(result);
        }
    }
}
