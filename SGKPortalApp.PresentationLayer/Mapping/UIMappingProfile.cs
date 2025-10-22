using AutoMapper;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.PresentationLayer.Models.FormModels.PersonelIslemleri;

namespace SGKPortalApp.PresentationLayer.Mapping
{
    /// <summary>
    /// UI katmanı AutoMapper profili
    /// FormModel ↔ DTO mapping'leri
    /// </summary>
    public class UIMappingProfile : Profile
    {
        public UIMappingProfile()
        {
            // ═══════════════════════════════════════════════════════
            // DEPARTMAN FORM MODEL MAPPING'LERİ
            // ═══════════════════════════════════════════════════════

            // FormModel → Create Request DTO
            CreateMap<DepartmanFormModel, DepartmanCreateRequestDto>();

            // FormModel → Update Request DTO
            CreateMap<DepartmanFormModel, DepartmanUpdateRequestDto>();

            // Response DTO → FormModel (Düzenleme için)
            CreateMap<DepartmanResponseDto, DepartmanFormModel>();


            // ═══════════════════════════════════════════════════════
            // SERVİS FORM MODEL MAPPING'LERİ
            // ═══════════════════════════════════════════════════════

            // FormModel → Create Request DTO
            CreateMap<ServisFormModel, ServisCreateRequestDto>();

            // FormModel → Update Request DTO
            CreateMap<ServisFormModel, ServisUpdateRequestDto>();

            // Response DTO → FormModel
            CreateMap<ServisResponseDto, ServisFormModel>();


            // ═══════════════════════════════════════════════════════
            // UNVAN FORM MODEL MAPPING'LERİ
            // ═══════════════════════════════════════════════════════

            CreateMap<UnvanFormModel, UnvanCreateRequestDto>();

            // FormModel → Update Request DTO
            CreateMap<UnvanFormModel, UnvanUpdateRequestDto>();

            // Response DTO → FormModel
            CreateMap<UnvanResponseDto, UnvanFormModel>();


            // ═════════════════════════════════════════════════════════
            // PERSONEL FORM MODEL MAPPING'LERİ
            // ═════════════════════════════════════════════════════════

            // ✅ DÜZELTİLMİŞ: FormModel'den gelen GERÇEK değerleri kullan
            CreateMap<PersonelFormModel, PersonelCreateRequestDto>()
                .ForMember(dest => dest.AtanmaNedeniId, opt => opt.MapFrom(src => src.AtanmaNedeniId))
                .ForMember(dest => dest.HizmetBinasiId, opt => opt.MapFrom(src => src.HizmetBinasiId))
                .ForMember(dest => dest.IlId, opt => opt.MapFrom(src => src.IlId))
                .ForMember(dest => dest.IlceId, opt => opt.MapFrom(src => src.IlceId))
                .ForMember(dest => dest.SendikaId, opt => opt.MapFrom(src => src.SendikaId > 0 ? src.SendikaId : 0))
                .ForMember(dest => dest.OgrenimSuresi, opt => opt.MapFrom(src => src.OgrenimSuresi));

            // FormModel → Update Request DTO
            CreateMap<PersonelFormModel, PersonelUpdateRequestDto>();

            // Response DTO → FormModel (Düzenleme için)
            CreateMap<PersonelResponseDto, PersonelFormModel>();
        }
    }
}