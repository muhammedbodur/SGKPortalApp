using AutoMapper;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;

namespace SGKPortalApp.BusinessLogicLayer.Mapping.Profiles.Common
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            // User -> UserResponseDto
            CreateMap<User, UserResponseDto>()
                .ForMember(dest => dest.SessionID, opt => opt.MapFrom(src => src.SessionID))
                .ForMember(dest => dest.PersonelAdSoyad, opt => opt.MapFrom(src => src.Personel == null ? null : src.Personel.AdSoyad))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Personel == null ? null : src.Personel.Email))
                .ForMember(dest => dest.CepTelefonu, opt => opt.MapFrom(src => src.Personel == null ? null : src.Personel.CepTelefonu))
                .ForMember(dest => dest.SicilNo, opt => opt.MapFrom(src => src.Personel == null ? (int?)null : src.Personel.SicilNo))
                .ForMember(dest => dest.DepartmanAdi, opt => opt.MapFrom(src => src.Personel == null || src.Personel.Departman == null ? null : src.Personel.Departman.DepartmanAdi))
                .ForMember(dest => dest.ServisAdi, opt => opt.MapFrom(src => src.Personel == null || src.Personel.Servis == null ? null : src.Personel.Servis.ServisAdi));
        }
    }
}
