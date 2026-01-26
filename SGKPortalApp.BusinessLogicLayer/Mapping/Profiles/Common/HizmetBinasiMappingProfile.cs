using AutoMapper;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Mapping.Profiles.Common
{
    public class HizmetBinasiMappingProfile : Profile
    {
        public HizmetBinasiMappingProfile()
        {
            // ═══════════════════════════════════════════════════════
            // HİZMET BİNASI - BASİT MAPPING (Entity -> Response DTO)
            // ═══════════════════════════════════════════════════════

            CreateMap<HizmetBinasi, HizmetBinasiResponseDto>()
                .ForMember(dest => dest.Departmanlar,
                    opt => opt.MapFrom(src => src.DepartmanHizmetBinalari != null
                        ? src.DepartmanHizmetBinalari
                            .Where(dhb => !dhb.SilindiMi)
                            .Select(dhb => new DepartmanBinaDto
                            {
                                DepartmanId = dhb.DepartmanId,
                                DepartmanAdi = dhb.Departman != null ? dhb.Departman.DepartmanAdi : string.Empty,
                                AnaBina = dhb.AnaBina
                            }).ToList()
                        : new List<DepartmanBinaDto>()))
                .ForMember(dest => dest.PersonelSayisi,
                    opt => opt.MapFrom(src => src.DepartmanHizmetBinalari != null
                        ? src.DepartmanHizmetBinalari.SelectMany(dhb => dhb.Personeller ?? new List<Personel>()).Count(p => !p.SilindiMi && p.PersonelAktiflikDurum == PersonelAktiflikDurum.Aktif)
                        : 0))
                .ForMember(dest => dest.BankoSayisi,
                    opt => opt.MapFrom(src => src.DepartmanHizmetBinalari != null
                        ? src.DepartmanHizmetBinalari.SelectMany(dhb => dhb.Bankolar ?? new List<Banko>()).Count(b => !b.SilindiMi && b.Aktiflik == Aktiflik.Aktif)
                        : 0))
                .ForMember(dest => dest.TvSayisi,
                    opt => opt.MapFrom(src => src.DepartmanHizmetBinalari != null
                        ? src.DepartmanHizmetBinalari.SelectMany(dhb => dhb.Tvler ?? new List<Tv>()).Count(t => !t.SilindiMi && t.Aktiflik == Aktiflik.Aktif)
                        : 0));

            // ═══════════════════════════════════════════════════════
            // HİZMET BİNASI - DETAYLI MAPPING (Entity -> Detail Response DTO)
            // ═══════════════════════════════════════════════════════

            CreateMap<HizmetBinasi, HizmetBinasiDetailResponseDto>()
                // Temel bilgiler
                .ForMember(dest => dest.HizmetBinasiId,
                    opt => opt.MapFrom(src => src.HizmetBinasiId))
                .ForMember(dest => dest.HizmetBinasiAdi,
                    opt => opt.MapFrom(src => src.HizmetBinasiAdi))
                .ForMember(dest => dest.LegacyKod,
                    opt => opt.MapFrom(src => src.LegacyKod))
                .ForMember(dest => dest.Adres,
                    opt => opt.MapFrom(src => src.Adres))
                .ForMember(dest => dest.Aktiflik,
                    opt => opt.MapFrom(src => src.Aktiflik))
                .ForMember(dest => dest.Departmanlar,
                    opt => opt.MapFrom(src => src.DepartmanHizmetBinalari != null
                        ? src.DepartmanHizmetBinalari
                            .Where(dhb => !dhb.SilindiMi)
                            .Select(dhb => new DepartmanBinaDto
                            {
                                DepartmanId = dhb.DepartmanId,
                                DepartmanAdi = dhb.Departman != null ? dhb.Departman.DepartmanAdi : string.Empty,
                                AnaBina = dhb.AnaBina
                            }).ToList()
                        : new List<DepartmanBinaDto>()))
                .ForMember(dest => dest.EklenmeTarihi,
                    opt => opt.MapFrom(src => src.EklenmeTarihi))
                .ForMember(dest => dest.DuzenlenmeTarihi,
                    opt => opt.MapFrom(src => src.DuzenlenmeTarihi))

                // İstatistikler
                .ForMember(dest => dest.ToplamPersonelSayisi,
                    opt => opt.MapFrom(src => src.DepartmanHizmetBinalari != null
                        ? src.DepartmanHizmetBinalari.SelectMany(dhb => dhb.Personeller ?? new List<Personel>()).Count()
                        : 0))
                .ForMember(dest => dest.AktifPersonelSayisi,
                    opt => opt.MapFrom(src => src.DepartmanHizmetBinalari != null
                        ? src.DepartmanHizmetBinalari.SelectMany(dhb => dhb.Personeller ?? new List<Personel>()).Count(p => p.PersonelAktiflikDurum == PersonelAktiflikDurum.Aktif)
                        : 0)) // 
                .ForMember(dest => dest.ToplamBankoSayisi,
                    opt => opt.MapFrom(src => src.DepartmanHizmetBinalari != null
                        ? src.DepartmanHizmetBinalari.SelectMany(dhb => dhb.Bankolar ?? new List<Banko>()).Count()
                        : 0))
                .ForMember(dest => dest.AktifBankoSayisi,
                    opt => opt.MapFrom(src => src.DepartmanHizmetBinalari != null
                        ? src.DepartmanHizmetBinalari.SelectMany(dhb => dhb.Bankolar ?? new List<Banko>()).Count(b => b.Aktiflik == Aktiflik.Aktif)
                        : 0))
                .ForMember(dest => dest.ToplamTvSayisi,
                    opt => opt.MapFrom(src => src.DepartmanHizmetBinalari != null
                        ? src.DepartmanHizmetBinalari.SelectMany(dhb => dhb.Tvler ?? new List<Tv>()).Count()
                        : 0))

                // İlişkili veriler
                .ForMember(dest => dest.Personeller,
                    opt => opt.MapFrom(src => src.DepartmanHizmetBinalari != null
                        ? src.DepartmanHizmetBinalari.SelectMany(dhb => dhb.Personeller ?? new List<Personel>()).ToList()
                        : new List<Personel>()))
                .ForMember(dest => dest.Bankolar,
                    opt => opt.MapFrom(src => src.DepartmanHizmetBinalari != null
                        ? src.DepartmanHizmetBinalari.SelectMany(dhb => dhb.Bankolar ?? new List<Banko>()).ToList()
                        : new List<Banko>()))
                .ForMember(dest => dest.Tvler,
                    opt => opt.MapFrom(src => src.DepartmanHizmetBinalari != null
                        ? src.DepartmanHizmetBinalari.SelectMany(dhb => dhb.Tvler ?? new List<Tv>()).ToList()
                        : new List<Tv>()))

                // Personel durum istatistikleri
                .ForMember(dest => dest.PersonelDurumIstatistikleri,
                    opt => opt.MapFrom(src =>
                        src.DepartmanHizmetBinalari != null
                            ? src.DepartmanHizmetBinalari
                                .SelectMany(dhb => dhb.Personeller ?? new List<Personel>())
                                .GroupBy(p => p.PersonelAktiflikDurum)
                                .ToDictionary(g => g.Key, g => g.Count())
                            : new Dictionary<PersonelAktiflikDurum, int>()));

            // ═══════════════════════════════════════════════════════
            // BANKO MAPPING (Detail DTO için gerekli)
            // ═══════════════════════════════════════════════════════
            // NOT: Banko -> BankoResponseDto mapping'i BankoMappingProfile'da tanımlı

            // ═══════════════════════════════════════════════════════
            // TV MAPPING (Detail DTO için gerekli)
            // ═══════════════════════════════════════════════════════

            CreateMap<Tv, TvResponseDto>()
                .ForMember(dest => dest.TvId,
                    opt => opt.MapFrom(src => src.TvId))
                .ForMember(dest => dest.TvAdi,
                    opt => opt.MapFrom(src => src.TvAdi))
                .ForMember(dest => dest.TvAciklama,
                    opt => opt.MapFrom(src => src.TvAciklama))
                .ForMember(dest => dest.Aktiflik,
                    opt => opt.MapFrom(src => src.Aktiflik))
                .ForMember(dest => dest.HizmetBinasiId,
                    opt => opt.MapFrom(src => src.DepartmanHizmetBinasi != null ? src.DepartmanHizmetBinasi.HizmetBinasiId : 0))
                .ForMember(dest => dest.HizmetBinasiAdi,
                    opt => opt.MapFrom(src => src.DepartmanHizmetBinasi != null && src.DepartmanHizmetBinasi.HizmetBinasi != null ? src.DepartmanHizmetBinasi.HizmetBinasi.HizmetBinasiAdi : null))
                .ForMember(dest => dest.BankoSayisi,
                    opt => opt.MapFrom(src => src.TvBankolar != null
                        ? src.TvBankolar.Count(tb => tb.Aktiflik == Aktiflik.Aktif)
                        : 0))
                .ForMember(dest => dest.EklenmeTarihi,
                    opt => opt.MapFrom(src => src.EklenmeTarihi))
                .ForMember(dest => dest.DuzenlenmeTarihi,
                    opt => opt.MapFrom(src => src.DuzenlenmeTarihi));

            // ═══════════════════════════════════════════════════════
            // PERSONEL MAPPING - KALDIRILDI
            // ═══════════════════════════════════════════════════════
            // NOT: Personel -> PersonelResponseDto mapping'i PersonelMappingProfile.cs'de tanımlı
            // REQUEST DTO -> ENTITY (Create)
            // ═══════════════════════════════════════════════════════

            CreateMap<HizmetBinasiCreateRequestDto, HizmetBinasi>()
                .ForMember(dest => dest.HizmetBinasiId, opt => opt.Ignore())
                .ForMember(dest => dest.LegacyKod, opt => opt.Ignore())
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.DepartmanHizmetBinalari, opt => opt.Ignore())
                .ForMember(dest => dest.Devices, opt => opt.Ignore())
                .ForMember(dest => dest.SpecialCards, opt => opt.Ignore())
                .ForMember(dest => dest.SilindiMi, opt => opt.Ignore())
                .ForMember(dest => dest.SilinmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.SilenKullanici, opt => opt.Ignore());

            // ═══════════════════════════════════════════════════════
            // REQUEST DTO -> ENTITY (Update)
            // ═══════════════════════════════════════════════════════

            CreateMap<HizmetBinasiUpdateRequestDto, HizmetBinasi>()
                .ForMember(dest => dest.HizmetBinasiId, opt => opt.Ignore())
                .ForMember(dest => dest.LegacyKod, opt => opt.Ignore())
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.DepartmanHizmetBinalari, opt => opt.Ignore())
                .ForMember(dest => dest.Devices, opt => opt.Ignore())
                .ForMember(dest => dest.SpecialCards, opt => opt.Ignore())
                .ForMember(dest => dest.SilindiMi, opt => opt.Ignore())
                .ForMember(dest => dest.SilinmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.SilenKullanici, opt => opt.Ignore());

            // ═══════════════════════════════════════════════════════
            // REVERSE MAPPING (Response DTO -> Entity, gerekirse)
            // ═══════════════════════════════════════════════════════

            CreateMap<HizmetBinasiResponseDto, HizmetBinasi>()
                .ForMember(dest => dest.DepartmanHizmetBinalari, opt => opt.Ignore())
                .ForMember(dest => dest.Devices, opt => opt.Ignore())
                .ForMember(dest => dest.SpecialCards, opt => opt.Ignore())
                .ForMember(dest => dest.SilindiMi, opt => opt.Ignore())
                .ForMember(dest => dest.SilinmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.SilenKullanici, opt => opt.Ignore());

            // ═══════════════════════════════════════════════════════
            // REVERSE MAPPING (Entity -> Update Request DTO)
            // Field permission kontrolü için gerekli
            // ═══════════════════════════════════════════════════════

            CreateMap<HizmetBinasi, HizmetBinasiUpdateRequestDto>();
        }
    }
}