using AutoMapper;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Mapping.Profiles.PersonelIslemleri
{
    public class PersonelImzaYetkisiMappingProfile : Profile
    {
        public PersonelImzaYetkisiMappingProfile()
        {
            CreateMap<PersonelImzaYetkisi, PersonelImzaYetkisiResponseDto>()
                .ForMember(dest => dest.DepartmanAdi, opt => opt.MapFrom(src => src.Departman != null ? src.Departman.DepartmanAdi : null))
                .ForMember(dest => dest.ServisAdi, opt => opt.MapFrom(src => src.Servis != null ? src.Servis.ServisAdi : null));
            
            CreateMap<PersonelImzaYetkisiCreateRequestDto, PersonelImzaYetkisi>()
                .ForMember(dest => dest.PersonelImzaYetkisiId, opt => opt.Ignore())
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Personel, opt => opt.Ignore())
                .ForMember(dest => dest.Departman, opt => opt.Ignore())
                .ForMember(dest => dest.Servis, opt => opt.Ignore());
        }
    }
}
