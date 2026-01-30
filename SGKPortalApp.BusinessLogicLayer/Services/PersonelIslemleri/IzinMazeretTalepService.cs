using AutoMapper;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.Exceptions;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PdksIslemleri;
using SGKPortalApp.Common.Extensions;
using SGKPortalApp.BusinessObjectLayer.Entities.PdksIslemleri;
using SGKPortalApp.Common.Helpers;

namespace SGKPortalApp.BusinessLogicLayer.Services.PersonelIslemleri
{
    /// <summary>
    /// İzin/Mazeret talepleri business service
    /// Çakışma kontrolü, onay workflow ve raporlama içerir
    /// </summary>
    public class IzinMazeretTalepService : IIzinMazeretTalepService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<IzinMazeretTalepService> _logger;

        public IzinMazeretTalepService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<IzinMazeretTalepService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        // ═══════════════════════════════════════════════════════
        // CRUD İŞLEMLERİ
        // ═══════════════════════════════════════════════════════

        public async Task<ApiResponseDto<List<IzinMazeretTalepListResponseDto>>> GetAllAsync()
        {
            try
            {
                var talepler = await _unitOfWork.Repository<IzinMazeretTalep>().GetAllAsync();
                var taleplerDto = MapToListDto(talepler);

                return ApiResponseDto<List<IzinMazeretTalepListResponseDto>>
                    .SuccessResult(taleplerDto, "Talepler başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Talepler getirilirken hata oluştu");
                return ApiResponseDto<List<IzinMazeretTalepListResponseDto>>
                    .ErrorResult("Talepler getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<IzinMazeretTalepResponseDto>> GetByIdAsync(int id, string? currentUserTc = null)
        {
            try
            {
                var talep = await _unitOfWork.Repository<IzinMazeretTalep>().GetByIdAsync(id);

                if (talep == null)
                    return ApiResponseDto<IzinMazeretTalepResponseDto>.ErrorResult("Talep bulunamadı");

                // 🔒 OWNERSHIP KONTROLÜ
                // Eğer currentUserTc belirtilmişse, kaydın sahibi olup olmadığını kontrol et
                if (!string.IsNullOrEmpty(currentUserTc) && talep.TcKimlikNo != currentUserTc)
                {
                    _logger.LogWarning(
                        "⚠️ SECURITY: Yetkisiz erişim denemesi! Talep ID: {TalepId}, Sahip: {SahipTc}, İsteyen: {IsteyenTc}",
                        id, talep.TcKimlikNo, currentUserTc);

                    return ApiResponseDto<IzinMazeretTalepResponseDto>
                        .ErrorResult("Bu kaydı görüntüleme yetkiniz yok");
                }

                var talepDto = MapToResponseDto(talep);
                return ApiResponseDto<IzinMazeretTalepResponseDto>
                    .SuccessResult(talepDto, "Talep başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Talep getirilirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<IzinMazeretTalepResponseDto>
                    .ErrorResult("Talep getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<IzinMazeretTalepResponseDto>> CreateAsync(IzinMazeretTalepCreateRequestDto request)
        {
            try
            {
                // Personel kontrolü
                var personel = await _unitOfWork.Repository<Personel>().GetByIdAsync(request.TcKimlikNo);
                if (personel == null)
                    return ApiResponseDto<IzinMazeretTalepResponseDto>.ErrorResult("Personel bulunamadı");

                // İzin türü kontrolü
                var izinTuruRepository = _unitOfWork.GetRepository<IIzinMazeretTuruTanimRepository>();
                var izinTuru = await izinTuruRepository.GetByIdAsync(request.IzinMazeretTuruId);
                if (izinTuru == null)
                    return ApiResponseDto<IzinMazeretTalepResponseDto>.ErrorResult("İzin türü bulunamadı");

                // Türe göre validasyon
                var validationResult = ValidateRequestByType(izinTuru.PlanliIzinMi, request.BaslangicTarihi, request.BitisTarihi, request.MazeretTarihi, request.BaslangicSaati, request.BitisSaati);
                if (!string.IsNullOrEmpty(validationResult))
                    return ApiResponseDto<IzinMazeretTalepResponseDto>.ErrorResult(validationResult);

                // ÖNEMLİ: Çakışma kontrolü
                var overlapCheck = await CheckOverlapAsync(
                    request.TcKimlikNo,
                    request.BaslangicTarihi,
                    request.BitisTarihi,
                    request.MazeretTarihi,
                    null);

                if (overlapCheck.Data)
                {
                    return ApiResponseDto<IzinMazeretTalepResponseDto>
                        .ErrorResult("Bu tarih aralığında zaten bir izin/mazeret kaydı bulunmaktadır. Lütfen çakışan kayıtları kontrol edin.");
                }

                // Talep oluştur
                var talep = _mapper.Map<IzinMazeretTalep>(request);
                talep.TalepTarihi = DateTimeHelper.Now;
                talep.IsActive = true;

                // Toplam gün hesapla (İzin için)
                if (request.BaslangicTarihi.HasValue && request.BitisTarihi.HasValue)
                {
                    talep.ToplamGun = (request.BitisTarihi.Value - request.BaslangicTarihi.Value).Days + 1;
                }

                // Onaycı atama mantığı
                await AssignApproversAsync(talep, personel);

                await _unitOfWork.Repository<IzinMazeretTalep>().AddAsync(talep);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("İzin/Mazeret talebi oluşturuldu. ID: {Id}, TC: {Tc}, Tür ID: {TurId}",
                    talep.IzinMazeretTalepId, talep.TcKimlikNo, talep.IzinMazeretTuruId);

                var talepDto = MapToResponseDto(talep);
                return ApiResponseDto<IzinMazeretTalepResponseDto>
                    .SuccessResult(talepDto, "Talep başarıyla oluşturuldu");
            }
            catch (DatabaseException ex)
            {
                _logger.LogWarning(ex, "Veritabanı kısıtlama hatası: {ErrorType}", ex.ErrorType);
                return ApiResponseDto<IzinMazeretTalepResponseDto>
                    .ErrorResult(ex.UserFriendlyMessage, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Talep oluşturulurken hata oluştu");
                return ApiResponseDto<IzinMazeretTalepResponseDto>
                    .ErrorResult("Talep oluşturulurken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<IzinMazeretTalepResponseDto>> UpdateAsync(int id, IzinMazeretTalepUpdateRequestDto request, string? currentUserTc = null)
        {
            try
            {
                var talep = await _unitOfWork.Repository<IzinMazeretTalep>().GetByIdAsync(id);

                if (talep == null)
                    return ApiResponseDto<IzinMazeretTalepResponseDto>.ErrorResult("Talep bulunamadı");

                // 🔒 OWNERSHIP KONTROLÜ
                // Eğer currentUserTc belirtilmişse, kaydın sahibi olup olmadığını kontrol et
                if (!string.IsNullOrEmpty(currentUserTc) && talep.TcKimlikNo != currentUserTc)
                {
                    _logger.LogWarning(
                        "⚠️ SECURITY: Yetkisiz güncelleme denemesi! Talep ID: {TalepId}, Sahip: {SahipTc}, İsteyen: {IsteyenTc}",
                        id, talep.TcKimlikNo, currentUserTc);

                    return ApiResponseDto<IzinMazeretTalepResponseDto>
                        .ErrorResult("Bu kaydı düzenleme yetkiniz yok");
                }

                // Sadece beklemedeki talepler güncellenebilir
                if (talep.BirinciOnayDurumu != OnayDurumu.Beklemede)
                {
                    return ApiResponseDto<IzinMazeretTalepResponseDto>
                        .ErrorResult("Sadece beklemede olan talepler güncellenebilir");
                }

                // İzin türü kontrolü
                var izinTuruRepository = _unitOfWork.GetRepository<IIzinMazeretTuruTanimRepository>();
                var izinTuru = await izinTuruRepository.GetByIdAsync(request.IzinMazeretTuruId);
                if (izinTuru == null)
                    return ApiResponseDto<IzinMazeretTalepResponseDto>.ErrorResult("İzin türü bulunamadı");

                // Validasyon
                var validationResult = ValidateRequestByType(izinTuru.PlanliIzinMi, request.BaslangicTarihi, request.BitisTarihi, request.MazeretTarihi, request.BaslangicSaati, request.BitisSaati);
                if (!string.IsNullOrEmpty(validationResult))
                    return ApiResponseDto<IzinMazeretTalepResponseDto>.ErrorResult(validationResult);

                // Çakışma kontrolü
                var overlapCheck = await CheckOverlapAsync(
                    talep.TcKimlikNo,
                    request.BaslangicTarihi,
                    request.BitisTarihi,
                    request.MazeretTarihi,
                    id);

                if (overlapCheck.Data)
                {
                    return ApiResponseDto<IzinMazeretTalepResponseDto>
                        .ErrorResult("Bu tarih aralığında zaten bir izin/mazeret kaydı bulunmaktadır.");
                }

                // Güncelle
                _mapper.Map(request, talep);

                // Toplam gün hesapla
                if (request.BaslangicTarihi.HasValue && request.BitisTarihi.HasValue)
                {
                    talep.ToplamGun = (request.BitisTarihi.Value - request.BaslangicTarihi.Value).Days + 1;
                }

                _unitOfWork.Repository<IzinMazeretTalep>().Update(talep);
                await _unitOfWork.SaveChangesAsync();

                var talepDto = MapToResponseDto(talep);
                return ApiResponseDto<IzinMazeretTalepResponseDto>
                    .SuccessResult(talepDto, "Talep başarıyla güncellendi");
            }
            catch (DatabaseException ex)
            {
                _logger.LogWarning(ex, "Veritabanı kısıtlama hatası: {ErrorType}", ex.ErrorType);
                return ApiResponseDto<IzinMazeretTalepResponseDto>
                    .ErrorResult(ex.UserFriendlyMessage, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Talep güncellenirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<IzinMazeretTalepResponseDto>
                    .ErrorResult("Talep güncellenirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteAsync(int id, string? currentUserTc = null)
        {
            try
            {
                var talep = await _unitOfWork.Repository<IzinMazeretTalep>().GetByIdAsync(id);

                if (talep == null)
                    return ApiResponseDto<bool>.ErrorResult("Talep bulunamadı");

                // 🔒 OWNERSHIP KONTROLÜ
                // Eğer currentUserTc belirtilmişse, kaydın sahibi olup olmadığını kontrol et
                if (!string.IsNullOrEmpty(currentUserTc) && talep.TcKimlikNo != currentUserTc)
                {
                    _logger.LogWarning(
                        "⚠️ SECURITY: Yetkisiz silme denemesi! Talep ID: {TalepId}, Sahip: {SahipTc}, İsteyen: {IsteyenTc}",
                        id, talep.TcKimlikNo, currentUserTc);

                    return ApiResponseDto<bool>
                        .ErrorResult("Bu kaydı silme yetkiniz yok");
                }

                _unitOfWork.Repository<IzinMazeretTalep>().Delete(talep);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponseDto<bool>.SuccessResult(true, "Talep başarıyla silindi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Talep silinirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<bool>.ErrorResult("Talep silinirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> CancelAsync(int id, string iptalNedeni)
        {
            try
            {
                var talep = await _unitOfWork.Repository<IzinMazeretTalep>().GetByIdAsync(id);

                if (talep == null)
                    return ApiResponseDto<bool>.ErrorResult("Talep bulunamadı");

                talep.IsActive = false;
                talep.BirinciOnayDurumu = OnayDurumu.IptalEdildi;
                talep.BirinciOnayAciklama = $"Talep iptal edildi. Neden: {iptalNedeni}";
                talep.BirinciOnayTarihi = DateTimeHelper.Now;

                _unitOfWork.Repository<IzinMazeretTalep>().Update(talep);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponseDto<bool>.SuccessResult(true, "Talep başarıyla iptal edildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Talep iptal edilirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<bool>.ErrorResult("Talep iptal edilirken bir hata oluştu", ex.Message);
            }
        }

        // ═══════════════════════════════════════════════════════
        // PERSONEL BAZINDA SORGULAR
        // ═══════════════════════════════════════════════════════

        public async Task<ApiResponseDto<List<IzinMazeretTalepListResponseDto>>> GetByPersonelTcAsync(string tcKimlikNo, bool includeInactive = false)
        {
            try
            {
                var repository = _unitOfWork.GetRepository<IIzinMazeretTalepRepository>();
                var talepler = await repository.GetByPersonelTcAsync(tcKimlikNo, includeInactive);
                var taleplerDto = MapToListDto(talepler);

                return ApiResponseDto<List<IzinMazeretTalepListResponseDto>>
                    .SuccessResult(taleplerDto, "Talepler başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel talepleri getirilirken hata oluştu. TC: {Tc}", tcKimlikNo);
                return ApiResponseDto<List<IzinMazeretTalepListResponseDto>>
                    .ErrorResult("Talepler getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<IzinMazeretTalepListResponseDto>>> GetPendingByPersonelTcAsync(string tcKimlikNo)
        {
            try
            {
                var repository = _unitOfWork.GetRepository<IIzinMazeretTalepRepository>();
                var talepler = await repository.GetPendingByPersonelTcAsync(tcKimlikNo);
                var taleplerDto = MapToListDto(talepler);

                return ApiResponseDto<List<IzinMazeretTalepListResponseDto>>
                    .SuccessResult(taleplerDto, "Bekleyen talepler başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bekleyen talepler getirilirken hata oluştu. TC: {Tc}", tcKimlikNo);
                return ApiResponseDto<List<IzinMazeretTalepListResponseDto>>
                    .ErrorResult("Talepler getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<IzinMazeretTalepListResponseDto>>> GetApprovedByPersonelTcAsync(
            string tcKimlikNo,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            try
            {
                var repository = _unitOfWork.GetRepository<IIzinMazeretTalepRepository>();
                var talepler = await repository.GetApprovedByPersonelTcAsync(tcKimlikNo, startDate, endDate);
                var taleplerDto = MapToListDto(talepler);

                return ApiResponseDto<List<IzinMazeretTalepListResponseDto>>
                    .SuccessResult(taleplerDto, "Onaylanmış talepler başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Onaylanmış talepler getirilirken hata oluştu. TC: {Tc}", tcKimlikNo);
                return ApiResponseDto<List<IzinMazeretTalepListResponseDto>>
                    .ErrorResult("Talepler getirilirken bir hata oluştu", ex.Message);
            }
        }

        // ═══════════════════════════════════════════════════════
        // ONAY İŞLEMLERİ
        // ═══════════════════════════════════════════════════════

        public async Task<ApiResponseDto<bool>> ApproveOrRejectAsync(
            int talepId,
            string onayciTcKimlikNo,
            IzinMazeretTalepOnayRequestDto request)
        {
            try
            {
                var talep = await _unitOfWork.Repository<IzinMazeretTalep>().GetByIdAsync(talepId);

                if (talep == null)
                    return ApiResponseDto<bool>.ErrorResult("Talep bulunamadı");

                // Onayci kontrolü
                bool isFirstApprover = talep.BirinciOnayciTcKimlikNo == onayciTcKimlikNo;
                bool isSecondApprover = talep.IkinciOnayciTcKimlikNo == onayciTcKimlikNo;

                if (!isFirstApprover && !isSecondApprover)
                {
                    return ApiResponseDto<bool>.ErrorResult("Bu talebi onaylama yetkiniz yok");
                }

                // Seviyeye göre işlem
                if (request.OnayciSeviyesi == 1)
                {
                    if (!isFirstApprover)
                        return ApiResponseDto<bool>.ErrorResult("1. onayci değilsiniz");

                    talep.BirinciOnayDurumu = request.OnayDurumu;
                    talep.BirinciOnayTarihi = DateTimeHelper.Now;
                    talep.BirinciOnayAciklama = request.Aciklama;
                }
                else if (request.OnayciSeviyesi == 2)
                {
                    if (!isSecondApprover)
                        return ApiResponseDto<bool>.ErrorResult("2. onayci değilsiniz");

                    // 1. onay geçmiş olmalı
                    if (talep.BirinciOnayDurumu != OnayDurumu.Onaylandi)
                        return ApiResponseDto<bool>.ErrorResult("1. onay henüz tamamlanmamış");

                    talep.IkinciOnayDurumu = request.OnayDurumu;
                    talep.IkinciOnayTarihi = DateTimeHelper.Now;
                    talep.IkinciOnayAciklama = request.Aciklama;
                }

                _unitOfWork.Repository<IzinMazeretTalep>().Update(talep);
                await _unitOfWork.SaveChangesAsync();

                var action = request.OnayDurumu == OnayDurumu.Onaylandi ? "onaylandı" : "reddedildi";
                return ApiResponseDto<bool>.SuccessResult(true, $"Talep başarıyla {action}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Onay işlemi sırasında hata oluştu. Talep ID: {Id}", talepId);
                return ApiResponseDto<bool>.ErrorResult("Onay işlemi sırasında bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<IzinMazeretTalepListResponseDto>>> GetPendingForFirstApproverAsync(string onayciTcKimlikNo)
        {
            try
            {
                var repository = _unitOfWork.GetRepository<IIzinMazeretTalepRepository>();
                var talepler = await repository.GetPendingForFirstApproverAsync(onayciTcKimlikNo);
                var taleplerDto = MapToListDto(talepler);

                return ApiResponseDto<List<IzinMazeretTalepListResponseDto>>
                    .SuccessResult(taleplerDto, "1. onay bekleyen talepler başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "1. onay talepler getirilirken hata oluştu");
                return ApiResponseDto<List<IzinMazeretTalepListResponseDto>>
                    .ErrorResult("Talepler getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<IzinMazeretTalepListResponseDto>>> GetPendingForSecondApproverAsync(string onayciTcKimlikNo)
        {
            try
            {
                var repository = _unitOfWork.GetRepository<IIzinMazeretTalepRepository>();
                var talepler = await repository.GetPendingForSecondApproverAsync(onayciTcKimlikNo);
                var taleplerDto = MapToListDto(talepler);

                return ApiResponseDto<List<IzinMazeretTalepListResponseDto>>
                    .SuccessResult(taleplerDto, "2. onay bekleyen talepler başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "2. onay talepler getirilirken hata oluştu");
                return ApiResponseDto<List<IzinMazeretTalepListResponseDto>>
                    .ErrorResult("Talepler getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<IzinMazeretTalepListResponseDto>>> GetPendingByDepartmanAsync(int departmanId)
        {
            try
            {
                var repository = _unitOfWork.GetRepository<IIzinMazeretTalepRepository>();
                var talepler = await repository.GetPendingByDepartmanAsync(departmanId);
                var taleplerDto = MapToListDto(talepler);

                return ApiResponseDto<List<IzinMazeretTalepListResponseDto>>
                    .SuccessResult(taleplerDto, "Departman talepleri başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Departman talepleri getirilirken hata oluştu. Departman ID: {Id}", departmanId);
                return ApiResponseDto<List<IzinMazeretTalepListResponseDto>>
                    .ErrorResult("Talepler getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<IzinMazeretTalepListResponseDto>>> GetPendingByServisAsync(int servisId)
        {
            try
            {
                var repository = _unitOfWork.GetRepository<IIzinMazeretTalepRepository>();
                var talepler = await repository.GetPendingByServisAsync(servisId);
                var taleplerDto = MapToListDto(talepler);

                return ApiResponseDto<List<IzinMazeretTalepListResponseDto>>
                    .SuccessResult(taleplerDto, "Servis talepleri başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Servis talepleri getirilirken hata oluştu. Servis ID: {Id}", servisId);
                return ApiResponseDto<List<IzinMazeretTalepListResponseDto>>
                    .ErrorResult("Talepler getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri.PersonelResponseDto>>> GetAvailableApproversAsync(string tcKimlikNo)
        {
            try
            {
                // Kullanıcının bilgilerini al
                var personelRepo = _unitOfWork.GetRepository<IPersonelRepository>();
                var currentPersonel = await personelRepo.GetByTcKimlikNoAsync(tcKimlikNo);

                if (currentPersonel == null)
                {
                    return ApiResponseDto<List<SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri.PersonelResponseDto>>
                        .ErrorResult("Personel bulunamadı");
                }

                var currentDepartmanId = currentPersonel.DepartmanId;
                var currentServisId = currentPersonel.ServisId;

                // Tüm aktif personelleri getir
                var allPersonel = await personelRepo.GetActiveAsync();

                // Filtreleme kuralları:
                // 1. Unvan ID = 7: Tüm departmanlarda görünür
                // 2. Servis ID = 33: Sadece kendi departmanında görünür
                // 3. Unvan ID IN (5,87): Aynı departman VE aynı servis

                var availableApprovers = allPersonel
                    .Where(p => p.TcKimlikNo != tcKimlikNo) // Kendisi hariç
                    .Where(p => p.PersonelAktiflikDurum == SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri.PersonelAktiflikDurum.Aktif) // Sadece aktif personeller
                    .Where(p =>
                        // Kural 1: Unvan ID = 7 (SG İL MÜDÜR YARDIMCISI) - Tüm departmanlarda
                        (p.UnvanId == 7) ||
                        // Kural 2: Servis ID = 33 (İDARE) - Sadece kendi departmanında
                        (p.ServisId == 33 && p.DepartmanId == currentDepartmanId) ||
                        // Kural 3: Unvan ID IN (5,87) (ŞEF, ŞEF V.) - Aynı departman VE aynı servis
                        ((p.UnvanId == 5 || p.UnvanId == 87) && 
                         p.DepartmanId == currentDepartmanId && 
                         p.ServisId == currentServisId)
                    )
                    .OrderBy(p => p.AdSoyad)
                    .ToList();

                // PersonelResponseDto'ya map et
                var approverDtos = availableApprovers.Select(p => new SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri.PersonelResponseDto
                {
                    TcKimlikNo = p.TcKimlikNo,
                    AdSoyad = p.AdSoyad,
                    SicilNo = p.SicilNo ?? 0,
                    DepartmanId = p.DepartmanId,
                    DepartmanAdi = p.Departman?.DepartmanAdi ?? string.Empty,
                    ServisId = p.ServisId,
                    ServisAdi = p.Servis?.ServisAdi ?? string.Empty,
                    UnvanId = p.UnvanId,
                    UnvanAdi = p.Unvan?.UnvanAdi ?? string.Empty,
                    Email = p.Email,
                    PersonelAktiflikDurum = p.PersonelAktiflikDurum,
                    EklenmeTarihi = p.EklenmeTarihi,
                    DuzenlenmeTarihi = p.DuzenlenmeTarihi
                }).ToList();

                return ApiResponseDto<List<SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri.PersonelResponseDto>>
                    .SuccessResult(approverDtos, $"{approverDtos.Count} adet onaycı bulundu");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Onaycılar getirilirken hata oluştu. TC: {Tc}", tcKimlikNo);
                return ApiResponseDto<List<SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri.PersonelResponseDto>>
                    .ErrorResult("Onaycılar getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<IzinMazeretTuruResponseDto>>> GetAvailableLeaveTypesAsync()
        {
            try
            {
                var repository = _unitOfWork.GetRepository<IIzinMazeretTuruTanimRepository>();
                var turuTanimlari = await repository.GetAllActiveAsync();

                var turuDtos = turuTanimlari.Select(t => new IzinMazeretTuruResponseDto
                {
                    IzinMazeretTuruId = t.IzinMazeretTuruId,
                    TuruAdi = t.TuruAdi,
                    KisaKod = t.KisaKod,
                    Aciklama = t.Aciklama,
                    BirinciOnayciGerekli = t.BirinciOnayciGerekli,
                    IkinciOnayciGerekli = t.IkinciOnayciGerekli,
                    PlanliIzinMi = t.PlanliIzinMi,
                    Sira = t.Sira,
                    IsActive = t.IsActive,
                    RenkKodu = t.RenkKodu
                }).ToList();

                return ApiResponseDto<List<IzinMazeretTuruResponseDto>>
                    .SuccessResult(turuDtos, $"{turuDtos.Count} adet izin türü bulundu");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İzin türleri getirilirken hata oluştu");
                return ApiResponseDto<List<IzinMazeretTuruResponseDto>>
                    .ErrorResult("İzin türleri getirilirken bir hata oluştu", ex.Message);
            }
        }

        // ═══════════════════════════════════════════════════════
        // RAPORLAMA VE FİLTRELEME
        // ═══════════════════════════════════════════════════════

        public async Task<ApiResponseDto<IzinMazeretTalepFilterResponseDto>> GetFilteredAsync(
            IzinMazeretTalepFilterRequestDto filter)
        {
            try
            {
                var repository = _unitOfWork.GetRepository<IIzinMazeretTalepRepository>();

                var (items, totalCount) = await repository.GetFilteredAsync(
                    filter.TcKimlikNo,
                    filter.DepartmanId,
                    filter.ServisId,
                    filter.IzinMazeretTuruId,
                    filter.BirinciOnayDurumu,
                    filter.IkinciOnayDurumu,
                    filter.BaslangicTarihiMin,
                    filter.BaslangicTarihiMax,
                    filter.TalepTarihiMin,
                    filter.TalepTarihiMax,
                    filter.IsActive,
                    filter.IzinIslendiMi,
                    filter.PageNumber,
                    filter.PageSize,
                    filter.SortBy,
                    filter.SortDescending);

                var taleplerDto = MapToListDto(items);

                var response = new IzinMazeretTalepFilterResponseDto
                {
                    Items = taleplerDto,
                    TotalCount = totalCount
                };

                return ApiResponseDto<IzinMazeretTalepFilterResponseDto>
                    .SuccessResult(response, "Filtrelenmiş talepler başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Filtrelenmiş talepler getirilirken hata oluştu");
                return ApiResponseDto<IzinMazeretTalepFilterResponseDto>
                    .ErrorResult("Talepler getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<IzinMazeretTalepListResponseDto>>> GetByDateRangeAsync(
            DateTime startDate,
            DateTime endDate,
            int? departmanId = null,
            int? servisId = null)
        {
            try
            {
                var repository = _unitOfWork.GetRepository<IIzinMazeretTalepRepository>();
                var talepler = await repository.GetByDateRangeAsync(startDate, endDate, departmanId, servisId);
                var taleplerDto = MapToListDto(talepler);

                return ApiResponseDto<List<IzinMazeretTalepListResponseDto>>
                    .SuccessResult(taleplerDto, "Tarih aralığındaki talepler başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Tarih aralığındaki talepler getirilirken hata oluştu");
                return ApiResponseDto<List<IzinMazeretTalepListResponseDto>>
                    .ErrorResult("Talepler getirilirken bir hata oluştu", ex.Message);
            }
        }

        // ═══════════════════════════════════════════════════════
        // İSTATİSTİKLER
        // ═══════════════════════════════════════════════════════

        public async Task<ApiResponseDto<int>> GetTotalYillikIzinDaysAsync(string tcKimlikNo, int year)
        {
            try
            {
                var repository = _unitOfWork.GetRepository<IIzinMazeretTalepRepository>();
                var totalDays = await repository.GetTotalYillikIzinDaysAsync(tcKimlikNo, year);

                return ApiResponseDto<int>.SuccessResult(totalDays, "Yıllık izin günü başarıyla hesaplandı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Yıllık izin hesaplanırken hata oluştu. TC: {Tc}, Yıl: {Year}", tcKimlikNo, year);
                return ApiResponseDto<int>.ErrorResult("İzin günü hesaplanırken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<int>> GetTotalUsedDaysAsync(
            string tcKimlikNo,
            int? izinTuruValue = null,
            int? year = null)
        {
            try
            {
                var repository = _unitOfWork.GetRepository<IIzinMazeretTalepRepository>();
                var totalDays = await repository.GetTotalUsedDaysAsync(tcKimlikNo, izinTuruValue, year);

                return ApiResponseDto<int>.SuccessResult(totalDays, "Toplam kullanılan gün başarıyla hesaplandı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Toplam gün hesaplanırken hata oluştu. TC: {Tc}", tcKimlikNo);
                return ApiResponseDto<int>.ErrorResult("Gün hesaplanırken bir hata oluştu", ex.Message);
            }
        }

        // ═══════════════════════════════════════════════════════
        // ÇAKIŞMA KONTROLÜ
        // ═══════════════════════════════════════════════════════

        public async Task<ApiResponseDto<bool>> CheckOverlapAsync(
            string tcKimlikNo,
            DateTime? baslangicTarihi,
            DateTime? bitisTarihi,
            DateTime? mazeretTarihi,
            int? excludeTalepId = null)
        {
            try
            {
                var repository = _unitOfWork.GetRepository<IIzinMazeretTalepRepository>();
                var hasOverlap = await repository.HasOverlappingRequestAsync(
                    tcKimlikNo,
                    baslangicTarihi,
                    bitisTarihi,
                    mazeretTarihi,
                    excludeTalepId);

                return ApiResponseDto<bool>.SuccessResult(hasOverlap);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Çakışma kontrolü sırasında hata oluştu");
                return ApiResponseDto<bool>.ErrorResult("Çakışma kontrolü sırasında bir hata oluştu", ex.Message);
            }
        }

        // ═══════════════════════════════════════════════════════
        // HELPER METHODS
        // ═══════════════════════════════════════════════════════

        private string ValidateRequestByType(
            bool planliIzinMi,
            DateTime? baslangicTarihi,
            DateTime? bitisTarihi,
            DateTime? mazeretTarihi,
            TimeSpan? baslangicSaati,
            TimeSpan? bitisSaati)
        {
            // Mazeret ise (planlı izin değilse)
            if (!planliIzinMi)
            {
                if (!mazeretTarihi.HasValue)
                    return "Mazeret için tarih zorunludur";
                if (!baslangicSaati.HasValue || !bitisSaati.HasValue)
                    return "Mazeret için başlangıç ve bitiş saati zorunludur";
                if (baslangicSaati.Value >= bitisSaati.Value)
                    return "Başlangıç saati bitiş saatinden önce olmalıdır";
            }
            // İzin ise (planlı izin)
            else
            {
                if (!baslangicTarihi.HasValue || !bitisTarihi.HasValue)
                    return "İzin için başlangıç ve bitiş tarihi zorunludur";
                if (baslangicTarihi.Value > bitisTarihi.Value)
                    return "Başlangıç tarihi bitiş tarihinden sonra olamaz";
            }

            return string.Empty;
        }

        private IzinMazeretTalepResponseDto MapToResponseDto(IzinMazeretTalep talep)
        {
            return new IzinMazeretTalepResponseDto
            {
                IzinMazeretTalepId = talep.IzinMazeretTalepId,
                TcKimlikNo = talep.TcKimlikNo,
                AdSoyad = talep.Personel?.AdSoyad ?? "",
                SicilNo = talep.Personel?.SicilNo ?? 0,
                DepartmanAdi = talep.Personel?.Departman?.DepartmanAdi,
                ServisAdi = talep.Personel?.Servis?.ServisAdi,
                IzinMazeretTuruId = talep.IzinMazeretTuruId,
                TuruAdi = talep.IzinMazeretTuru?.TuruAdi ?? "",
                Aciklama = talep.Aciklama,
                TalepTarihi = talep.TalepTarihi,
                IsActive = talep.IsActive,
                BaslangicTarihi = talep.BaslangicTarihi,
                BitisTarihi = talep.BitisTarihi,
                ToplamGun = talep.ToplamGun,
                MazeretTarihi = talep.MazeretTarihi,
                BaslangicSaati = talep.BaslangicSaati,
                BitisSaati = talep.BitisSaati,
                BirinciOnayciTcKimlikNo = talep.BirinciOnayciTcKimlikNo,
                BirinciOnayDurumu = talep.BirinciOnayDurumu,
                BirinciOnayDurumuAdi = talep.BirinciOnayDurumu.GetDescription(),
                BirinciOnayTarihi = talep.BirinciOnayTarihi,
                BirinciOnayAciklama = talep.BirinciOnayAciklama,
                IkinciOnayciTcKimlikNo = talep.IkinciOnayciTcKimlikNo,
                IkinciOnayDurumu = talep.IkinciOnayDurumu,
                IkinciOnayDurumuAdi = talep.IkinciOnayDurumu.GetDescription(),
                IkinciOnayTarihi = talep.IkinciOnayTarihi,
                IkinciOnayAciklama = talep.IkinciOnayAciklama,
                EklenmeTarihi = talep.EklenmeTarihi,
                EkleyenKullanici = talep.EkleyenKullanici,
                DuzenlenmeTarihi = talep.DuzenlenmeTarihi,
                DuzenleyenKullanici = talep.DuzenleyenKullanici
            };
        }

        private List<IzinMazeretTalepListResponseDto> MapToListDto(IEnumerable<IzinMazeretTalep> talepler)
        {
            // AutoMapper kullanarak map et
            var dtoList = _mapper.Map<List<IzinMazeretTalepListResponseDto>>(talepler);
            
            // GenelDurum'u manuel hesapla (AutoMapper'da Ignore edildi)
            foreach (var dto in dtoList)
            {
                var talep = talepler.FirstOrDefault(t => t.IzinMazeretTalepId == dto.IzinMazeretTalepId);
                if (talep != null)
                {
                    dto.GenelDurum = GetGenelDurum(talep);
                }
            }
            
            return dtoList;
        }

        private string GetGenelDurum(IzinMazeretTalep talep)
        {
            if (!talep.IsActive)
                return "İptal";

            if (talep.BirinciOnayDurumu == OnayDurumu.Reddedildi || talep.IkinciOnayDurumu == OnayDurumu.Reddedildi)
                return "Reddedildi";

            if (talep.BirinciOnayDurumu == OnayDurumu.Onaylandi && talep.IkinciOnayDurumu == OnayDurumu.Onaylandi)
                return "Onaylandı";

            if (talep.BirinciOnayDurumu == OnayDurumu.Beklemede)
                return "1. Onay Bekliyor";

            if (talep.IkinciOnayDurumu == OnayDurumu.Beklemede)
                return "2. Onay Bekliyor";

            return "Beklemede";
        }

        // ═══════════════════════════════════════════════════════
        // ONAYCI ATAMA MANTĞI
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Talep için onaycıları otomatik atar
        /// İzin: 1. VE 2. onaycı gerekli (sıralı onay)
        /// Mazeret: 1. VEYA 2. onaycıdan biri yeterli (tek onay)
        /// </summary>
        private async Task AssignApproversAsync(IzinMazeretTalep talep, Personel personel)
        {
            try
            {
                // Manuel atanmışsa, otomatik atama yapma
                if (!string.IsNullOrEmpty(talep.BirinciOnayciTcKimlikNo) || 
                    !string.IsNullOrEmpty(talep.IkinciOnayciTcKimlikNo))
                {
                    _logger.LogInformation("Onaycılar manuel atandı, otomatik atama yapılmadı");
                    return;
                }

                var izinSorumluRepo = _unitOfWork.Repository<IzinSorumlu>();
                var allSorumlular = await izinSorumluRepo.GetAllAsync();

                // Aktif sorumluları filtrele
                var aktiveSorumlular = allSorumlular.Where(s => s.Aktif).ToList();

                if (!aktiveSorumlular.Any())
                {
                    _logger.LogWarning("Hiç aktif izin sorumlusu bulunamadı");
                    return;
                }

                // Personelin departman ve servisine göre sorumluları filtrele
                var uygunSorumlular = aktiveSorumlular.Where(s =>
                    // Departman kontrolü: Null ise tüm departmanlar, değilse eşleşmeli
                    (!s.DepartmanId.HasValue || s.DepartmanId == personel.DepartmanId) &&
                    // Servis kontrolü: Null ise tüm servisler, değilse eşleşmeli
                    (!s.ServisId.HasValue || s.ServisId == personel.ServisId)
                ).ToList();

                if (!uygunSorumlular.Any())
                {
                    _logger.LogWarning("Personel için uygun izin sorumlusu bulunamadı. Departman: {DeptId}, Servis: {ServId}",
                        personel.DepartmanId, personel.ServisId);
                    return;
                }

                // 1. ve 2. Onaycıları ayır
                var birinciOnaycılar = uygunSorumlular.Where(s => s.OnaySeviyesi == 1).ToList();
                var ikinciOnaycılar = uygunSorumlular.Where(s => s.OnaySeviyesi == 2).ToList();

                // İZİN TALEPLERİ: 1. VE 2. onaycı gerekli (sıralı onay)
                // Planlı izinler için (mazeret değilse)
                if (talep.IzinMazeretTuru?.PlanliIzinMi == true)
                {
                    // 1. Onaycı ata
                    if (birinciOnaycılar.Any())
                    {
                        var birinci = birinciOnaycılar.First();
                        talep.BirinciOnayciTcKimlikNo = birinci.SorumluPersonelTcKimlikNo;
                        _logger.LogInformation("İzin talebi için 1. Onaycı atandı: {Tc}", birinci.SorumluPersonelTcKimlikNo);
                    }
                    else
                    {
                        _logger.LogWarning("İzin talebi için 1. Onaycı bulunamadı");
                    }

                    // 2. Onaycı ata
                    if (ikinciOnaycılar.Any())
                    {
                        var ikinci = ikinciOnaycılar.First();
                        talep.IkinciOnayciTcKimlikNo = ikinci.SorumluPersonelTcKimlikNo;
                        _logger.LogInformation("İzin talebi için 2. Onaycı atandı: {Tc}", ikinci.SorumluPersonelTcKimlikNo);
                    }
                    else
                    {
                        _logger.LogWarning("İzin talebi için 2. Onaycı bulunamadı");
                    }
                }
                // MAZERET TALEPLERİ: 1. VEYA 2. onaycıdan biri yeterli (tek onay)
                else
                {
                    // Önce 1. Onaycı varsa onu ata
                    if (birinciOnaycılar.Any())
                    {
                        var birinci = birinciOnaycılar.First();
                        talep.BirinciOnayciTcKimlikNo = birinci.SorumluPersonelTcKimlikNo;
                        _logger.LogInformation("Mazeret talebi için 1. Onaycı atandı: {Tc}", birinci.SorumluPersonelTcKimlikNo);
                    }
                    // 1. Onaycı yoksa 2. Onaycıyı ata
                    else if (ikinciOnaycılar.Any())
                    {
                        var ikinci = ikinciOnaycılar.First();
                        talep.IkinciOnayciTcKimlikNo = ikinci.SorumluPersonelTcKimlikNo;
                        _logger.LogInformation("Mazeret talebi için 2. Onaycı atandı: {Tc}", ikinci.SorumluPersonelTcKimlikNo);
                    }
                    else
                    {
                        _logger.LogWarning("Mazeret talebi için hiç onaycı bulunamadı");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Onaycı atama sırasında hata oluştu");
                // Hata olsa bile talep oluşturulsun, manuel atama yapılabilir
            }
        }

        // ═══════════════════════════════════════════════════════
        // SGK İŞLEM TAKİBİ
        // ═══════════════════════════════════════════════════════

        public async Task<ApiResponseDto<bool>> ProcessSgkIslemAsync(IzinSgkIslemRequestDto request, string kullaniciTc)
        {
            try
            {
                var repository = _unitOfWork.GetRepository<IIzinMazeretTalepRepository>();
                var talep = await repository.GetByIdAsync(request.IzinMazeretTalepId);

                if (talep == null)
                    return ApiResponseDto<bool>.ErrorResult("Talep bulunamadı");

                // Sadece onaylanmış talepler işlenebilir
                if (talep.BirinciOnayDurumu != OnayDurumu.Onaylandi || 
                    talep.IkinciOnayDurumu != OnayDurumu.Onaylandi)
                {
                    return ApiResponseDto<bool>.ErrorResult("Sadece onaylanmış talepler işlenebilir");
                }

                if (request.Isle)
                {
                    // SGK'ya işle
                    if (talep.IzinIslendiMi)
                        return ApiResponseDto<bool>.ErrorResult("Bu talep zaten işlenmiş");

                    talep.IzinIslendiMi = true;
                    talep.IzinIslemTarihi = DateTimeHelper.Now;
                    talep.IzinIslemYapanKullanici = kullaniciTc;
                    talep.IzinIslemNotlari = request.Notlar;
                }
                else
                {
                    // İşlemi geri al
                    if (!talep.IzinIslendiMi)
                        return ApiResponseDto<bool>.ErrorResult("Bu talep henüz işlenmemiş");

                    talep.IzinIslendiMi = false;
                    talep.IzinIslemTarihi = null;
                    talep.IzinIslemYapanKullanici = null;
                    talep.IzinIslemNotlari = request.Notlar; // Geri alma nedeni
                }

                repository.Update(talep);
                await _unitOfWork.SaveChangesAsync();

                var mesaj = request.Isle ? "İzin SGK sistemine başarıyla işlendi" : "SGK işlemi başarıyla geri alındı";
                return ApiResponseDto<bool>.SuccessResult(true, mesaj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SGK işlem sırasında hata: {TalepId}", request.IzinMazeretTalepId);
                return ApiResponseDto<bool>.ErrorResult($"İşlem sırasında hata: {ex.Message}");
            }
        }
    }
}
