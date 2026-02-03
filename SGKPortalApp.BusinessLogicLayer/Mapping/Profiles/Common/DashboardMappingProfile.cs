using AutoMapper;
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
        }
    }
}
