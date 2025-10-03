using AutoMapper;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.PresentationLayer.Models.FormModels;

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

            // FormModel → Create Request DTO
            CreateMap<UnvanFormModel, UnvanCreateRequestDto>();

            // FormModel → Update Request DTO
            CreateMap<UnvanFormModel, UnvanUpdateRequestDto>();

            // Response DTO → FormModel
            CreateMap<UnvanResponseDto, UnvanFormModel>();
        }
    }
}