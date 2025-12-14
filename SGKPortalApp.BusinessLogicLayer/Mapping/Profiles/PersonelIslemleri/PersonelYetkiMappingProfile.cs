using AutoMapper;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Mapping.Profiles.PersonelIslemleri
{
    public class PersonelYetkiMappingProfile : Profile
    {
        public PersonelYetkiMappingProfile()
        {
            CreateMap<PersonelYetki, PersonelYetkiResponseDto>()
                .ForMember(dest => dest.YetkiAdi, opt => opt.MapFrom(src => src.Yetki != null ? src.Yetki.YetkiAdi : null))
                .ForMember(dest => dest.ModulControllerIslemAdi, opt => opt.MapFrom(src => src.ModulControllerIslem != null ? src.ModulControllerIslem.ModulControllerIslemAdi : null));

            CreateMap<PersonelYetkiCreateRequestDto, PersonelYetki>()
                .ForMember(dest => dest.PersonelYetkiId, opt => opt.Ignore())
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.SilindiMi, opt => opt.Ignore())
                .ForMember(dest => dest.SilinmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.SilenKullanici, opt => opt.Ignore())
                .ForMember(dest => dest.EkleyenKullanici, opt => opt.Ignore())
                .ForMember(dest => dest.DuzenleyenKullanici, opt => opt.Ignore())
                .ForMember(dest => dest.Personel, opt => opt.Ignore())
                .ForMember(dest => dest.Yetki, opt => opt.Ignore())
                .ForMember(dest => dest.ModulControllerIslem, opt => opt.Ignore());

            CreateMap<PersonelYetkiUpdateRequestDto, PersonelYetki>()
                .ForMember(dest => dest.PersonelYetkiId, opt => opt.Ignore())
                .ForMember(dest => dest.TcKimlikNo, opt => opt.Ignore())
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.SilindiMi, opt => opt.Ignore())
                .ForMember(dest => dest.SilinmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.SilenKullanici, opt => opt.Ignore())
                .ForMember(dest => dest.EkleyenKullanici, opt => opt.Ignore())
                .ForMember(dest => dest.DuzenleyenKullanici, opt => opt.Ignore())
                .ForMember(dest => dest.Personel, opt => opt.Ignore())
                .ForMember(dest => dest.Yetki, opt => opt.Ignore())
                .ForMember(dest => dest.ModulControllerIslem, opt => opt.Ignore());
        }
    }
}
