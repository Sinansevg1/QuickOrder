using AutoMapper;
using SignalR.DtoLayer.MenuTableDto;
using SignalR.EntityLayer.Entities;

namespace SignalRApi.Mapping
{
    public class ManuTableMapping : Profile
    {
        public ManuTableMapping()
        {
            CreateMap<MenuTable, ResultMenuTableDto>();
            CreateMap<MenuTable, CreateMenuTableDto>();
            CreateMap<MenuTable, UpdateMenuTableDto>();
            CreateMap<MenuTable, GetMenuTableDto>();
        }
    }
}
