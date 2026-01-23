using AutoMapper;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.BusinessLogicLayer.Mapping.Profiles.SiramatikIslemleri
{
    public class KioskMappingProfile : Profile
    {
        public KioskMappingProfile()
        {
            // Entity -> DTO
            CreateMap<KioskMenu, KioskMenuResponseDto>();

            CreateMap<Kiosk, KioskResponseDto>()
                .ForMember(dest => dest.HizmetBinasiAdi,
                    opt => opt.MapFrom(src => src.DepartmanHizmetBinasi != null && src.DepartmanHizmetBinasi.HizmetBinasi != null ? src.DepartmanHizmetBinasi.HizmetBinasi.HizmetBinasiAdi : null))
                .ForMember(dest => dest.DepartmanId,
                    opt => opt.MapFrom(src => src.DepartmanHizmetBinasi != null ? src.DepartmanHizmetBinasi.DepartmanId : (int?)null))
                .ForMember(dest => dest.DepartmanAdi,
                    opt => opt.MapFrom(src => src.DepartmanHizmetBinasi != null && src.DepartmanHizmetBinasi.Departman != null ? src.DepartmanHizmetBinasi.Departman.DepartmanAdi : null))
                .ForMember(dest => dest.AtananKioskMenuId,
                    opt => opt.MapFrom(src => src.MenuAtamalari != null && src.MenuAtamalari.Any(ma => ma.Aktiflik == Aktiflik.Aktif) 
                        ? src.MenuAtamalari.First(ma => ma.Aktiflik == Aktiflik.Aktif).KioskMenuId 
                        : (int?)null))
                .ForMember(dest => dest.AtananKioskMenuAdi,
                    opt => opt.MapFrom(src => src.MenuAtamalari != null && src.MenuAtamalari.Any(ma => ma.Aktiflik == Aktiflik.Aktif) 
                        ? src.MenuAtamalari.First(ma => ma.Aktiflik == Aktiflik.Aktif).KioskMenu.MenuAdi 
                        : null));

            CreateMap<KioskMenuIslem, KioskMenuIslemResponseDto>()
                .ForMember(dest => dest.KioskMenuAdi, opt => opt.MapFrom(src => src.KioskMenu != null ? src.KioskMenu.MenuAdi : null))
                .ForMember(dest => dest.KanalAltAdi, opt => opt.MapFrom(src => src.KanalAlt != null ? src.KanalAlt.KanalAltAdi : null));

            CreateMap<KioskMenuAtama, KioskMenuAtamaResponseDto>()
                .ForMember(dest => dest.KioskAdi, opt => opt.MapFrom(src => src.Kiosk != null ? src.Kiosk.KioskAdi : null))
                .ForMember(dest => dest.HizmetBinasiAdi, opt => opt.MapFrom(src => src.Kiosk != null && src.Kiosk.DepartmanHizmetBinasi != null && src.Kiosk.DepartmanHizmetBinasi.HizmetBinasi != null ? src.Kiosk.DepartmanHizmetBinasi.HizmetBinasi.HizmetBinasiAdi : null))
                .ForMember(dest => dest.KioskMenuAdi, opt => opt.MapFrom(src => src.KioskMenu != null ? src.KioskMenu.MenuAdi : null));

            CreateMap<Kiosk, KioskSummaryDto>();

            // DTO -> Entity
            CreateMap<KioskMenuCreateRequestDto, KioskMenu>();
            CreateMap<KioskMenuUpdateRequestDto, KioskMenu>();
            CreateMap<KioskCreateRequestDto, Kiosk>();
            CreateMap<KioskUpdateRequestDto, Kiosk>();
            CreateMap<KioskMenuIslemCreateRequestDto, KioskMenuIslem>();
            CreateMap<KioskMenuIslemUpdateRequestDto, KioskMenuIslem>();
            CreateMap<KioskMenuAtamaCreateRequestDto, KioskMenuAtama>()
                .ForMember(dest => dest.Kiosk, opt => opt.Ignore())
                .ForMember(dest => dest.KioskMenu, opt => opt.Ignore())
                .ForMember(dest => dest.KioskMenuAtamaId, opt => opt.Ignore())
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.EkleyenKullanici, opt => opt.Ignore())
                .ForMember(dest => dest.DuzenleyenKullanici, opt => opt.Ignore())
                .ForMember(dest => dest.SilindiMi, opt => opt.Ignore())
                .ForMember(dest => dest.SilinmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.SilenKullanici, opt => opt.Ignore());
            
            CreateMap<KioskMenuAtamaUpdateRequestDto, KioskMenuAtama>()
                .ForMember(dest => dest.Kiosk, opt => opt.Ignore())
                .ForMember(dest => dest.KioskMenu, opt => opt.Ignore())
                .ForMember(dest => dest.AtamaTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.EkleyenKullanici, opt => opt.Ignore())
                .ForMember(dest => dest.DuzenleyenKullanici, opt => opt.Ignore())
                .ForMember(dest => dest.SilindiMi, opt => opt.Ignore())
                .ForMember(dest => dest.SilinmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.SilenKullanici, opt => opt.Ignore());
        }
    }
}
