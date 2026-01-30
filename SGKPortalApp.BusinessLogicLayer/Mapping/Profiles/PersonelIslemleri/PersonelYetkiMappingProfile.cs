using AutoMapper;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.Common.Helpers;

namespace SGKPortalApp.BusinessLogicLayer.Mapping.Profiles.PersonelIslemleri
{
    public class PersonelYetkiMappingProfile : Profile
    {
        public PersonelYetkiMappingProfile()
        {
            CreateMap<PersonelYetki, PersonelYetkiResponseDto>()
                .ForMember(dest => dest.ModulControllerIslemAdi, opt => opt.MapFrom(src => src.ModulControllerIslem != null ? src.ModulControllerIslem.ModulControllerIslemAdi : string.Empty))
                .ForMember(dest => dest.PermissionKey, opt => opt.MapFrom(src => src.ModulControllerIslem != null ? src.ModulControllerIslem.PermissionKey : string.Empty))
                .ForMember(dest => dest.IslemTipi, opt => opt.MapFrom(src => src.ModulControllerIslem != null ? src.ModulControllerIslem.IslemTipi : default));

            CreateMap<PersonelYetkiCreateRequestDto, PersonelYetki>()
                .ForMember(dest => dest.PersonelYetkiId, opt => opt.Ignore())
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.MapFrom(_ => DateTimeHelper.Now))
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.MapFrom(_ => DateTimeHelper.Now))
                .ForMember(dest => dest.SilindiMi, opt => opt.Ignore())
                .ForMember(dest => dest.SilinmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.SilenKullanici, opt => opt.Ignore())
                .ForMember(dest => dest.EkleyenKullanici, opt => opt.Ignore())
                .ForMember(dest => dest.DuzenleyenKullanici, opt => opt.Ignore())
                .ForMember(dest => dest.Personel, opt => opt.Ignore())
                .ForMember(dest => dest.ModulControllerIslem, opt => opt.Ignore());

            CreateMap<PersonelYetkiUpdateRequestDto, PersonelYetki>()
                .ForMember(dest => dest.PersonelYetkiId, opt => opt.Ignore())
                .ForMember(dest => dest.TcKimlikNo, opt => opt.Ignore())
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.MapFrom(_ => DateTimeHelper.Now))
                .ForMember(dest => dest.SilindiMi, opt => opt.Ignore())
                .ForMember(dest => dest.SilinmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.SilenKullanici, opt => opt.Ignore())
                .ForMember(dest => dest.EkleyenKullanici, opt => opt.Ignore())
                .ForMember(dest => dest.DuzenleyenKullanici, opt => opt.Ignore())
                .ForMember(dest => dest.Personel, opt => opt.Ignore())
                .ForMember(dest => dest.ModulControllerIslem, opt => opt.Ignore());
        }
    }
}
