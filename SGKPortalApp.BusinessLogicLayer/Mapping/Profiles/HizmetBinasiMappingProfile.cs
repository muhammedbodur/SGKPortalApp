using AutoMapper;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Mapping.Profiles
{
    public class HizmetBinasiMappingProfile : Profile
    {
        public HizmetBinasiMappingProfile()
        {
            // ═══════════════════════════════════════════════════════
            // HİZMET BİNASI - BASİT MAPPING (Entity -> Response DTO)
            // ═══════════════════════════════════════════════════════

            CreateMap<HizmetBinasi, HizmetBinasiResponseDto>()
                .ForMember(dest => dest.DepartmanAdi,
                    opt => opt.MapFrom(src => src.Departman != null ? src.Departman.DepartmanAdi : string.Empty))
                .ForMember(dest => dest.PersonelSayisi,
                    opt => opt.MapFrom(src => src.Personeller != null ? src.Personeller.Count : 0))
                .ForMember(dest => dest.BankoSayisi,
                    opt => opt.MapFrom(src => src.Bankolar != null ? src.Bankolar.Count : 0))
                .ForMember(dest => dest.TvSayisi,
                    opt => opt.MapFrom(src => src.Tvler != null ? src.Tvler.Count : 0));

            // ═══════════════════════════════════════════════════════
            // HİZMET BİNASI - DETAYLI MAPPING (Entity -> Detail Response DTO)
            // ═══════════════════════════════════════════════════════

            CreateMap<HizmetBinasi, HizmetBinasiDetailResponseDto>()
                // Temel bilgiler
                .ForMember(dest => dest.HizmetBinasiId,
                    opt => opt.MapFrom(src => src.HizmetBinasiId))
                .ForMember(dest => dest.HizmetBinasiAdi,
                    opt => opt.MapFrom(src => src.HizmetBinasiAdi))
                .ForMember(dest => dest.HizmetBinasiAktiflik,
                    opt => opt.MapFrom(src => src.HizmetBinasiAktiflik))
                .ForMember(dest => dest.DepartmanAdi,
                    opt => opt.MapFrom(src => src.Departman != null ? src.Departman.DepartmanAdi : string.Empty))
                .ForMember(dest => dest.EklenmeTarihi,
                    opt => opt.MapFrom(src => src.EklenmeTarihi))
                .ForMember(dest => dest.DuzenlenmeTarihi,
                    opt => opt.MapFrom(src => src.DuzenlenmeTarihi))

                // İstatistikler
                .ForMember(dest => dest.ToplamPersonelSayisi,
                    opt => opt.MapFrom(src => src.Personeller != null ? src.Personeller.Count : 0))
                .ForMember(dest => dest.ToplamBankoSayisi,
                    opt => opt.MapFrom(src => src.Bankolar != null ? src.Bankolar.Count : 0))
                .ForMember(dest => dest.ToplamTvSayisi,
                    opt => opt.MapFrom(src => src.Tvler != null ? src.Tvler.Count : 0))

                // İlişkili veriler
                .ForMember(dest => dest.Personeller,
                    opt => opt.MapFrom(src => src.Personeller ?? new List<Personel>()))
                .ForMember(dest => dest.Bankolar,
                    opt => opt.MapFrom(src => src.Bankolar ?? new List<Banko>()))
                .ForMember(dest => dest.Tvler,
                    opt => opt.MapFrom(src => src.Tvler ?? new List<Tv>()))

                // Personel durum istatistikleri
                .ForMember(dest => dest.PersonelDurumIstatistikleri,
                    opt => opt.MapFrom(src =>
                        src.Personeller != null
                            ? src.Personeller
                                .GroupBy(p => p.PersonelAktiflikDurum)
                                .ToDictionary(g => g.Key, g => g.Count())
                            : new Dictionary<PersonelAktiflikDurum, int>()));

            // ═══════════════════════════════════════════════════════
            // BANKO MAPPING (Detail DTO için gerekli)
            // ═══════════════════════════════════════════════════════

            CreateMap<Banko, BankoResponseDto>()
                .ForMember(dest => dest.BankoId,
                    opt => opt.MapFrom(src => src.BankoId))
                .ForMember(dest => dest.BankoNo,
                    opt => opt.MapFrom(src => src.BankoNo))
                .ForMember(dest => dest.BankoTipi,
                    opt => opt.MapFrom(src => src.BankoTipi))
                .ForMember(dest => dest.KatTipi,
                    opt => opt.MapFrom(src => src.KatTipi))
                .ForMember(dest => dest.BankoAktiflik,
                    opt => opt.MapFrom(src => src.BankoAktiflik))
                .ForMember(dest => dest.HizmetBinasiId,
                    opt => opt.MapFrom(src => src.HizmetBinasiId))
                .ForMember(dest => dest.HizmetBinasiAdi,
                    opt => opt.MapFrom(src => src.HizmetBinasi != null ? src.HizmetBinasi.HizmetBinasiAdi : null))
                .ForMember(dest => dest.TcKimlikNo,
                    opt => opt.MapFrom(src => src.BankoKullanicilari != null && src.BankoKullanicilari.Any()
                        ? src.BankoKullanicilari.FirstOrDefault()!.TcKimlikNo
                        : null))
                .ForMember(dest => dest.PersonelAdSoyad,
                    opt => opt.MapFrom(src => src.BankoKullanicilari != null && src.BankoKullanicilari.Any()
                        ? src.BankoKullanicilari.FirstOrDefault()!.Personel.AdSoyad
                        : null))
                .ForMember(dest => dest.PersonelResim,
                    opt => opt.MapFrom(src => src.BankoKullanicilari != null && src.BankoKullanicilari.Any()
                        ? src.BankoKullanicilari.FirstOrDefault()!.Personel.Resim
                        : null))
                .ForMember(dest => dest.BekleyenSiraSayisi,
                    opt => opt.MapFrom(src => 0)) // TODO: Sıra sisteminden gelecek
                .ForMember(dest => dest.TamamlananSiraSayisi,
                    opt => opt.MapFrom(src => 0)) // TODO: Sıra sisteminden gelecek
                .ForMember(dest => dest.EklenmeTarihi,
                    opt => opt.MapFrom(src => src.EklenmeTarihi))
                .ForMember(dest => dest.DuzenlenmeTarihi,
                    opt => opt.MapFrom(src => src.DuzenlenmeTarihi));

            // ═══════════════════════════════════════════════════════
            // TV MAPPING (Detail DTO için gerekli)
            // ═══════════════════════════════════════════════════════

            CreateMap<Tv, TvResponseDto>()
                .ForMember(dest => dest.TvId,
                    opt => opt.MapFrom(src => src.TvId))
                .ForMember(dest => dest.TvAdi,
                    opt => opt.MapFrom(src => src.TvAdi))
                .ForMember(dest => dest.Aciklama,
                    opt => opt.MapFrom(src => src.Aciklama))
                .ForMember(dest => dest.TvAktiflik,
                    opt => opt.MapFrom(src => src.TvAktiflik))
                .ForMember(dest => dest.HizmetBinasiId,
                    opt => opt.MapFrom(src => src.HizmetBinasiId))
                .ForMember(dest => dest.HizmetBinasiAdi,
                    opt => opt.MapFrom(src => src.HizmetBinasi != null ? src.HizmetBinasi.HizmetBinasiAdi : null))
                .ForMember(dest => dest.BankoSayisi,
                    opt => opt.MapFrom(src => src.TvBankolar != null ? src.TvBankolar.Count : 0))
                .ForMember(dest => dest.EklenmeTarihi,
                    opt => opt.MapFrom(src => src.EklenmeTarihi))
                .ForMember(dest => dest.DuzenlenmeTarihi,
                    opt => opt.MapFrom(src => src.DuzenlenmeTarihi));

            // ═══════════════════════════════════════════════════════
            // PERSONEL MAPPING (Detail DTO için gerekli)
            // ═══════════════════════════════════════════════════════

            CreateMap<Personel, PersonelResponseDto>()
                .ForMember(dest => dest.TcKimlikNo,
                    opt => opt.MapFrom(src => src.TcKimlikNo))
                .ForMember(dest => dest.SicilNo,
                    opt => opt.MapFrom(src => src.SicilNo))
                .ForMember(dest => dest.AdSoyad,
                    opt => opt.MapFrom(src => src.AdSoyad))
                .ForMember(dest => dest.NickName,
                    opt => opt.MapFrom(src => src.NickName))
                .ForMember(dest => dest.Email,
                    opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.DepartmanId,
                    opt => opt.MapFrom(src => src.DepartmanId))
                .ForMember(dest => dest.DepartmanAdi,
                    opt => opt.MapFrom(src => src.Departman != null ? src.Departman.DepartmanAdi : string.Empty))
                .ForMember(dest => dest.ServisId,
                    opt => opt.MapFrom(src => src.ServisId))
                .ForMember(dest => dest.ServisAdi,
                    opt => opt.MapFrom(src => src.Servis != null ? src.Servis.ServisAdi : string.Empty))
                .ForMember(dest => dest.UnvanId,
                    opt => opt.MapFrom(src => src.UnvanId))
                .ForMember(dest => dest.UnvanAdi,
                    opt => opt.MapFrom(src => src.Unvan != null ? src.Unvan.UnvanAdi : string.Empty))
                .ForMember(dest => dest.Dahili,
                    opt => opt.MapFrom(src => src.Dahili))
                .ForMember(dest => dest.CepTelefonu,
                    opt => opt.MapFrom(src => src.CepTelefonu))
                .ForMember(dest => dest.CepTelefonu2,
                    opt => opt.MapFrom(src => src.CepTelefonu2))
                .ForMember(dest => dest.EvTelefonu,
                    opt => opt.MapFrom(src => src.EvTelefonu))
                .ForMember(dest => dest.Adres,
                    opt => opt.MapFrom(src => src.Adres))
                .ForMember(dest => dest.Semt,
                    opt => opt.MapFrom(src => src.Semt))
                .ForMember(dest => dest.DogumTarihi,
                    opt => opt.MapFrom(src => src.DogumTarihi))
                .ForMember(dest => dest.Cinsiyet,
                    opt => opt.MapFrom(src => src.Cinsiyet))
                .ForMember(dest => dest.MedeniDurumu,
                    opt => opt.MapFrom(src => src.MedeniDurumu))
                .ForMember(dest => dest.KanGrubu,
                    opt => opt.MapFrom(src => src.KanGrubu))
                .ForMember(dest => dest.PersonelTipi,
                    opt => opt.MapFrom(src => src.PersonelTipi))
                .ForMember(dest => dest.PersonelAktiflikDurum,
                    opt => opt.MapFrom(src => src.PersonelAktiflikDurum))
                .ForMember(dest => dest.OgrenimDurumu,
                    opt => opt.MapFrom(src => src.OgrenimDurumu))
                .ForMember(dest => dest.BitirdigiOkul,
                    opt => opt.MapFrom(src => src.BitirdigiOkul))
                .ForMember(dest => dest.BitirdigiBolum,
                    opt => opt.MapFrom(src => src.BitirdigiBolum))
                .ForMember(dest => dest.Resim,
                    opt => opt.MapFrom(src => src.Resim))
                .ForMember(dest => dest.EklenmeTarihi,
                    opt => opt.MapFrom(src => src.EklenmeTarihi))
                .ForMember(dest => dest.DuzenlenmeTarihi,
                    opt => opt.MapFrom(src => src.DuzenlenmeTarihi));

            // ═══════════════════════════════════════════════════════
            // REQUEST DTO -> ENTITY (Create)
            // ═══════════════════════════════════════════════════════

            CreateMap<HizmetBinasiCreateRequestDto, HizmetBinasi>()
                .ForMember(dest => dest.HizmetBinasiId, opt => opt.Ignore())
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.Departman, opt => opt.Ignore())
                .ForMember(dest => dest.Personeller, opt => opt.Ignore())
                .ForMember(dest => dest.Bankolar, opt => opt.Ignore())
                .ForMember(dest => dest.Tvler, opt => opt.Ignore())
                .ForMember(dest => dest.SilindiMi, opt => opt.Ignore())
                .ForMember(dest => dest.SilinmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.SilenKullanici, opt => opt.Ignore());

            // ═══════════════════════════════════════════════════════
            // REQUEST DTO -> ENTITY (Update)
            // ═══════════════════════════════════════════════════════

            CreateMap<HizmetBinasiUpdateRequestDto, HizmetBinasi>()
                .ForMember(dest => dest.HizmetBinasiId, opt => opt.Ignore())
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.Departman, opt => opt.Ignore())
                .ForMember(dest => dest.Personeller, opt => opt.Ignore())
                .ForMember(dest => dest.Bankolar, opt => opt.Ignore())
                .ForMember(dest => dest.Tvler, opt => opt.Ignore())
                .ForMember(dest => dest.SilindiMi, opt => opt.Ignore())
                .ForMember(dest => dest.SilinmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.SilenKullanici, opt => opt.Ignore());

            // ═══════════════════════════════════════════════════════
            // REVERSE MAPPING (Response DTO -> Entity, gerekirse)
            // ═══════════════════════════════════════════════════════

            CreateMap<HizmetBinasiResponseDto, HizmetBinasi>()
                .ForMember(dest => dest.Departman, opt => opt.Ignore())
                .ForMember(dest => dest.Personeller, opt => opt.Ignore())
                .ForMember(dest => dest.Bankolar, opt => opt.Ignore())
                .ForMember(dest => dest.Tvler, opt => opt.Ignore())
                .ForMember(dest => dest.SilindiMi, opt => opt.Ignore())
                .ForMember(dest => dest.SilinmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.SilenKullanici, opt => opt.Ignore());
        }
    }
}