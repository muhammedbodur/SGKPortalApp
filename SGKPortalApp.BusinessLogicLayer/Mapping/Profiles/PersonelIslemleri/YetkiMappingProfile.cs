using AutoMapper;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Mapping.Profiles.PersonelIslemleri
{
    public class YetkiMappingProfile : Profile
    {
        public YetkiMappingProfile()
        {
            CreateMap<Yetki, YetkiResponseDto>();

            CreateMap<YetkiCreateRequestDto, Yetki>()
                .ForMember(dest => dest.YetkiId, opt => opt.Ignore())
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.SilindiMi, opt => opt.Ignore())
                .ForMember(dest => dest.SilinmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.SilenKullanici, opt => opt.Ignore())
                .ForMember(dest => dest.EkleyenKullanici, opt => opt.Ignore())
                .ForMember(dest => dest.DuzenleyenKullanici, opt => opt.Ignore())
                .ForMember(dest => dest.UstYetki, opt => opt.Ignore())
                .ForMember(dest => dest.AltYetkiler, opt => opt.Ignore())
                .ForMember(dest => dest.PersonelYetkileri, opt => opt.Ignore());

            CreateMap<YetkiUpdateRequestDto, Yetki>()
                .ForMember(dest => dest.YetkiId, opt => opt.Ignore())
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.SilindiMi, opt => opt.Ignore())
                .ForMember(dest => dest.SilinmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.SilenKullanici, opt => opt.Ignore())
                .ForMember(dest => dest.EkleyenKullanici, opt => opt.Ignore())
                .ForMember(dest => dest.DuzenleyenKullanici, opt => opt.Ignore())
                .ForMember(dest => dest.UstYetki, opt => opt.Ignore())
                .ForMember(dest => dest.AltYetkiler, opt => opt.Ignore())
                .ForMember(dest => dest.PersonelYetkileri, opt => opt.Ignore());
        }
    }
}
