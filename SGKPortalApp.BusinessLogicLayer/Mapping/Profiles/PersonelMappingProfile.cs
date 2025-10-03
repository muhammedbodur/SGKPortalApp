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

            // Entity → Response DTO
            CreateMap<Departman, DepartmanResponseDto>()
                .ForMember(dest => dest.PersonelSayisi,
                    opt => opt.MapFrom(src => src.Personeller != null ? src.Personeller.Count : 0));

            // Create Request DTO → Entity
            CreateMap<DepartmanCreateRequestDto, Departman>()
                .ForMember(dest => dest.DepartmanId, opt => opt.Ignore())
                .ForMember(dest => dest.DepartmanAktiflik,
                    opt => opt.MapFrom(src => BusinessObjectLayer.Enums.Common.Aktiflik.Aktif))
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Personeller, opt => opt.Ignore());

            // Update Request DTO → Entity
            CreateMap<DepartmanUpdateRequestDto, Departman>()
                .ForMember(dest => dest.DepartmanId, opt => opt.Ignore())
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Personeller, opt => opt.Ignore());

            // Response DTO → Create Request DTO (FormModel için)
            CreateMap<DepartmanResponseDto, DepartmanCreateRequestDto>();

            // Response DTO → Update Request DTO (FormModel için)
            CreateMap<DepartmanResponseDto, DepartmanUpdateRequestDto>();


            // ═══════════════════════════════════════════════════════
            // SERVİS MAPPING'LERİ
            // ═══════════════════════════════════════════════════════

            // Entity → Response DTO
            CreateMap<Servis, ServisResponseDto>()
                .ForMember(dest => dest.PersonelSayisi,
                    opt => opt.MapFrom(src => src.Personeller != null ? src.Personeller.Count : 0));

            // Create Request DTO → Entity
            CreateMap<ServisCreateRequestDto, Servis>()
                .ForMember(dest => dest.ServisId, opt => opt.Ignore())
                .ForMember(dest => dest.ServisAktiflik,
                    opt => opt.MapFrom(src => BusinessObjectLayer.Enums.Common.Aktiflik.Aktif))
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Personeller, opt => opt.Ignore());

            // Update Request DTO → Entity
            CreateMap<ServisUpdateRequestDto, Servis>()
                .ForMember(dest => dest.ServisId, opt => opt.Ignore())
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Personeller, opt => opt.Ignore());


            // ═══════════════════════════════════════════════════════
            // UNVAN MAPPING'LERİ
            // ═══════════════════════════════════════════════════════

            // Entity → Response DTO
            CreateMap<Unvan, UnvanResponseDto>()
                .ForMember(dest => dest.PersonelSayisi,
                    opt => opt.MapFrom(src => src.Personeller != null ? src.Personeller.Count : 0));

            // Create Request DTO → Entity
            CreateMap<UnvanCreateRequestDto, Unvan>()
                .ForMember(dest => dest.UnvanId, opt => opt.Ignore())
                .ForMember(dest => dest.UnvanAktiflik,
                    opt => opt.MapFrom(src => BusinessObjectLayer.Enums.Common.Aktiflik.Aktif))
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Personeller, opt => opt.Ignore());

            // Update Request DTO → Entity
            CreateMap<UnvanUpdateRequestDto, Unvan>()
                .ForMember(dest => dest.UnvanId, opt => opt.Ignore())
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Personeller, opt => opt.Ignore());


            // ═══════════════════════════════════════════════════════
            // PERSONEL MAPPING'LERİ
            // ═══════════════════════════════════════════════════════

            // Entity → Response DTO
            CreateMap<Personel, PersonelResponseDto>()
                .ForMember(dest => dest.DepartmanAdi,
                    opt => opt.MapFrom(src => src.Departman != null ? src.Departman.DepartmanAdi : ""))
                .ForMember(dest => dest.ServisAdi,
                    opt => opt.MapFrom(src => src.Servis != null ? src.Servis.ServisAdi : ""))
                .ForMember(dest => dest.UnvanAdi,
                    opt => opt.MapFrom(src => src.Unvan != null ? src.Unvan.UnvanAdi : ""));

            // Create Request DTO → Entity
            CreateMap<PersonelCreateRequestDto, Personel>()
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Departman, opt => opt.Ignore())
                .ForMember(dest => dest.Servis, opt => opt.Ignore())
                .ForMember(dest => dest.Unvan, opt => opt.Ignore())
                .ForMember(dest => dest.PersonelCocuklari, opt => opt.Ignore());

            // Update Request DTO → Entity
            CreateMap<PersonelUpdateRequestDto, Personel>()
                .ForMember(dest => dest.TcKimlikNo, opt => opt.Ignore())
                .ForMember(dest => dest.SicilNo, opt => opt.Ignore())
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Departman, opt => opt.Ignore())
                .ForMember(dest => dest.Servis, opt => opt.Ignore())
                .ForMember(dest => dest.Unvan, opt => opt.Ignore())
                .ForMember(dest => dest.PersonelCocuklari, opt => opt.Ignore());
        }
    }
}