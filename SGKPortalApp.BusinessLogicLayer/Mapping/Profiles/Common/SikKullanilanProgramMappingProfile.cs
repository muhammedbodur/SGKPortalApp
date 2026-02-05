using AutoMapper;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;

namespace SGKPortalApp.BusinessLogicLayer.Mapping.Profiles.Common
{
    public class SikKullanilanProgramMappingProfile : Profile
    {
        public SikKullanilanProgramMappingProfile()
        {
            CreateMap<SikKullanilanProgramCreateRequestDto, SikKullanilanProgram>();
            CreateMap<SikKullanilanProgramUpdateRequestDto, SikKullanilanProgram>();
            CreateMap<SikKullanilanProgram, SikKullanilanProgramUpdateRequestDto>();
            CreateMap<SikKullanilanProgram, SikKullanilanProgramResponseDto>();
        }
    }
}
