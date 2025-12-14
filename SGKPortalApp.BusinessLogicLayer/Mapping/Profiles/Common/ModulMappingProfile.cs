using AutoMapper;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;

namespace SGKPortalApp.BusinessLogicLayer.Mapping.Profiles.Common
{
    public class ModulMappingProfile : Profile
    {
        public ModulMappingProfile()
        {
            CreateMap<Modul, ModulResponseDto>()
                .ForMember(dest => dest.ControllerCount, opt => opt.Ignore());

            CreateMap<ModulCreateRequestDto, Modul>()
                .ForMember(dest => dest.ModulId, opt => opt.Ignore())
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.SilindiMi, opt => opt.Ignore())
                .ForMember(dest => dest.SilinmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.SilenKullanici, opt => opt.Ignore())
                .ForMember(dest => dest.EkleyenKullanici, opt => opt.Ignore())
                .ForMember(dest => dest.DuzenleyenKullanici, opt => opt.Ignore())
                .ForMember(dest => dest.ModulAltlari, opt => opt.Ignore())
                .ForMember(dest => dest.ModulControllers, opt => opt.Ignore());

            CreateMap<ModulUpdateRequestDto, Modul>()
                .ForMember(dest => dest.ModulId, opt => opt.Ignore())
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.SilindiMi, opt => opt.Ignore())
                .ForMember(dest => dest.SilinmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.SilenKullanici, opt => opt.Ignore())
                .ForMember(dest => dest.EkleyenKullanici, opt => opt.Ignore())
                .ForMember(dest => dest.DuzenleyenKullanici, opt => opt.Ignore())
                .ForMember(dest => dest.ModulAltlari, opt => opt.Ignore())
                .ForMember(dest => dest.ModulControllers, opt => opt.Ignore());
        }
    }
}
