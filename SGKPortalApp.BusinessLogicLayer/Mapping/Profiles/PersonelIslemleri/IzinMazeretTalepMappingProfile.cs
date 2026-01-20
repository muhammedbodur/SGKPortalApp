using AutoMapper;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.PdksIslemleri;
using SGKPortalApp.Common.Extensions;

namespace SGKPortalApp.BusinessLogicLayer.Mapping.Profiles.PersonelIslemleri
{
    /// <summary>
    /// İzin/Mazeret talepleri AutoMapper profile
    /// Request DTO -> Entity mapping
    /// Response DTO mapping'leri service layer'da manuel yapılıyor (GetDescription() extension kullanımı için)
    /// </summary>
    public class IzinMazeretTalepMappingProfile : Profile
    {
        public IzinMazeretTalepMappingProfile()
        {
            // ═══════════════════════════════════════════════════════
            // REQUEST DTO -> ENTITY (Create)
            // ═══════════════════════════════════════════════════════

            CreateMap<IzinMazeretTalepCreateRequestDto, IzinMazeretTalep>()
                // ID ve Audit alanları ignore
                .ForMember(dest => dest.IzinMazeretTalepId, opt => opt.Ignore())
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.EkleyenKullanici, opt => opt.Ignore())
                .ForMember(dest => dest.DuzenleyenKullanici, opt => opt.Ignore())
                .ForMember(dest => dest.SilindiMi, opt => opt.Ignore())
                .ForMember(dest => dest.SilinmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.SilenKullanici, opt => opt.Ignore())

                // Navigation property ignore
                .ForMember(dest => dest.Personel, opt => opt.Ignore())

                // Otomatik set edilen alanlar ignore (service layer'da set edilecek)
                .ForMember(dest => dest.TalepTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.ToplamGun, opt => opt.Ignore())

                // Onay durumları default değerlerde kalacak
                .ForMember(dest => dest.BirinciOnayDurumu, opt => opt.Ignore())
                .ForMember(dest => dest.BirinciOnayTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.BirinciOnayAciklama, opt => opt.Ignore())
                .ForMember(dest => dest.IkinciOnayDurumu, opt => opt.Ignore())
                .ForMember(dest => dest.IkinciOnayTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.IkinciOnayAciklama, opt => opt.Ignore());

            // ═══════════════════════════════════════════════════════
            // REQUEST DTO -> ENTITY (Update)
            // ═══════════════════════════════════════════════════════

            CreateMap<IzinMazeretTalepUpdateRequestDto, IzinMazeretTalep>()
                // ID ve Audit alanları ignore
                .ForMember(dest => dest.IzinMazeretTalepId, opt => opt.Ignore())
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.EkleyenKullanici, opt => opt.Ignore())
                .ForMember(dest => dest.DuzenleyenKullanici, opt => opt.Ignore())
                .ForMember(dest => dest.SilindiMi, opt => opt.Ignore())
                .ForMember(dest => dest.SilinmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.SilenKullanici, opt => opt.Ignore())

                // Navigation property ignore
                .ForMember(dest => dest.Personel, opt => opt.Ignore())

                // Update edilmeyen alanlar
                .ForMember(dest => dest.TcKimlikNo, opt => opt.Ignore())
                .ForMember(dest => dest.TalepTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.ToplamGun, opt => opt.Ignore())

                // Onay bilgileri değişmez
                .ForMember(dest => dest.BirinciOnayciTcKimlikNo, opt => opt.Ignore())
                .ForMember(dest => dest.BirinciOnayDurumu, opt => opt.Ignore())
                .ForMember(dest => dest.BirinciOnayTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.BirinciOnayAciklama, opt => opt.Ignore())
                .ForMember(dest => dest.IkinciOnayciTcKimlikNo, opt => opt.Ignore())
                .ForMember(dest => dest.IkinciOnayDurumu, opt => opt.Ignore())
                .ForMember(dest => dest.IkinciOnayTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.IkinciOnayAciklama, opt => opt.Ignore());

            // ═══════════════════════════════════════════════════════
            // ENTITY -> RESPONSE DTO (List)
            // ═══════════════════════════════════════════════════════

            CreateMap<IzinMazeretTalep, IzinMazeretTalepListResponseDto>()
                .ForMember(dest => dest.AdSoyad, opt => opt.MapFrom(src => src.Personel != null ? src.Personel.AdSoyad : ""))
                .ForMember(dest => dest.SicilNo, opt => opt.MapFrom(src => src.Personel != null ? src.Personel.SicilNo : 0))
                .ForMember(dest => dest.DepartmanAdi, opt => opt.MapFrom(src => src.Personel != null && src.Personel.Departman != null ? src.Personel.Departman.DepartmanAdi : null))
                .ForMember(dest => dest.ServisAdi, opt => opt.MapFrom(src => src.Personel != null && src.Personel.Servis != null ? src.Personel.Servis.ServisAdi : null))
                .ForMember(dest => dest.TuruAdi, opt => opt.MapFrom(src => src.IzinMazeretTuru != null ? src.IzinMazeretTuru.TuruAdi : ""))
                .ForMember(dest => dest.BirinciOnayDurumuAdi, opt => opt.MapFrom(src => src.BirinciOnayDurumu.GetDescription()))
                .ForMember(dest => dest.IkinciOnayDurumuAdi, opt => opt.MapFrom(src => src.IkinciOnayDurumu.GetDescription()))
                .ForMember(dest => dest.BirinciOnayciAdSoyad, opt => opt.MapFrom(src => src.BirinciOnayci != null ? src.BirinciOnayci.AdSoyad : null))
                .ForMember(dest => dest.IkinciOnayciAdSoyad, opt => opt.MapFrom(src => src.IkinciOnayci != null ? src.IkinciOnayci.AdSoyad : null))
                .ForMember(dest => dest.GenelDurum, opt => opt.Ignore()); // Service layer'da hesaplanacak

            // ═══════════════════════════════════════════════════════
            // ENTITY -> RESPONSE DTO (Detail)
            // ═══════════════════════════════════════════════════════

            CreateMap<IzinMazeretTalep, IzinMazeretTalepResponseDto>()
                .ForMember(dest => dest.AdSoyad, opt => opt.MapFrom(src => src.Personel != null ? src.Personel.AdSoyad : ""))
                .ForMember(dest => dest.SicilNo, opt => opt.MapFrom(src => src.Personel != null ? src.Personel.SicilNo : 0))
                .ForMember(dest => dest.DepartmanAdi, opt => opt.MapFrom(src => src.Personel != null && src.Personel.Departman != null ? src.Personel.Departman.DepartmanAdi : null))
                .ForMember(dest => dest.ServisAdi, opt => opt.MapFrom(src => src.Personel != null && src.Personel.Servis != null ? src.Personel.Servis.ServisAdi : null))
                .ForMember(dest => dest.TuruAdi, opt => opt.MapFrom(src => src.IzinMazeretTuru != null ? src.IzinMazeretTuru.TuruAdi : ""))
                .ForMember(dest => dest.BirinciOnayDurumuAdi, opt => opt.MapFrom(src => src.BirinciOnayDurumu.GetDescription()))
                .ForMember(dest => dest.IkinciOnayDurumuAdi, opt => opt.MapFrom(src => src.IkinciOnayDurumu.GetDescription()))
                .ForMember(dest => dest.BirinciOnayciAdSoyad, opt => opt.MapFrom(src => src.BirinciOnayci != null ? src.BirinciOnayci.AdSoyad : null))
                .ForMember(dest => dest.IkinciOnayciAdSoyad, opt => opt.MapFrom(src => src.IkinciOnayci != null ? src.IkinciOnayci.AdSoyad : null));
        }
    }
}
