using AutoMapper;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Mapping.Profiles
{
    public class HizmetBinasiMappingProfile : Profile
    {
        public HizmetBinasiMappingProfile()
        {
            // ═══════════════════════════════════════════════════════
            // Entity -> Response DTO
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

            CreateMap<HizmetBinasi, HizmetBinasiDetailResponseDto>()
                .ForMember(dest => dest.DepartmanAdi,
                    opt => opt.MapFrom(src => src.Departman != null ? src.Departman.DepartmanAdi : string.Empty))
                .ForMember(dest => dest.Personeller,
                    opt => opt.MapFrom(src => src.Personeller ?? new List<Personel>()))
                .ForMember(dest => dest.Bankolar,
                    opt => opt.MapFrom(src => src.Bankolar ?? new List<Banko>()))
                .ForMember(dest => dest.Tvler,
                    opt => opt.MapFrom(src => src.Tvler ?? new List<Tv>()));

            // ═══════════════════════════════════════════════════════
            // Request DTO -> Entity (Create)
            // ═══════════════════════════════════════════════════════

            CreateMap<HizmetBinasiCreateRequestDto, HizmetBinasi>()
                .ForMember(dest => dest.HizmetBinasiId, opt => opt.Ignore())
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.Departman, opt => opt.Ignore())
                .ForMember(dest => dest.Personeller, opt => opt.Ignore())
                .ForMember(dest => dest.Bankolar, opt => opt.Ignore())
                .ForMember(dest => dest.Tvler, opt => opt.Ignore());

            // ═══════════════════════════════════════════════════════
            // Request DTO -> Entity (Update)
            // ═══════════════════════════════════════════════════════

            CreateMap<HizmetBinasiUpdateRequestDto, HizmetBinasi>()
                .ForMember(dest => dest.HizmetBinasiId, opt => opt.Ignore())
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.Departman, opt => opt.Ignore())
                .ForMember(dest => dest.Personeller, opt => opt.Ignore())
                .ForMember(dest => dest.Bankolar, opt => opt.Ignore())
                .ForMember(dest => dest.Tvler, opt => opt.Ignore());

            // ═══════════════════════════════════════════════════════
            // Reverse Mapping (gerekirse)
            // ═══════════════════════════════════════════════════════

            CreateMap<HizmetBinasiResponseDto, HizmetBinasi>()
                .ForMember(dest => dest.Departman, opt => opt.Ignore())
                .ForMember(dest => dest.Personeller, opt => opt.Ignore())
                .ForMember(dest => dest.Bankolar, opt => opt.Ignore())
                .ForMember(dest => dest.Tvler, opt => opt.Ignore());
        }
    }
}