using AutoMapper;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Mapping.Profiles.PersonelIslemleri
{
    public class SendikaMappingProfile : Profile
    {
        public SendikaMappingProfile()
        {
            // Request -> Entity
            CreateMap<SendikaCreateRequestDto, Sendika>();
            CreateMap<SendikaUpdateRequestDto, Sendika>();

            // Entity -> Response
            CreateMap<Sendika, SendikaResponseDto>()
                .ForMember(dest => dest.PersonelSayisi,
                    opt => opt.MapFrom(src => src.Personeller != null
                        ? src.Personeller.Count(p => !p.SilindiMi && p.PersonelAktiflikDurum == PersonelAktiflikDurum.Aktif)
                        : 0));
        }
    }
}
