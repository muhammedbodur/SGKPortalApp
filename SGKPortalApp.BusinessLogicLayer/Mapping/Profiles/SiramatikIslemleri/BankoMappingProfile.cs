using AutoMapper;
using SGKPortalApp;
using SGKPortalApp.BusinessLogicLayer.Mapping.ValueConverters;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.Common.Extensions;
using SGKPortalApp.Common.Helpers;

namespace SGKPortalApp.BusinessLogicLayer.Mapping.Profiles.SiramatikIslemleri
{
    public class BankoMappingProfile : Profile
    {
        public BankoMappingProfile()
        {
            // ═══════════════════════════════════════════════════════
            // REQUEST -> ENTITY
            // ═══════════════════════════════════════════════════════

            CreateMap<BankoCreateRequestDto, Banko>()
                .ForMember(dest => dest.BankoId, opt => opt.Ignore())
                .ForMember(dest => dest.Aktiflik, opt => opt.MapFrom(src => Aktiflik.Aktif))
                .ForMember(dest => dest.BankoSira, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.MapFrom(src => DateTimeHelper.Now))
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.MapFrom(src => DateTimeHelper.Now))
                .ForMember(dest => dest.SilindiMi, opt => opt.Ignore())
                .ForMember(dest => dest.DepartmanHizmetBinasi, opt => opt.Ignore())
                .ForMember(dest => dest.BankoKullanicilari, opt => opt.Ignore())
                .ForMember(dest => dest.TvBankolar, opt => opt.Ignore())
                .ForMember(dest => dest.BankoHareketleri, opt => opt.Ignore());

            CreateMap<BankoUpdateRequestDto, Banko>()
                .ForMember(dest => dest.BankoId, opt => opt.Ignore())
                .ForMember(dest => dest.DepartmanHizmetBinasiId, opt => opt.Ignore())
                .ForMember(dest => dest.BankoNo, opt => opt.Ignore())
                .ForMember(dest => dest.KatTipi, opt => opt.Ignore())
                .ForMember(dest => dest.BankoSira, opt => opt.Ignore())
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.MapFrom(src => DateTimeHelper.Now))
                .ForMember(dest => dest.SilindiMi, opt => opt.Ignore())
                .ForMember(dest => dest.DepartmanHizmetBinasi, opt => opt.Ignore())
                .ForMember(dest => dest.BankoKullanicilari, opt => opt.Ignore())
                .ForMember(dest => dest.TvBankolar, opt => opt.Ignore())
                .ForMember(dest => dest.BankoHareketleri, opt => opt.Ignore());

            // ═══════════════════════════════════════════════════════
            // ENTITY -> RESPONSE
            // ═══════════════════════════════════════════════════════

            CreateMap<Banko, BankoResponseDto>()
                .ForMember(dest => dest.BankoTipiAdi, 
                    opt => opt.MapFrom(src => src.BankoTipi.GetDisplayName()))
                .ForMember(dest => dest.KatTipiAdi, 
                    opt => opt.MapFrom(src => src.KatTipi.GetDisplayName()))
                .ForMember(dest => dest.DepartmanId,
                    opt => opt.MapFrom(src => src.DepartmanHizmetBinasi != null ? src.DepartmanHizmetBinasi.DepartmanId : (int?)null))
                .ForMember(dest => dest.DepartmanAdi,
                    opt => opt.MapFrom(src => src.DepartmanHizmetBinasi != null && src.DepartmanHizmetBinasi.Departman != null ? src.DepartmanHizmetBinasi.Departman.DepartmanAdi : null))
                .ForMember(dest => dest.HizmetBinasiAdi, 
                    opt => opt.MapFrom(src => src.DepartmanHizmetBinasi != null && src.DepartmanHizmetBinasi.HizmetBinasi != null ? src.DepartmanHizmetBinasi.HizmetBinasi.HizmetBinasiAdi : null))
                .ForMember(dest => dest.AtananPersonel, 
                    opt => opt.MapFrom(src => src.BankoKullanicilari != null && src.BankoKullanicilari.Any() 
                        ? src.BankoKullanicilari.First() 
                        : null))
                .ForMember(dest => dest.BankoMusaitMi, 
                    opt => opt.MapFrom(src => src.BankoKullanicilari == null || !src.BankoKullanicilari.Any()));

            // ═══════════════════════════════════════════════════════
            // BANKO KULLANICI -> PERSONEL ATAMA DTO
            // ═══════════════════════════════════════════════════════

            CreateMap<BankoKullanici, PersonelAtamaDto>()
                .ForMember(dest => dest.TcKimlikNo, 
                    opt => opt.MapFrom(src => src.TcKimlikNo))
                .ForMember(dest => dest.AdSoyad, 
                    opt => opt.MapFrom(src => src.Personel != null ? src.Personel.AdSoyad : string.Empty))
                .ForMember(dest => dest.ServisAdi, 
                    opt => opt.MapFrom(src => src.Personel != null && src.Personel.Servis != null 
                        ? src.Personel.Servis.ServisAdi 
                        : string.Empty))
                .ForMember(dest => dest.Resim,
                    opt => opt.ConvertUsing<ImagePathConverter, string?>(src => src.Personel != null ? src.Personel.Resim : null))
                .ForMember(dest => dest.AtanmaTarihi, 
                    opt => opt.MapFrom(src => src.EklenmeTarihi));
        }
    }
}
