using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignalR.BusinessLayer.Abstract;
using SignalR.DtoLayer.AboutDto;
using SignalR.EntityLayer.Entities;

namespace SignalRApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AboutController : ControllerBase
    {
        private readonly IAboutService _aboutService;
        private readonly IMapper _mapper;

        //Dependency Injection ile IAboutService arayüzü enjekte ediliyor
        public AboutController(IAboutService aboutService, IMapper mapper = null)
        {
            _aboutService = aboutService;
            _mapper = mapper;
        }
        //HTTP GET, sunucudan veri okumak veya listelemek için kullanılan istek yöntemidir
        [HttpGet]
        public IActionResult AboutList()
        {
            var values = _aboutService.TGetListAll();
            return Ok(_mapper.Map<List<ResultAboutDto>>(values));
        }
        //HTTP POST, sunucuya yeni veri eklemek veya işlem yaptırmak için kullanılan istek yöntemidir.
        [HttpPost]
        public IActionResult CreateAbout(CreateAboutDto createAboutDto)
        {
            var value = _mapper.Map<About>(createAboutDto);

            _aboutService.TAdd(value);
            return Ok("Hakkında kısmı başarılı bir şekilde eklendi");
        }
      //  HTTP DELETE, sunucudaki bir veriyi silmek için kullanılan istek yöntemidir
        [HttpDelete ("{id}")]
        public IActionResult DeleteAbout(int id)
        {
            var value = _aboutService.TGetByID(id);
            _aboutService.TDelete(value);
            return Ok("Hakkında kısmı başarılı bir şekilde silindi");
        }
        //HTTP PUT, sunucudaki mevcut bir veriyi güncellemek için kullanılan istek yöntemidir.
        [HttpPut]
        public IActionResult UpdateAbout(UpdateAboutDto updateAboutDto)
        {
            var value =_mapper.Map<About>(updateAboutDto);
             
            _aboutService.TUpdate(value);
            return Ok("Hakkında kısmı başarılı bir şekilde güncellendi");
        }
        [HttpGet("{id}")]
        public IActionResult GetAbout(int id)
        {
            var value = _aboutService.TGetByID(id);
            return Ok(_mapper.Map<GetAboutDto>(value));
        }

    }
}
