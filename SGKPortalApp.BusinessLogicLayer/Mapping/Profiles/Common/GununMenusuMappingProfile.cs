using AutoMapper;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;

namespace SGKPortalApp.BusinessLogicLayer.Mapping.Profiles.Common
{
    public class GununMenusuMappingProfile : Profile
    {
        public GununMenusuMappingProfile()
        {
            CreateMap<GununMenusuCreateRequestDto, GununMenusu>();
            CreateMap<GununMenusuUpdateRequestDto, GununMenusu>();

            CreateMap<GununMenusu, GununMenusuUpdateRequestDto>();

            CreateMap<GununMenusu, GununMenusuResponseDto>();
        }
    }
}
