using AutoMapper;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Mapping.Profiles.PersonelIslemleri
{
    public class DepartmanMappingProfile : Profile
    {
        public DepartmanMappingProfile()
        {
            // Request -> Entity
            CreateMap<DepartmanCreateRequestDto, Departman>();
            CreateMap<DepartmanUpdateRequestDto, Departman>();

            // Entity -> Response
            CreateMap<Departman, DepartmanResponseDto>()
                .ForMember(dest => dest.PersonelSayisi,
                    opt => opt.MapFrom(src => src.Personeller != null ? src.Personeller.Count : 0));
        }
    }
}