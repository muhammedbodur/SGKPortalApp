using AutoMapper;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.DTOs.Shared.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco;

namespace SGKPortalApp.BusinessLogicLayer.Mapping.Profiles.ZKTeco
{
    public class DeviceMappingProfile : Profile
    {
        public DeviceMappingProfile()
        {
            // ═══════════════════════════════════════════════════════
            // DEVICE - ENTITY -> RESPONSE DTO
            // ═══════════════════════════════════════════════════════

            CreateMap<Device, DeviceResponseDto>()
                .ForMember(dest => dest.DeviceId,
                    opt => opt.MapFrom(src => src.DeviceId))
                .ForMember(dest => dest.DeviceName,
                    opt => opt.MapFrom(src => src.DeviceName))
                .ForMember(dest => dest.IpAddress,
                    opt => opt.MapFrom(src => src.IpAddress))
                .ForMember(dest => dest.Port,
                    opt => opt.MapFrom(src => src.Port ?? "4370"))
                .ForMember(dest => dest.DeviceCode,
                    opt => opt.MapFrom(src => src.DeviceCode))
                .ForMember(dest => dest.DeviceInfo,
                    opt => opt.MapFrom(src => src.DeviceInfo))
                .ForMember(dest => dest.IsActive,
                    opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.HizmetBinasiId,
                    opt => opt.MapFrom(src => src.HizmetBinasiId))
                .ForMember(dest => dest.HizmetBinasiAdi,
                    opt => opt.MapFrom(src => src.HizmetBinasi != null ? src.HizmetBinasi.HizmetBinasiAdi : null))
                .ForMember(dest => dest.DepartmanAdi,
                    opt => opt.MapFrom(src => src.HizmetBinasi != null && src.HizmetBinasi.DepartmanHizmetBinalari != null 
                        ? src.HizmetBinasi.DepartmanHizmetBinalari.Where(dhb => !dhb.SilindiMi && dhb.Departman != null).Select(dhb => dhb.Departman!.DepartmanAdi).FirstOrDefault() 
                        : null))
                .ForMember(dest => dest.LastSyncTime,
                    opt => opt.MapFrom(src => src.LastSyncTime))
                .ForMember(dest => dest.SyncCount,
                    opt => opt.MapFrom(src => src.SyncCount))
                .ForMember(dest => dest.LastSyncSuccess,
                    opt => opt.MapFrom(src => src.LastSyncSuccess))
                .ForMember(dest => dest.LastSyncStatus,
                    opt => opt.MapFrom(src => src.LastSyncStatus))
                .ForMember(dest => dest.LastHealthCheckTime,
                    opt => opt.MapFrom(src => src.LastHealthCheckTime))
                .ForMember(dest => dest.HealthCheckCount,
                    opt => opt.MapFrom(src => src.HealthCheckCount))
                .ForMember(dest => dest.LastHealthCheckSuccess,
                    opt => opt.MapFrom(src => src.LastHealthCheckSuccess))
                .ForMember(dest => dest.LastHealthCheckStatus,
                    opt => opt.MapFrom(src => src.LastHealthCheckStatus));
        }
    }
}
