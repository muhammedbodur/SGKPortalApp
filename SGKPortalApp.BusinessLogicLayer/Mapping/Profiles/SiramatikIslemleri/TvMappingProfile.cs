using AutoMapper;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.BusinessLogicLayer.Mapping.Profiles.SiramatikIslemleri
{
    public class TvMappingProfile : Profile
    {
        public TvMappingProfile()
        {
            // Request to Entity
            CreateMap<TvCreateRequestDto, Tv>()
                .ForMember(dest => dest.TvId, opt => opt.Ignore())
                .ForMember(dest => dest.HizmetBinasi, opt => opt.Ignore())
                .ForMember(dest => dest.IslemZamani, opt => opt.Ignore())
                .ForMember(dest => dest.HubTvConnection, opt => opt.Ignore())
                .ForMember(dest => dest.TvBankolar, opt => opt.Ignore())
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.SilindiMi, opt => opt.Ignore());

            CreateMap<TvUpdateRequestDto, Tv>()
                .ForMember(dest => dest.HizmetBinasi, opt => opt.Ignore())
                .ForMember(dest => dest.IslemZamani, opt => opt.Ignore())
                .ForMember(dest => dest.HubTvConnection, opt => opt.Ignore())
                .ForMember(dest => dest.TvBankolar, opt => opt.Ignore())
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.SilindiMi, opt => opt.Ignore());

            // Entity to Response
            CreateMap<Tv, TvResponseDto>()
                .ForMember(dest => dest.HizmetBinasiAdi, opt => opt.MapFrom(src => src.HizmetBinasi != null ? src.HizmetBinasi.HizmetBinasiAdi : null))
                .ForMember(dest => dest.BankoSayisi, opt => opt.MapFrom(src => src.TvBankolar != null ? src.TvBankolar.Count(tb => tb.Aktiflik == Aktiflik.Aktif) : 0))
                .ForMember(dest => dest.EslesmiBankoIdler, opt => opt.MapFrom(src => src.TvBankolar != null ? src.TvBankolar.Where(tb => tb.Aktiflik == Aktiflik.Aktif).Select(tb => tb.BankoId).ToList() : new List<int>()))
                .ForMember(dest => dest.IsConnected, opt => opt.Ignore()); // Service'de hesaplanacak
        }
    }
}
