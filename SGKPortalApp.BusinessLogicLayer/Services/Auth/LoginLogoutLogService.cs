using AutoMapper;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Auth;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Auth;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Auth;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common;

namespace SGKPortalApp.BusinessLogicLayer.Services.Auth
{
    public class LoginLogoutLogService : ILoginLogoutLogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<LoginLogoutLogService> _logger;

        public LoginLogoutLogService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<LoginLogoutLogService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponseDto<LoginLogoutLogPagedResultDto>> GetLogsAsync(LoginLogoutLogFilterDto filter)
        {
            try
            {
                var loginLogoutRepo = _unitOfWork.Repository<ILoginLogoutLogRepository>();

                var (logs, totalCount) = await loginLogoutRepo.GetFilteredLogsAsync(
                    filter.SearchText,
                    filter.StartDate,
                    filter.EndDate,
                    filter.OnlyActiveSession,
                    filter.OnlyFailedLogins,
                    filter.IpAddress,
                    filter.PageNumber,
                    filter.PageSize);

                var logDtos = logs.Select(l => _mapper.Map<LoginLogoutLogResponseDto>(l)).ToList();

                var result = new LoginLogoutLogPagedResultDto
                {
                    Logs = logDtos,
                    TotalCount = totalCount,
                    PageNumber = filter.PageNumber,
                    PageSize = filter.PageSize
                };

                return ApiResponseDto<LoginLogoutLogPagedResultDto>.SuccessResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login/Logout log'lar getirilirken hata oluştu");
                return ApiResponseDto<LoginLogoutLogPagedResultDto>.ErrorResult("Log'lar getirilirken bir hata oluştu");
            }
        }

        public async Task<ApiResponseDto<LoginLogoutLogResponseDto>> GetLogByIdAsync(int id)
        {
            try
            {
                var loginLogoutRepo = _unitOfWork.Repository<ILoginLogoutLogRepository>();
                var log = await loginLogoutRepo.GetByIdAsync(id);

                if (log == null)
                    return ApiResponseDto<LoginLogoutLogResponseDto>.ErrorResult("Log bulunamadı");

                var dto = _mapper.Map<LoginLogoutLogResponseDto>(log);
                return ApiResponseDto<LoginLogoutLogResponseDto>.SuccessResult(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login/Logout log getir ilirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<LoginLogoutLogResponseDto>.ErrorResult("Log getirilirken bir hata oluştu");
            }
        }

        public async Task<ApiResponseDto<int>> GetActiveSessionCountAsync()
        {
            try
            {
                var loginLogoutRepo = _unitOfWork.Repository<ILoginLogoutLogRepository>();
                var count = await loginLogoutRepo.GetActiveUserCountAsync();
                return ApiResponseDto<int>.SuccessResult(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Aktif oturum sayısı getirilirken hata oluştu");
                return ApiResponseDto<int>.ErrorResult("Aktif oturum sayısı getirilirken bir hata oluştu");
            }
        }

        public async Task<ApiResponseDto<int>> GetTodayLoginCountAsync()
        {
            try
            {
                var loginLogoutRepo = _unitOfWork.Repository<ILoginLogoutLogRepository>();
                var count = await loginLogoutRepo.GetDailyLoginCountAsync(DateTime.Today);
                return ApiResponseDto<int>.SuccessResult(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bugünkü login sayısı getirilirken hata oluştu");
                return ApiResponseDto<int>.ErrorResult("Bugünkü login sayısı getirilirken bir hata oluştu");
            }
        }
    }
}
