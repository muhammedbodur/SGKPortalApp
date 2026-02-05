using AutoMapper;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;

namespace SGKPortalApp.BusinessLogicLayer.Mapping.Profiles.Common
{
    public class OnemliLinkMappingProfile : Profile
    {
        public OnemliLinkMappingProfile()
        {
            CreateMap<OnemliLinkCreateRequestDto, OnemliLink>();
            CreateMap<OnemliLinkUpdateRequestDto, OnemliLink>();
            CreateMap<OnemliLink, OnemliLinkUpdateRequestDto>();
            CreateMap<OnemliLink, OnemliLinkResponseDto>();
        }
    }
}
