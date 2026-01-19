using AutoMapper;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.BusinessLogicLayer.Mappings
{
    public class ResmiTatilMappingProfile : Profile
    {
        public ResmiTatilMappingProfile()
        {
            CreateMap<ResmiTatil, ResmiTatilResponseDto>()
                .ForMember(dest => dest.TatilTipiText, opt => opt.MapFrom(src => GetTatilTipiText(src.TatilTipi)));

            CreateMap<ResmiTatilCreateRequestDto, ResmiTatil>()
                .ForMember(dest => dest.TatilId, opt => opt.Ignore())
                .ForMember(dest => dest.Yil, opt => opt.Ignore())
                .ForMember(dest => dest.OtomatikSenkronize, opt => opt.Ignore())
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.SilindiMi, opt => opt.Ignore())
                .ForMember(dest => dest.EkleyenKullanici, opt => opt.Ignore())
                .ForMember(dest => dest.DuzenleyenKullanici, opt => opt.Ignore());

            CreateMap<ResmiTatilUpdateRequestDto, ResmiTatil>()
                .ForMember(dest => dest.Yil, opt => opt.Ignore())
                .ForMember(dest => dest.OtomatikSenkronize, opt => opt.Ignore())
                .ForMember(dest => dest.EklenmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.DuzenlenmeTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.SilindiMi, opt => opt.Ignore())
                .ForMember(dest => dest.EkleyenKullanici, opt => opt.Ignore())
                .ForMember(dest => dest.DuzenleyenKullanici, opt => opt.Ignore());
        }

        private static string GetTatilTipiText(TatilTipi tatilTipi)
        {
            return tatilTipi switch
            {
                TatilTipi.SabitTatil => "Sabit Tatil",
                TatilTipi.DiniTatil => "Dini Tatil",
                TatilTipi.OzelTatil => "Özel Tatil",
                _ => "Bilinmiyor"
            };
        }
    }
}
