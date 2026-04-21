using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignalR.BusinessLayer.Abstract;
using SignalR.DtoLayer.AboutDto;
using SignalR.EntiyLayer.Entities;
using static System.Net.WebRequestMethods;

namespace SignalRApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AboutController : ControllerBase
    {
        private readonly IAboutService _aboutService;

        //Dependency Injection ile IAboutService arayüzü enjekte ediliyor
        public AboutController(IAboutService aboutService)
        {
            _aboutService = aboutService;
        }
        //HTTP GET, sunucudan veri okumak veya listelemek için kullanılan istek yöntemidir
        [HttpGet]
        public IActionResult AboutList()
        {
            var values = _aboutService.TGetListAll();
            return Ok(values);
        }
        //HTTP POST, sunucuya yeni veri eklemek veya işlem yaptırmak için kullanılan istek yöntemidir.
        [HttpPost]
        public IActionResult CreateAbout(CreateAboutDto createAboutDto)
        {
            About about = new About()
            {
                Description = createAboutDto.Description,
                ImageUrl = createAboutDto.ImageUrl,
                Title = createAboutDto.Title
            };

            _aboutService.TAdd(about);
            return Ok("Hakkında kısmı başarılı bir şekilde eklendi");
        }
        //  HTTP DELETE, sunucudaki bir veriyi silmek için kullanılan istek yöntemidir
        [HttpDelete("{id}")]
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
            About about = new About()
            {
                AboutID = updateAboutDto.AboutID,
                Description = updateAboutDto.Description,
                ImageUrl = updateAboutDto.ImageUrl,
                Title = updateAboutDto.Title
            };
            _aboutService.TUpdate(about);
            return Ok("Hakkında kısmı başarılı bir şekilde güncellendi");
        }
        [HttpGet("{id}")]
        public IActionResult GetAbout(int id)
        {
            var value = _aboutService.TGetByID(id);
            return Ok(value);
        }
    }
}

