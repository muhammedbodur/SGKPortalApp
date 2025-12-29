using AutoMapper;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Auth;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;

namespace SGKPortalApp.BusinessLogicLayer.Mapping.Profiles.Auth
{
    public class LoginLogoutLogMappingProfile : Profile
    {
        public LoginLogoutLogMappingProfile()
        {
            // Entity → Response DTO
            CreateMap<LoginLogoutLog, LoginLogoutLogResponseDto>();
        }
    }
}
