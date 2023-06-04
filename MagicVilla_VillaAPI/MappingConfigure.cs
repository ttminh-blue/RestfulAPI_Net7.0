using AutoMapper;
using MagicVilla_VillaAPI.Models;


namespace MagicVilla_VillaAPI
{
    public class MappingConfigure : Profile
    {
        public MappingConfigure() {
            CreateMap<Villa, VillaDto>();
            CreateMap<VillaDto, Villa>();

            CreateMap<Villa, VillaCreateDto>().ReverseMap();
            CreateMap<Villa, 
                VillaUpdateDto>().ReverseMap();

        }
    }
}
