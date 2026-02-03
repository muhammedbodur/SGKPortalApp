using AutoMapper;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;

namespace SGKPortalApp.BusinessLogicLayer.Mappings
{
    public class DepartmanHizmetBinasiMappingProfile : Profile
    {
        public DepartmanHizmetBinasiMappingProfile()
        {
            // Entity -> Response DTO
            CreateMap<DepartmanHizmetBinasi, DepartmanHizmetBinasiResponseDto>()
                .ForMember(dest => dest.DepartmanAdi, opt => opt.MapFrom(src => src.Departman != null ? src.Departman.DepartmanAdi : string.Empty))
                .ForMember(dest => dest.HizmetBinasiAdi, opt => opt.MapFrom(src => src.HizmetBinasi != null ? src.HizmetBinasi.HizmetBinasiAdi : string.Empty));

            // Request DTO -> Entity
            CreateMap<DepartmanHizmetBinasiCreateRequestDto, DepartmanHizmetBinasi>();
            CreateMap<DepartmanHizmetBinasiUpdateRequestDto, DepartmanHizmetBinasi>();

            // Entity -> Dropdown DTO
            CreateMap<DepartmanHizmetBinasi, DepartmanHizmetBinasiDto>()
                .ForMember(dest => dest.DepartmanAdi, opt => opt.MapFrom(src => src.Departman != null ? src.Departman.DepartmanAdi : string.Empty))
                .ForMember(dest => dest.HizmetBinasiAdi, opt => opt.MapFrom(src => src.HizmetBinasi != null ? src.HizmetBinasi.HizmetBinasiAdi : string.Empty));

            // Response DTO -> Dropdown DTO
            CreateMap<DepartmanHizmetBinasiResponseDto, DepartmanHizmetBinasiDto>();
        }
    }
}
