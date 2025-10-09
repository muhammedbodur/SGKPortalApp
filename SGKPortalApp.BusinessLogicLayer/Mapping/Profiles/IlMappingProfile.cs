using AutoMapper;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;

namespace SGKPortalApp.BusinessLogicLayer.Mapping.Profiles
{
    public class IlMappingProfile : Profile
    {
        public IlMappingProfile()
        {
            // Request -> Entity
            CreateMap<IlCreateRequestDto, Il>();
            CreateMap<IlUpdateRequestDto, Il>();

            // Entity -> Response
            CreateMap<Il, IlResponseDto>()
                .ForMember(dest => dest.IlceSayisi, opt => opt.Ignore()); // Service'de set edilecek
        }
    }
}
