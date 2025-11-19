using AutoMapper;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;

namespace SGKPortalApp.BusinessLogicLayer.Mapping.Profiles.Common
{
    public class IlceMappingProfile : Profile
    {
        public IlceMappingProfile()
        {
            // Request -> Entity
            CreateMap<IlceCreateRequestDto, Ilce>();
            CreateMap<IlceUpdateRequestDto, Ilce>();

            // Entity -> Response
            CreateMap<Ilce, IlceResponseDto>()
                .ForMember(dest => dest.IlAdi, opt => opt.MapFrom(src => src.Il.IlAdi));
        }
    }
}
