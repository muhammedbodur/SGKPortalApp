using AutoMapper;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Auth;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Auth;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Auth;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.Common.Helpers;
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
                var loginLogoutRepo = _unitOfWork.GetRepository<ILoginLogoutLogRepository>();

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
                var loginLogoutRepo = _unitOfWork.GetRepository<ILoginLogoutLogRepository>();
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
                var loginLogoutRepo = _unitOfWork.GetRepository<ILoginLogoutLogRepository>();
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
                var loginLogoutRepo = _unitOfWork.GetRepository<ILoginLogoutLogRepository>();
                var count = await loginLogoutRepo.GetDailyLoginCountAsync(DateTime.Today);
                return ApiResponseDto<int>.SuccessResult(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bugünkü login sayısı getirilirken hata oluştu");
                return ApiResponseDto<int>.ErrorResult("Bugünkü login sayısı getirilirken bir hata oluştu");
            }
        }

        public async Task<ApiResponseDto<bool>> UpdateLogoutTimeBySessionIdAsync(string sessionId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sessionId))
                    return ApiResponseDto<bool>.ErrorResult("SessionId boş olamaz");

                var loginLogoutRepo = _unitOfWork.GetRepository<ILoginLogoutLogRepository>();
                var result = await loginLogoutRepo.UpdateLogoutTimeBySessionIdAsync(sessionId, DateTimeHelper.Now);

                if (result)
                {
                    await _unitOfWork.SaveChangesAsync();
                    _logger.LogInformation("✅ Logout time güncellendi - SessionID: {SessionId}", sessionId);
                }
                else
                {
                    _logger.LogWarning("⚠️ SessionID ile aktif login log bulunamadı - SessionID: {SessionId}", sessionId);
                }

                return ApiResponseDto<bool>.SuccessResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Logout time güncellenirken hata - SessionID: {SessionId}", sessionId);
                return ApiResponseDto<bool>.ErrorResult("Logout time güncellenirken bir hata oluştu");
            }
        }

        public async Task<ApiResponseDto<bool>> UpdateLogoutTimeByTcKimlikNoAsync(string tcKimlikNo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tcKimlikNo))
                    return ApiResponseDto<bool>.ErrorResult("TcKimlikNo boş olamaz");

                // User'dan SessionID'yi al
                var userRepo = _unitOfWork.GetRepository<IUserRepository>();
                var user = await userRepo.GetByTcKimlikNoAsync(tcKimlikNo);

                if (user == null || string.IsNullOrWhiteSpace(user.SessionID))
                {
                    _logger.LogWarning("⚠️ User veya SessionID bulunamadı - TcKimlikNo: {TcKimlikNo}", tcKimlikNo);
                    return ApiResponseDto<bool>.SuccessResult(false);
                }

                // LoginLogoutLog güncelle
                var loginLogoutRepo = _unitOfWork.GetRepository<ILoginLogoutLogRepository>();
                var result = await loginLogoutRepo.UpdateLogoutTimeBySessionIdAsync(user.SessionID, DateTimeHelper.Now);

                if (result)
                {
                    await _unitOfWork.SaveChangesAsync();
                    _logger.LogInformation("✅ Logout time güncellendi - TcKimlikNo: {TcKimlikNo}, SessionID: {SessionID}",
                        tcKimlikNo, user.SessionID);
                }
                else
                {
                    _logger.LogWarning("⚠️ SessionID ile aktif login log bulunamadı - SessionID: {SessionID}", user.SessionID);
                }

                return ApiResponseDto<bool>.SuccessResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Logout time güncellenirken hata - TcKimlikNo: {TcKimlikNo}", tcKimlikNo);
                return ApiResponseDto<bool>.ErrorResult("Logout time güncellenirken bir hata oluştu");
            }
        }

        public async Task<ApiResponseDto<int>> CleanupOrphanSessionsAsync()
        {
            try
            {
                var loginLogoutRepo = _unitOfWork.GetRepository<ILoginLogoutLogRepository>();
                var hubConnectionRepo = _unitOfWork.GetRepository<IHubConnectionRepository>();

                var sessionTimeout = TimeSpan.FromHours(8);
                var hubConnectionTimeout = TimeSpan.FromMinutes(15);
                var now = DateTimeHelper.Now;

                int totalCleaned = 0;

                // 1️⃣ Orphan LoginLogoutLog kayıtlarını temizle (LogoutTime null ve çok eski)
                var sessionTimeoutThreshold = now.Subtract(sessionTimeout);
                var orphanCount = await loginLogoutRepo.UpdateOrphanSessionsLogoutTimeAsync(
                    sessionTimeoutThreshold,
                    sessionTimeoutThreshold.Add(sessionTimeout)); // LoginTime + 8 saat

                totalCleaned += orphanCount;
                _logger.LogInformation("🧹 {Count} orphan session temizlendi", orphanCount);

                // 2️⃣ Stale HubConnection kayıtlarını pasifleştir
                var hubTimeoutThreshold = now.Subtract(hubConnectionTimeout);
                var staleCount = await hubConnectionRepo.DeactivateStaleConnectionsAsync(hubTimeoutThreshold);
                totalCleaned += staleCount;
                _logger.LogInformation("🧹 {Count} stale hub connection pasifleştirildi", staleCount);

                // 3️⃣ Disconnected-but-not-logged-out sessions
                var activeSessionIds = await hubConnectionRepo.GetActiveSessionIdsAsync();
                var disconnectedSessions = await loginLogoutRepo.GetDisconnectedButNotLoggedOutSessionsAsync(
                    hubTimeoutThreshold,
                    activeSessionIds);

                foreach (var session in disconnectedSessions)
                {
                    session.LogoutTime = now;
                    totalCleaned++;
                }

                _logger.LogInformation("🧹 {Count} disconnected-but-not-logged-out session temizlendi", disconnectedSessions.Count());

                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("✅ Toplam {Count} kayıt temizlendi", totalCleaned);

                return ApiResponseDto<int>.SuccessResult(totalCleaned);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Orphan session cleanup hatası");
                return ApiResponseDto<int>.ErrorResult("Cleanup işlemi sırasında hata oluştu");
            }
        }

        public async Task<ApiResponseDto<bool>> IsSessionValidAsync(string sessionId)
        {
            try
            {
                var loginLogoutRepo = _unitOfWork.GetRepository<ILoginLogoutLogRepository>();

                // SessionID ile log kaydını bul
                var log = await loginLogoutRepo.GetBySessionIdAsync(sessionId);

                if (log == null)
                {
                    // Session kaydı yok - geçersiz
                    _logger.LogWarning("⚠️ Session kaydı bulunamadı - SessionID: {SessionId}", sessionId);
                    return ApiResponseDto<bool>.SuccessResult(false, "Session kaydı bulunamadı");
                }

                // LogoutTime set edilmişse session geçersiz
                if (log.LogoutTime.HasValue)
                {
                    _logger.LogInformation("🚪 Session sonlanmış (LogoutTime set) - SessionID: {SessionId}, LogoutTime: {LogoutTime}",
                        sessionId, log.LogoutTime.Value);
                    return ApiResponseDto<bool>.SuccessResult(false, "Session sonlanmış");
                }

                // LogoutTime yok - session geçerli
                _logger.LogDebug("✅ Session geçerli - SessionID: {SessionId}", sessionId);
                return ApiResponseDto<bool>.SuccessResult(true, "Session geçerli");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Session validity kontrolü hatası - SessionID: {SessionId}", sessionId);
                return ApiResponseDto<bool>.ErrorResult("Session validity kontrolü sırasında hata oluştu");
            }
        }
    }
}
