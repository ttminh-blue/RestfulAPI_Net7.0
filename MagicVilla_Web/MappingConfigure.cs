using AutoMapper;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Models.VM;

namespace MagicVilla_Web
{
    public class MappingConfigure : Profile
    {
        public MappingConfigure() {
            CreateMap<VillaDto, VillaCreateDto>().ReverseMap();
            CreateMap<VillaDto, VillaUpdateDto>().ReverseMap();
            CreateMap<VillaNumberDto, VillaNumberCreateDto>().ReverseMap();
            CreateMap<VillaNumberDto, VillaNumberUpdateDto>().ReverseMap();

			CreateMap<VillaNumberUpdateVM, VillaNumberUpdateDto>().ReverseMap();

			CreateMap<VillaNumberUpdateVM, VillaUpdateDto>().ReverseMap();

			CreateMap<VillaUpdateDto, VillaNumberUpdateVM>().ReverseMap();


			CreateMap<VillaNumberUpdateDto, VillaNumberUpdateVM>().ReverseMap();


		}
	}
}
