using AutoMapper;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.Common.Helpers;
using SGKPortalApp.Common.Extensions;

namespace SGKPortalApp.BusinessLogicLayer.Mapping.Profiles.PersonelIslemleri
{
    public class PersonelAktiflikDurumHareketMappingProfile : Profile
    {
        public PersonelAktiflikDurumHareketMappingProfile()
        {
            CreateMap<PersonelAktiflikDurumHareket, PersonelAktiflikDurumHareketResponseDto>()
                .ForMember(dest => dest.OncekiDurumAdi, opt => opt.MapFrom(src => src.OncekiDurum.HasValue ? EnumExtensions.GetDisplayName(src.OncekiDurum.Value) : null))
                .ForMember(dest => dest.YeniDurumAdi, opt => opt.MapFrom(src => EnumExtensions.GetDisplayName(src.YeniDurum)));

            CreateMap<PersonelAktiflikDurumHareketCreateRequestDto, PersonelAktiflikDurumHareket>()
                .ForMember(dest => dest.PersonelAktiflikDurumHareketId, opt => opt.Ignore())
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.MapFrom(src => DateTimeHelper.Now))
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.MapFrom(src => DateTimeHelper.Now))
                .ForMember(dest => dest.Personel, opt => opt.Ignore());
        }
    }
}
