using AutoMapper;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Mapping.Profiles
{
    public class SendikaMappingProfile : Profile
    {
        public SendikaMappingProfile()
        {
            // Request -> Entity
            CreateMap<SendikaCreateRequestDto, Sendika>();
            CreateMap<SendikaUpdateRequestDto, Sendika>();

            // Entity -> Response
            CreateMap<Sendika, SendikaResponseDto>();
        }
    }
}
