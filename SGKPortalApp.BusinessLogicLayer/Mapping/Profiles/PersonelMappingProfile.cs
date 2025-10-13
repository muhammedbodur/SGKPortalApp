using AutoMapper;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Mapping.Profiles
{
    public class PersonelMappingProfile : Profile
    {
        public PersonelMappingProfile()
        {
            // ═══════════════════════════════════════════════════════
            // DEPARTMAN MAPPING'LERİ
            // ═══════════════════════════════════════════════════════

            CreateMap<Departman, DepartmanResponseDto>()
                .ForMember(dest => dest.PersonelSayisi,
                    opt => opt.MapFrom(src => src.Personeller != null ? src.Personeller.Count : 0));

            CreateMap<DepartmanCreateRequestDto, Departman>()
                .ForMember(dest => dest.DepartmanId, opt => opt.Ignore())
                .ForMember(dest => dest.DepartmanAktiflik,
                    opt => opt.MapFrom(src => BusinessObjectLayer.Enums.Common.Aktiflik.Aktif))
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Personeller, opt => opt.Ignore());

            CreateMap<DepartmanUpdateRequestDto, Departman>()
                .ForMember(dest => dest.DepartmanId, opt => opt.Ignore())
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Personeller, opt => opt.Ignore());

            CreateMap<DepartmanResponseDto, DepartmanCreateRequestDto>();
            CreateMap<DepartmanResponseDto, DepartmanUpdateRequestDto>();


            // ═══════════════════════════════════════════════════════
            // SERVİS MAPPING'LERİ
            // ═══════════════════════════════════════════════════════

            CreateMap<Servis, ServisResponseDto>()
                .ForMember(dest => dest.PersonelSayisi,
                    opt => opt.MapFrom(src => src.Personeller != null ? src.Personeller.Count : 0));

            CreateMap<ServisCreateRequestDto, Servis>()
                .ForMember(dest => dest.ServisId, opt => opt.Ignore())
                .ForMember(dest => dest.ServisAktiflik,
                    opt => opt.MapFrom(src => BusinessObjectLayer.Enums.Common.Aktiflik.Aktif))
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Personeller, opt => opt.Ignore());

            CreateMap<ServisUpdateRequestDto, Servis>()
                .ForMember(dest => dest.ServisId, opt => opt.Ignore())
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Personeller, opt => opt.Ignore());


            // ═══════════════════════════════════════════════════════
            // UNVAN MAPPING'LERİ
            // ═══════════════════════════════════════════════════════

            CreateMap<Unvan, UnvanResponseDto>()
                .ForMember(dest => dest.PersonelSayisi,
                    opt => opt.MapFrom(src => src.Personeller != null ? src.Personeller.Count : 0));

            CreateMap<UnvanCreateRequestDto, Unvan>()
                .ForMember(dest => dest.UnvanId, opt => opt.Ignore())
                .ForMember(dest => dest.UnvanAktiflik,
                    opt => opt.MapFrom(src => BusinessObjectLayer.Enums.Common.Aktiflik.Aktif))
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Personeller, opt => opt.Ignore());

            CreateMap<UnvanUpdateRequestDto, Unvan>()
                .ForMember(dest => dest.UnvanId, opt => opt.Ignore())
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Personeller, opt => opt.Ignore());


            // ═══════════════════════════════════════════════════════
            // ⭐ PERSONEL MAPPING'LERİ - OTOMATİK + ÖZEL DURUMLAR
            // ═══════════════════════════════════════════════════════

            //  Entity → Response DTO
            CreateMap<Personel, PersonelResponseDto>()
                // Navigation property'lerden veri çekme
                .ForMember(dest => dest.DepartmanAdi,
                    opt => opt.MapFrom(src => src.Departman != null ? src.Departman.DepartmanAdi : ""))
                .ForMember(dest => dest.ServisAdi,
                    opt => opt.MapFrom(src => src.Servis != null ? src.Servis.ServisAdi : ""))
                .ForMember(dest => dest.UnvanAdi,
                    opt => opt.MapFrom(src => src.Unvan != null ? src.Unvan.UnvanAdi : ""))
                .ForMember(dest => dest.AtanmaNedeniAdi,
                    opt => opt.MapFrom(src => src.AtanmaNedeni != null ? src.AtanmaNedeni.AtanmaNedeni : ""))
                .ForMember(dest => dest.HizmetBinasiAdi,
                    opt => opt.MapFrom(src => src.HizmetBinasi != null ? src.HizmetBinasi.HizmetBinasiAdi : ""))
                .ForMember(dest => dest.SendikaAdi,
                opt => opt.MapFrom(src => src.Sendika != null ? src.Sendika.SendikaAdi : ""))

                // İl/İlçe bilgileri
                .ForMember(dest => dest.IlAdi,
                    opt => opt.MapFrom(src => src.Il != null ? src.Il.IlAdi : ""))
                .ForMember(dest => dest.IlceAdi,
                    opt => opt.MapFrom(src => src.Ilce != null ? src.Ilce.IlceAdi : ""))

                // Eşinin İş Yeri İl/İlçe
                .ForMember(dest => dest.EsininIsIlAdi,
                    opt => opt.MapFrom(src => src.EsininIsIl != null ? src.EsininIsIl.IlAdi : null))
                .ForMember(dest => dest.EsininIsIlceAdi,
                    opt => opt.MapFrom(src => src.EsininIsIlce != null ? src.EsininIsIlce.IlceAdi : null))


                // Navigation Collections
                .ForMember(dest => dest.Cocuklar,
                    opt => opt.MapFrom(src => src.PersonelCocuklari))
                .ForMember(dest => dest.Hizmetler,
                    opt => opt.MapFrom(src => src.PersonelHizmetleri))
                .ForMember(dest => dest.Egitimler,
                    opt => opt.MapFrom(src => src.PersonelEgitimleri))
                .ForMember(dest => dest.ImzaYetkileriDetay,
                    opt => opt.MapFrom(src => src.PersonelImzaYetkileri))
                .ForMember(dest => dest.Cezalar,
                    opt => opt.MapFrom(src => src.PersonelCezalari))
                .ForMember(dest => dest.Engeller,
                    opt => opt.MapFrom(src => src.PersonelEngelleri))

                // Nullable int → int çevirme
                .ForMember(dest => dest.SendikaId,
                    opt => opt.MapFrom(src => src.SendikaId.HasValue ? src.SendikaId.Value : 0));


            //  Create Request DTO → Entity
            CreateMap<PersonelCreateRequestDto, Personel>()
                // Nullable int handling
                .ForMember(dest => dest.SendikaId, opt => opt.MapFrom(src =>
                    src.SendikaId.HasValue && src.SendikaId.Value > 0 ? src.SendikaId : (int?)null))
                .ForMember(dest => dest.EsininIsIlId, opt => opt.MapFrom(src =>
                    src.EsininIsIlId.HasValue && src.EsininIsIlId.Value > 0 ? src.EsininIsIlId : (int?)null))
                .ForMember(dest => dest.EsininIsIlceId, opt => opt.MapFrom(src =>
                    src.EsininIsIlceId.HasValue && src.EsininIsIlceId.Value > 0 ? src.EsininIsIlceId : (int?)null))

                // Audit
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.SilindiMi, opt => opt.MapFrom(src => false))

                // Otomatik oluşturulan alanlar
                .ForMember(dest => dest.PersonelKayitNo, opt => opt.Ignore())
                .ForMember(dest => dest.KartNo, opt => opt.Ignore())
                .ForMember(dest => dest.KartNoAktiflikTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.KartNoDuzenlenmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.KartNoGonderimTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.KartGonderimIslemBasari, opt => opt.Ignore())
                .ForMember(dest => dest.PassWord, opt => opt.Ignore())
                .ForMember(dest => dest.SessionID, opt => opt.Ignore())

                // Navigation Properties
                .ForMember(dest => dest.Departman, opt => opt.Ignore())
                .ForMember(dest => dest.Servis, opt => opt.Ignore())
                .ForMember(dest => dest.Unvan, opt => opt.Ignore())
                .ForMember(dest => dest.AtanmaNedeni, opt => opt.Ignore())
                .ForMember(dest => dest.HizmetBinasi, opt => opt.Ignore())
                .ForMember(dest => dest.Il, opt => opt.Ignore())
                .ForMember(dest => dest.Ilce, opt => opt.Ignore())
                .ForMember(dest => dest.Sendika, opt => opt.Ignore())
                .ForMember(dest => dest.EsininIsIl, opt => opt.Ignore())
                .ForMember(dest => dest.EsininIsIlce, opt => opt.Ignore())
                .ForMember(dest => dest.BankoKullanicilari, opt => opt.Ignore())
                .ForMember(dest => dest.KanalPersonelleri, opt => opt.Ignore())
                .ForMember(dest => dest.PersonelCocuklari, opt => opt.Ignore())
                .ForMember(dest => dest.PersonelYetkileri, opt => opt.Ignore())
                .ForMember(dest => dest.PersonelHizmetleri, opt => opt.Ignore())
                .ForMember(dest => dest.PersonelEgitimleri, opt => opt.Ignore())
                .ForMember(dest => dest.PersonelImzaYetkileri, opt => opt.Ignore())
                .ForMember(dest => dest.PersonelCezalari, opt => opt.Ignore())
                .ForMember(dest => dest.PersonelEngelleri, opt => opt.Ignore())
                .ForMember(dest => dest.HubConnection, opt => opt.Ignore());


            //  Update Request DTO → Entity
            CreateMap<PersonelUpdateRequestDto, Personel>()
                // ═══ KORUNMASI GEREKEN ALANLAR ═══
                .ForMember(dest => dest.TcKimlikNo, opt => opt.Ignore())
                .ForMember(dest => dest.SicilNo, opt => opt.Ignore())
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.PassWord, opt => opt.Ignore())
                .ForMember(dest => dest.SessionID, opt => opt.Ignore())
                .ForMember(dest => dest.PersonelKayitNo, opt => opt.Ignore())
                .ForMember(dest => dest.KartNo, opt => opt.Ignore())
                .ForMember(dest => dest.KartNoAktiflikTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.KartNoDuzenlenmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.KartNoGonderimTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.KartGonderimIslemBasari, opt => opt.Ignore())
                .ForMember(dest => dest.DogumTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.Cinsiyet, opt => opt.Ignore())
                .ForMember(dest => dest.KanGrubu, opt => opt.Ignore())
                .ForMember(dest => dest.PersonelTipi, opt => opt.Ignore())
                .ForMember(dest => dest.OgrenimDurumu, opt => opt.Ignore())
                .ForMember(dest => dest.BitirdigiOkul, opt => opt.Ignore())
                .ForMember(dest => dest.BitirdigiBolum, opt => opt.Ignore())
                .ForMember(dest => dest.OgrenimSuresi, opt => opt.Ignore())
                .ForMember(dest => dest.Bransi, opt => opt.Ignore())
                .ForMember(dest => dest.EmekliSicilNo, opt => opt.Ignore())
                .ForMember(dest => dest.SehitYakinligi, opt => opt.Ignore())
                .ForMember(dest => dest.EsininAdi, opt => opt.Ignore())
                .ForMember(dest => dest.EsininIsDurumu, opt => opt.Ignore())
                .ForMember(dest => dest.EsininUnvani, opt => opt.Ignore())
                .ForMember(dest => dest.EsininIsAdresi, opt => opt.Ignore())
                .ForMember(dest => dest.EsininIsSemt, opt => opt.Ignore())
                .ForMember(dest => dest.EsininIsIlId, opt => opt.Ignore())
                .ForMember(dest => dest.EsininIsIlceId, opt => opt.Ignore())
                .ForMember(dest => dest.EvDurumu, opt => opt.Ignore())
                .ForMember(dest => dest.UlasimServis1, opt => opt.Ignore())
                .ForMember(dest => dest.UlasimServis2, opt => opt.Ignore())
                .ForMember(dest => dest.Tabldot, opt => opt.Ignore())
                .ForMember(dest => dest.HizmetBilgisi, opt => opt.Ignore())
                .ForMember(dest => dest.EgitimBilgisi, opt => opt.Ignore())
                .ForMember(dest => dest.ImzaYetkileri, opt => opt.Ignore())
                .ForMember(dest => dest.CezaBilgileri, opt => opt.Ignore())
                .ForMember(dest => dest.EngelBilgileri, opt => opt.Ignore())
                .ForMember(dest => dest.Gorev, opt => opt.Ignore())
                .ForMember(dest => dest.Uzmanlik, opt => opt.Ignore())
                .ForMember(dest => dest.AtanmaNedeniId, opt => opt.Ignore())
                .ForMember(dest => dest.HizmetBinasiId, opt => opt.Ignore())
                .ForMember(dest => dest.IlId, opt => opt.Ignore())
                .ForMember(dest => dest.IlceId, opt => opt.Ignore())
                .ForMember(dest => dest.SendikaId, opt => opt.Ignore())

                // ═══ AUDIT ═══
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.MapFrom(src => DateTime.Now))

                // ═══ NAVIGATION PROPERTIES ═══
                .ForMember(dest => dest.Departman, opt => opt.Ignore())
                .ForMember(dest => dest.Servis, opt => opt.Ignore())
                .ForMember(dest => dest.Unvan, opt => opt.Ignore())
                .ForMember(dest => dest.AtanmaNedeni, opt => opt.Ignore())
                .ForMember(dest => dest.HizmetBinasi, opt => opt.Ignore())
                .ForMember(dest => dest.Il, opt => opt.Ignore())
                .ForMember(dest => dest.Ilce, opt => opt.Ignore())
                .ForMember(dest => dest.Sendika, opt => opt.Ignore())
                .ForMember(dest => dest.EsininIsIl, opt => opt.Ignore())
                .ForMember(dest => dest.EsininIsIlce, opt => opt.Ignore())
                .ForMember(dest => dest.BankoKullanicilari, opt => opt.Ignore())
                .ForMember(dest => dest.KanalPersonelleri, opt => opt.Ignore())
                .ForMember(dest => dest.PersonelCocuklari, opt => opt.Ignore())
                .ForMember(dest => dest.PersonelYetkileri, opt => opt.Ignore())
                .ForMember(dest => dest.PersonelHizmetleri, opt => opt.Ignore())
                .ForMember(dest => dest.PersonelEgitimleri, opt => opt.Ignore())
                .ForMember(dest => dest.PersonelImzaYetkileri, opt => opt.Ignore())
                .ForMember(dest => dest.PersonelCezalari, opt => opt.Ignore())
                .ForMember(dest => dest.PersonelEngelleri, opt => opt.Ignore())
                .ForMember(dest => dest.HubConnection, opt => opt.Ignore());


            //  List Response
            CreateMap<Personel, PersonelListResponseDto>()
                .ForMember(dest => dest.DepartmanAdi,
                    opt => opt.MapFrom(src => src.Departman != null ? src.Departman.DepartmanAdi : ""))
                .ForMember(dest => dest.ServisAdi,
                    opt => opt.MapFrom(src => src.Servis != null ? src.Servis.ServisAdi : ""))
                .ForMember(dest => dest.UnvanAdi,
                    opt => opt.MapFrom(src => src.Unvan != null ? src.Unvan.UnvanAdi : ""))
                .ForMember(dest => dest.PersonelAktiflikDurum,
                    opt => opt.MapFrom(src => src.PersonelAktiflikDurum.ToString()));
        }
    }
}