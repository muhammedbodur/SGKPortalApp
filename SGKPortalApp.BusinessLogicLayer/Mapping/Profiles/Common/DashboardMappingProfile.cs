using AutoMapper;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Dashboard;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;

namespace SGKPortalApp.BusinessLogicLayer.Mapping.Profiles.Common
{
    /// <summary>
    /// Dashboard için AutoMapper profili
    /// </summary>
    public class DashboardMappingProfile : Profile
    {
        public DashboardMappingProfile()
        {
            // Entity -> Response mappings
            CreateMap<Duyuru, DuyuruResponseDto>();
            CreateMap<OnemliLink, OnemliLinkResponseDto>();
            CreateMap<SikKullanilanProgram, SikKullanilanProgramResponseDto>();
            CreateMap<GununMenusu, GununMenusuResponseDto>();

            // Request -> Entity mappings (for Duyuru CRUD)
            CreateMap<DuyuruCreateRequestDto, Duyuru>()
                .ForMember(dest => dest.DuyuruId, opt => opt.Ignore())
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.SilindiMi, opt => opt.Ignore())
                .ForMember(dest => dest.EkleyenKullanici, opt => opt.Ignore())
                .ForMember(dest => dest.DuzenleyenKullanici, opt => opt.Ignore());

            CreateMap<DuyuruUpdateRequestDto, Duyuru>()
                .ForMember(dest => dest.DuyuruId, opt => opt.Ignore())
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.SilindiMi, opt => opt.Ignore())
                .ForMember(dest => dest.EkleyenKullanici, opt => opt.Ignore())
                .ForMember(dest => dest.DuzenleyenKullanici, opt => opt.Ignore());
        }
    }
}
