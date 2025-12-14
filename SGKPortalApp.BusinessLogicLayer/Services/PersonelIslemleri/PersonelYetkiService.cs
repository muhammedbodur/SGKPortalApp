using AutoMapper;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Interfaces.SignalR;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Services.PersonelIslemleri
{
    public class PersonelYetkiService : IPersonelYetkiService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<PersonelYetkiService> _logger;
        private readonly ISignalRBroadcaster _broadcaster;
        private readonly IHubConnectionRepository _hubConnectionRepository;

        public PersonelYetkiService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<PersonelYetkiService> logger,
            ISignalRBroadcaster broadcaster,
            IHubConnectionRepository hubConnectionRepository)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _broadcaster = broadcaster;
            _hubConnectionRepository = hubConnectionRepository;
        }

        private async Task<Guid?> TouchPermissionStampAsync(string tcKimlikNo)
        {
            try
            {
                var userRepo = _unitOfWork.Repository<User>();
                var user = await userRepo.FirstOrDefaultAsync(u => u.TcKimlikNo == tcKimlikNo);
                if (user == null)
                {
                    _logger.LogWarning("PermissionStamp güncellenecek User bulunamadı. TcKimlikNo: {TcKimlikNo}", tcKimlikNo);
                    return null;
                }

                user.PermissionStamp = Guid.NewGuid();
                user.DuzenlenmeTarihi = DateTime.Now;
                userRepo.Update(user);
                await _unitOfWork.SaveChangesAsync();
                return user.PermissionStamp;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PermissionStamp güncellenirken hata oluştu. TcKimlikNo: {TcKimlikNo}", tcKimlikNo);
                return null;
            }
        }

        private async Task BroadcastPermissionsChangedAsync(string tcKimlikNo, Guid? permissionStamp)
        {
            try
            {
                var connections = await _hubConnectionRepository.GetByUserAsync(tcKimlikNo);
                var connectionIds = connections
                    .Where(c => c.ConnectionStatus == ConnectionStatus.online && !c.SilindiMi)
                    .Select(c => c.ConnectionId)
                    .Distinct()
                    .ToList();

                if (!connectionIds.Any())
                    return;

                await _broadcaster.SendToConnectionsAsync(
                    connectionIds,
                    "permissionsChanged",
                    new
                    {
                        TcKimlikNo = tcKimlikNo,
                        PermissionStamp = permissionStamp,
                        Timestamp = DateTime.Now
                    });
            }
            catch (Exception ex)
            {
                // Broadcast hatası ana işlemi bozmamalı
                _logger.LogWarning(ex, "permissionsChanged broadcast hatası. TcKimlikNo: {TcKimlikNo}", tcKimlikNo);
            }
        }

        public async Task<ApiResponseDto<PersonelYetkiResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return ApiResponseDto<PersonelYetkiResponseDto>.ErrorResult("Geçersiz personel yetki ID");

                var repo = _unitOfWork.Repository<PersonelYetki>();
                var entity = await repo.FirstOrDefaultAsync(py => py.PersonelYetkiId == id, py => py.Yetki, py => py.ModulControllerIslem);

                if (entity == null)
                    return ApiResponseDto<PersonelYetkiResponseDto>.ErrorResult("Personel yetkisi bulunamadı");

                var dto = _mapper.Map<PersonelYetkiResponseDto>(entity);
                return ApiResponseDto<PersonelYetkiResponseDto>.SuccessResult(dto, "Personel yetkisi başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel yetkisi getirilirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<PersonelYetkiResponseDto>.ErrorResult("Personel yetkisi getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<PersonelYetkiResponseDto>>> GetByTcKimlikNoAsync(string tcKimlikNo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tcKimlikNo))
                    return ApiResponseDto<List<PersonelYetkiResponseDto>>.ErrorResult("TcKimlikNo boş olamaz");

                var repo = _unitOfWork.Repository<PersonelYetki>();
                var entities = await repo.FindAsync(py => py.TcKimlikNo == tcKimlikNo, py => py.Yetki, py => py.ModulControllerIslem);
                var dtos = _mapper.Map<List<PersonelYetkiResponseDto>>(entities);

                return ApiResponseDto<List<PersonelYetkiResponseDto>>.SuccessResult(dtos, "Personel yetkileri başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel yetkileri getirilirken hata oluştu. TC: {TcKimlikNo}", tcKimlikNo);
                return ApiResponseDto<List<PersonelYetkiResponseDto>>.ErrorResult("Personel yetkileri getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<PersonelYetkiResponseDto>>> GetByYetkiIdAsync(int yetkiId)
        {
            try
            {
                if (yetkiId <= 0)
                    return ApiResponseDto<List<PersonelYetkiResponseDto>>.ErrorResult("Geçersiz YetkiId");

                var repo = _unitOfWork.Repository<PersonelYetki>();
                var entities = await repo.FindAsync(py => py.YetkiId == yetkiId, py => py.Yetki, py => py.ModulControllerIslem);
                var dtos = _mapper.Map<List<PersonelYetkiResponseDto>>(entities);

                return ApiResponseDto<List<PersonelYetkiResponseDto>>.SuccessResult(dtos, "Yetkiye bağlı personel yetkileri başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Yetkiye bağlı personel yetkileri getirilirken hata oluştu. YetkiId: {YetkiId}", yetkiId);
                return ApiResponseDto<List<PersonelYetkiResponseDto>>.ErrorResult("Personel yetkileri getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<PersonelYetkiResponseDto>> CreateAsync(PersonelYetkiCreateRequestDto request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.TcKimlikNo))
                    return ApiResponseDto<PersonelYetkiResponseDto>.ErrorResult("TcKimlikNo boş olamaz");

                var repo = _unitOfWork.Repository<PersonelYetki>();
                var exists = await repo.ExistsAsync(py => py.TcKimlikNo == request.TcKimlikNo && py.YetkiId == request.YetkiId);
                if (exists)
                    return ApiResponseDto<PersonelYetkiResponseDto>.ErrorResult("Bu personele bu yetki zaten atanmış");

                var personel = await _unitOfWork.Repository<Personel>()
                    .FirstOrDefaultAsync(p => p.TcKimlikNo == request.TcKimlikNo);
                if (personel == null)
                    return ApiResponseDto<PersonelYetkiResponseDto>.ErrorResult("Personel bulunamadı");

                var yetki = await _unitOfWork.Repository<Yetki>().GetByIdAsync(request.YetkiId);
                if (yetki == null)
                    return ApiResponseDto<PersonelYetkiResponseDto>.ErrorResult("Yetki bulunamadı");

                var islem = await _unitOfWork.Repository<ModulControllerIslem>().GetByIdAsync(request.ModulControllerIslemId);
                if (islem == null)
                    return ApiResponseDto<PersonelYetkiResponseDto>.ErrorResult("ModulControllerIslem bulunamadı");

                var entity = _mapper.Map<PersonelYetki>(request);
                entity.Personel = personel;
                entity.Yetki = yetki;
                entity.ModulControllerIslem = islem;

                await repo.AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();

                var stamp = await TouchPermissionStampAsync(request.TcKimlikNo);
                await BroadcastPermissionsChangedAsync(request.TcKimlikNo, stamp);

                var saved = await repo.FirstOrDefaultAsync(py => py.PersonelYetkiId == entity.PersonelYetkiId, py => py.Yetki, py => py.ModulControllerIslem);
                if (saved == null)
                    return ApiResponseDto<PersonelYetkiResponseDto>.ErrorResult("Personel yetkisi oluşturuldu fakat tekrar okunamadı");

                var dto = _mapper.Map<PersonelYetkiResponseDto>(saved);

                _logger.LogInformation("Personel yetkisi oluşturuldu. ID: {Id}", entity.PersonelYetkiId);
                return ApiResponseDto<PersonelYetkiResponseDto>.SuccessResult(dto, "Personel yetkisi başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel yetkisi oluşturulurken hata oluştu");
                return ApiResponseDto<PersonelYetkiResponseDto>.ErrorResult("Personel yetkisi oluşturulurken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<PersonelYetkiResponseDto>> UpdateAsync(int id, PersonelYetkiUpdateRequestDto request)
        {
            try
            {
                if (id <= 0)
                    return ApiResponseDto<PersonelYetkiResponseDto>.ErrorResult("Geçersiz personel yetki ID");

                var repo = _unitOfWork.Repository<PersonelYetki>();
                var entity = await repo.GetByIdAsync(id);

                if (entity == null)
                    return ApiResponseDto<PersonelYetkiResponseDto>.ErrorResult("Personel yetkisi bulunamadı");

                var exists = await repo.ExistsAsync(py => py.PersonelYetkiId != id && py.TcKimlikNo == entity.TcKimlikNo && py.YetkiId == request.YetkiId);
                if (exists)
                    return ApiResponseDto<PersonelYetkiResponseDto>.ErrorResult("Bu personele bu yetki zaten atanmış");

                var yetki = await _unitOfWork.Repository<Yetki>().GetByIdAsync(request.YetkiId);
                if (yetki == null)
                    return ApiResponseDto<PersonelYetkiResponseDto>.ErrorResult("Yetki bulunamadı");

                var islem = await _unitOfWork.Repository<ModulControllerIslem>().GetByIdAsync(request.ModulControllerIslemId);
                if (islem == null)
                    return ApiResponseDto<PersonelYetkiResponseDto>.ErrorResult("ModulControllerIslem bulunamadı");

                _mapper.Map(request, entity);
                repo.Update(entity);
                await _unitOfWork.SaveChangesAsync();

                var stamp = await TouchPermissionStampAsync(entity.TcKimlikNo);
                await BroadcastPermissionsChangedAsync(entity.TcKimlikNo, stamp);

                var saved = await repo.FirstOrDefaultAsync(py => py.PersonelYetkiId == id, py => py.Yetki, py => py.ModulControllerIslem);
                if (saved == null)
                    return ApiResponseDto<PersonelYetkiResponseDto>.ErrorResult("Personel yetkisi güncellendi fakat tekrar okunamadı");

                var dto = _mapper.Map<PersonelYetkiResponseDto>(saved);

                _logger.LogInformation("Personel yetkisi güncellendi. ID: {Id}", id);
                return ApiResponseDto<PersonelYetkiResponseDto>.SuccessResult(dto, "Personel yetkisi başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel yetkisi güncellenirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<PersonelYetkiResponseDto>.ErrorResult("Personel yetkisi güncellenirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return ApiResponseDto<bool>.ErrorResult("Geçersiz personel yetki ID");

                var repo = _unitOfWork.Repository<PersonelYetki>();
                var entity = await repo.GetByIdAsync(id);

                if (entity == null)
                    return ApiResponseDto<bool>.ErrorResult("Personel yetkisi bulunamadı");

                var tcKimlikNo = entity.TcKimlikNo;

                repo.Delete(entity);
                await _unitOfWork.SaveChangesAsync();

                var stamp = await TouchPermissionStampAsync(tcKimlikNo);
                await BroadcastPermissionsChangedAsync(tcKimlikNo, stamp);

                _logger.LogInformation("Personel yetkisi silindi (soft delete). ID: {Id}", id);
                return ApiResponseDto<bool>.SuccessResult(true, "Personel yetkisi başarıyla silindi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel yetkisi silinirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<bool>.ErrorResult("Personel yetkisi silinirken bir hata oluştu", ex.Message);
            }
        }
    }
}
