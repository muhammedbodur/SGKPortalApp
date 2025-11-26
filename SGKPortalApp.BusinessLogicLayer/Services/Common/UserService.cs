using AutoMapper;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Exceptions;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common;

namespace SGKPortalApp.BusinessLogicLayer.Services.Common
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<UserService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponseDto<UserResponseDto>> GetByTcKimlikNoAsync(string tcKimlikNo)
        {
            try
            {
                var userRepo = _unitOfWork.GetRepository<IUserRepository>();
                var user = await userRepo.GetByTcKimlikNoAsync(tcKimlikNo);

                if (user == null)
                    return ApiResponseDto<UserResponseDto>.ErrorResult("Kullanıcı bulunamadı");

                var dto = _mapper.Map<UserResponseDto>(user);
                return ApiResponseDto<UserResponseDto>.SuccessResult(dto, "Kullanıcı başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kullanıcı getirilirken hata oluştu. TC: {TcKimlikNo}", tcKimlikNo);
                return ApiResponseDto<UserResponseDto>
                    .ErrorResult("Kullanıcı getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<UserResponseDto>>> GetAllAsync()
        {
            try
            {
                var userRepo = _unitOfWork.GetRepository<IUserRepository>();
                var users = await userRepo.GetAllAsync();

                var dtos = _mapper.Map<List<UserResponseDto>>(users);
                return ApiResponseDto<List<UserResponseDto>>
                    .SuccessResult(dtos, "Kullanıcılar başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kullanıcılar getirilirken hata oluştu");
                return ApiResponseDto<List<UserResponseDto>>
                    .ErrorResult("Kullanıcılar getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<UserResponseDto>>> GetActiveUsersAsync()
        {
            try
            {
                var userRepo = _unitOfWork.GetRepository<IUserRepository>();
                var users = await userRepo.GetActiveUsersAsync();

                var dtos = _mapper.Map<List<UserResponseDto>>(users);
                return ApiResponseDto<List<UserResponseDto>>
                    .SuccessResult(dtos, "Aktif kullanıcılar başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Aktif kullanıcılar getirilirken hata oluştu");
                return ApiResponseDto<List<UserResponseDto>>
                    .ErrorResult("Aktif kullanıcılar getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<UserResponseDto>>> GetLockedUsersAsync()
        {
            try
            {
                var userRepo = _unitOfWork.GetRepository<IUserRepository>();
                var users = await userRepo.GetLockedUsersAsync();

                var dtos = _mapper.Map<List<UserResponseDto>>(users);
                return ApiResponseDto<List<UserResponseDto>>
                    .SuccessResult(dtos, "Kilitli kullanıcılar başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kilitli kullanıcılar getirilirken hata oluştu");
                return ApiResponseDto<List<UserResponseDto>>
                    .ErrorResult("Kilitli kullanıcılar getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<UserResponseDto>> UpdateAsync(string tcKimlikNo, UserUpdateRequestDto request)
        {
            try
            {
                var userRepo = _unitOfWork.GetRepository<IUserRepository>();
                var user = await userRepo.GetByTcKimlikNoAsync(tcKimlikNo);

                if (user == null)
                    return ApiResponseDto<UserResponseDto>.ErrorResult("Kullanıcı bulunamadı");

                // UserUpdateRequestDto sadece AktifMi içeriyor
                _mapper.Map(request, user);
                userRepo.Update(user);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Kullanıcı güncellendi. TC: {TcKimlikNo}", tcKimlikNo);

                var dto = _mapper.Map<UserResponseDto>(user);
                return ApiResponseDto<UserResponseDto>
                    .SuccessResult(dto, "Kullanıcı başarıyla güncellendi");
            }
            catch (DatabaseException ex)
            {
                _logger.LogWarning(ex, "Veritabanı kısıtlama hatası: {ErrorType}", ex.ErrorType);
                return ApiResponseDto<UserResponseDto>
                    .ErrorResult(ex.UserFriendlyMessage, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kullanıcı güncellenirken hata oluştu. TC: {TcKimlikNo}", tcKimlikNo);
                return ApiResponseDto<UserResponseDto>
                    .ErrorResult("Kullanıcı güncellenirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteAsync(string tcKimlikNo)
        {
            try
            {
                var userRepo = _unitOfWork.GetRepository<IUserRepository>();
                var user = await userRepo.GetByTcKimlikNoAsync(tcKimlikNo);

                if (user == null)
                    return ApiResponseDto<bool>.ErrorResult("Kullanıcı bulunamadı");

                userRepo.Delete(user);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Kullanıcı silindi. TC: {TcKimlikNo}", tcKimlikNo);

                return ApiResponseDto<bool>.SuccessResult(true, "Kullanıcı başarıyla silindi");
            }
            catch (DatabaseException ex)
            {
                _logger.LogWarning(ex, "Veritabanı kısıtlama hatası: {ErrorType}", ex.ErrorType);
                return ApiResponseDto<bool>
                    .ErrorResult(ex.UserFriendlyMessage, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kullanıcı silinirken hata oluştu. TC: {TcKimlikNo}", tcKimlikNo);
                return ApiResponseDto<bool>
                    .ErrorResult("Kullanıcı silinirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> ChangePasswordAsync(string tcKimlikNo, string oldPassword, string newPassword)
        {
            try
            {
                var userRepo = _unitOfWork.GetRepository<IUserRepository>();
                var user = await userRepo.GetByTcKimlikNoAsync(tcKimlikNo);

                if (user == null)
                    return ApiResponseDto<bool>.ErrorResult("Kullanıcı bulunamadı");

                // Eski şifre kontrolü
                if (user.PassWord != oldPassword)
                    return ApiResponseDto<bool>.ErrorResult("Mevcut şifre hatalı");

                // Yeni şifre ataması
                user.PassWord = newPassword;
                userRepo.Update(user);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Şifre değiştirildi. TC: {TcKimlikNo}", tcKimlikNo);

                return ApiResponseDto<bool>.SuccessResult(true, "Şifre başarıyla değiştirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Şifre değiştirilirken hata oluştu. TC: {TcKimlikNo}", tcKimlikNo);
                return ApiResponseDto<bool>
                    .ErrorResult("Şifre değiştirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> ResetPasswordAsync(string tcKimlikNo)
        {
            try
            {
                var userRepo = _unitOfWork.GetRepository<IUserRepository>();
                var user = await userRepo.GetByTcKimlikNoAsync(tcKimlikNo);

                if (user == null)
                    return ApiResponseDto<bool>.ErrorResult("Kullanıcı bulunamadı");

                // Şifreyi TC Kimlik No'ya sıfırla
                user.PassWord = tcKimlikNo;
                user.BasarisizGirisSayisi = 0;
                userRepo.Update(user);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Şifre sıfırlandı. TC: {TcKimlikNo}", tcKimlikNo);

                return ApiResponseDto<bool>.SuccessResult(true, "Şifre TC Kimlik No'ya sıfırlandı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Şifre sıfırlanırken hata oluştu. TC: {TcKimlikNo}", tcKimlikNo);
                return ApiResponseDto<bool>
                    .ErrorResult("Şifre sıfırlanırken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> LockUserAsync(string tcKimlikNo)
        {
            try
            {
                var userRepo = _unitOfWork.GetRepository<IUserRepository>();
                await userRepo.LockUserAsync(tcKimlikNo);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Kullanıcı kilitlendi. TC: {TcKimlikNo}", tcKimlikNo);

                return ApiResponseDto<bool>.SuccessResult(true, "Kullanıcı başarıyla kilitlendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kullanıcı kilitlenirken hata oluştu. TC: {TcKimlikNo}", tcKimlikNo);
                return ApiResponseDto<bool>
                    .ErrorResult("Kullanıcı kilitlenirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> UnlockUserAsync(string tcKimlikNo)
        {
            try
            {
                var userRepo = _unitOfWork.GetRepository<IUserRepository>();
                await userRepo.UnlockUserAsync(tcKimlikNo);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Kullanıcı kilidi açıldı. TC: {TcKimlikNo}", tcKimlikNo);

                return ApiResponseDto<bool>.SuccessResult(true, "Kullanıcı kilidi başarıyla açıldı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kullanıcı kilidi açılırken hata oluştu. TC: {TcKimlikNo}", tcKimlikNo);
                return ApiResponseDto<bool>
                    .ErrorResult("Kullanıcı kilidi açılırken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> ActivateUserAsync(string tcKimlikNo)
        {
            try
            {
                var userRepo = _unitOfWork.GetRepository<IUserRepository>();
                var user = await userRepo.GetByTcKimlikNoAsync(tcKimlikNo);

                if (user == null)
                    return ApiResponseDto<bool>.ErrorResult("Kullanıcı bulunamadı");

                user.AktifMi = true;
                userRepo.Update(user);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Kullanıcı aktif edildi. TC: {TcKimlikNo}", tcKimlikNo);

                return ApiResponseDto<bool>.SuccessResult(true, "Kullanıcı başarıyla aktif edildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kullanıcı aktif edilirken hata oluştu. TC: {TcKimlikNo}", tcKimlikNo);
                return ApiResponseDto<bool>
                    .ErrorResult("Kullanıcı aktif edilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> DeactivateUserAsync(string tcKimlikNo)
        {
            try
            {
                var userRepo = _unitOfWork.GetRepository<IUserRepository>();
                var user = await userRepo.GetByTcKimlikNoAsync(tcKimlikNo);

                if (user == null)
                    return ApiResponseDto<bool>.ErrorResult("Kullanıcı bulunamadı");

                user.AktifMi = false;
                user.SessionID = null; // Oturumu kapat
                userRepo.Update(user);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Kullanıcı pasif edildi. TC: {TcKimlikNo}", tcKimlikNo);

                return ApiResponseDto<bool>.SuccessResult(true, "Kullanıcı başarıyla pasif edildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kullanıcı pasif edilirken hata oluştu. TC: {TcKimlikNo}", tcKimlikNo);
                return ApiResponseDto<bool>
                    .ErrorResult("Kullanıcı pasif edilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> ClearSessionAsync(string tcKimlikNo)
        {
            try
            {
                var userRepo = _unitOfWork.GetRepository<IUserRepository>();
                var user = await userRepo.GetByTcKimlikNoAsync(tcKimlikNo);

                if (user == null)
                    return ApiResponseDto<bool>.ErrorResult("Kullanıcı bulunamadı");

                user.SessionID = null;
                userRepo.Update(user);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Oturum temizlendi. TC: {TcKimlikNo}", tcKimlikNo);

                return ApiResponseDto<bool>.SuccessResult(true, "Oturum başarıyla temizlendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Oturum temizlenirken hata oluştu. TC: {TcKimlikNo}", tcKimlikNo);
                return ApiResponseDto<bool>
                    .ErrorResult("Oturum temizlenirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<UserResponseDto>> GetBySessionIdAsync(string sessionId)
        {
            try
            {
                var userRepo = _unitOfWork.GetRepository<IUserRepository>();
                var users = await userRepo.GetAllAsync();
                var user = users.FirstOrDefault(u => u.SessionID == sessionId);

                if (user == null)
                    return ApiResponseDto<UserResponseDto>.ErrorResult("Geçerli oturum bulunamadı");

                var dto = _mapper.Map<UserResponseDto>(user);
                return ApiResponseDto<UserResponseDto>.SuccessResult(dto, "Kullanıcı başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Session ID ile kullanıcı getirilirken hata oluştu");
                return ApiResponseDto<UserResponseDto>
                    .ErrorResult("Kullanıcı getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> ActivateBankoModeAsync(string tcKimlikNo, int bankoId)
        {
            try
            {
                var userRepo = _unitOfWork.GetRepository<IUserRepository>();
                var success = await userRepo.ActivateBankoModeAsync(tcKimlikNo, bankoId);

                if (!success)
                    return ApiResponseDto<bool>.ErrorResult("Kullanıcı bulunamadı");

                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Banko modu aktif edildi. TC: {TcKimlikNo}, BankoId: {BankoId}", tcKimlikNo, bankoId);

                return ApiResponseDto<bool>.SuccessResult(true, "Banko modu başarıyla aktif edildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Banko modu aktif edilirken hata oluştu. TC: {TcKimlikNo}", tcKimlikNo);
                return ApiResponseDto<bool>
                    .ErrorResult("Banko modu aktif edilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> DeactivateBankoModeAsync(string tcKimlikNo)
        {
            try
            {
                var userRepo = _unitOfWork.GetRepository<IUserRepository>();
                var success = await userRepo.DeactivateBankoModeAsync(tcKimlikNo);

                if (!success)
                    return ApiResponseDto<bool>.ErrorResult("Kullanıcı bulunamadı");

                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Banko modu deaktif edildi. TC: {TcKimlikNo}", tcKimlikNo);

                return ApiResponseDto<bool>.SuccessResult(true, "Banko modu başarıyla deaktif edildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Banko modu deaktif edilirken hata oluştu. TC: {TcKimlikNo}", tcKimlikNo);
                return ApiResponseDto<bool>
                    .ErrorResult("Banko modu deaktif edilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> IsBankoModeActiveAsync(string tcKimlikNo)
        {
            try
            {
                var userRepo = _unitOfWork.GetRepository<IUserRepository>();
                var isActive = await userRepo.IsBankoModeActiveAsync(tcKimlikNo);

                return ApiResponseDto<bool>.SuccessResult(isActive, "Banko modu durumu başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Banko modu durumu kontrol edilirken hata oluştu. TC: {TcKimlikNo}", tcKimlikNo);
                return ApiResponseDto<bool>
                    .ErrorResult("Banko modu durumu kontrol edilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<int?>> GetActiveBankoIdAsync(string tcKimlikNo)
        {
            try
            {
                var userRepo = _unitOfWork.GetRepository<IUserRepository>();
                var bankoId = await userRepo.GetActiveBankoIdAsync(tcKimlikNo);

                return ApiResponseDto<int?>.SuccessResult(bankoId, "Aktif banko ID başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Aktif banko ID getirilirken hata oluştu. TC: {TcKimlikNo}", tcKimlikNo);
                return ApiResponseDto<int?>
                    .ErrorResult("Aktif banko ID getirilirken bir hata oluştu", ex.Message);
            }
        }
    }
}
