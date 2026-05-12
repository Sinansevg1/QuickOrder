using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignalR.BusinessLayer.Abstract;
using SignalR.DtoLayer.DiscountDto;
using SignalR.EntityLayer.Entities;
using AutoMapper;
namespace SignalRApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountController : ControllerBase
    { private readonly IDiscountService _disvountService;
      private readonly IMapper _mapper;

        public DiscountController(IDiscountService disvountService, IMapper mapper)
        {
            _disvountService = disvountService;
            _mapper = mapper;
        }
        [HttpGet]
        public IActionResult DiscountList()
        {
            var values = _mapper.Map<List<ResultDiscountDto>>(_disvountService.TGetListAll());
            return Ok(values);
        }
        [HttpPost]
        public IActionResult CreateDiscount(CreateDiscountDto createDiscountDto)
        {
           var values = _mapper.Map<Discount>(createDiscountDto);
            _disvountService.TAdd(values);
            return Ok("indirim kodu eklendi");
        }
        [HttpDelete("{id}")]    
        public IActionResult DeleteDiscount(int id)
        {
            var values = _disvountService.TGetByID(id);
            _disvountService.TDelete(values);
            return Ok("indirim kodu silindi");
        }
        [HttpGet("{id}")]
        public IActionResult GetDiscount(int id)
        {
            var values = _disvountService.TGetByID(id);
            return Ok(_mapper.Map<GetDiscountDto>(values));
        }
        [HttpPut]
        public IActionResult UpdateDiscount(UpdateDiscountDto updateDiscountDto)
        {
           var values = _mapper.Map<Discount>(updateDiscountDto);
            _disvountService.TUpdate(values);
            return Ok("indirim kodu güncellendi");
        }
        [HttpGet("ChangeStatusToTrue/{id}")]
        public IActionResult ChangeStatusToTrue(int id) 
        {
            _disvountService.TChangeStatusToTrue(id);
            return Ok("Ürün İndirimi Aktif Hale Getirildi");
        }
        [HttpGet("ChangeStatusToFalse/{id}")]
        public IActionResult ChangeStatusToFalse(int id) 
        {
            _disvountService.TChangeStatusToFalse(id);
            return Ok("Ürün İndirimi Pasif Hale Getirildi");
        }
        [HttpGet("GetListByStatusTrue")]
        public IActionResult GetListByStatusTrue() 
        {
            return Ok(_disvountService.TGetListByStatusTrue());
        }

    }
}
