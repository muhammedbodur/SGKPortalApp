using AutoMapper;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Exceptions;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Common;
using System.Text;
using System.Text.Json;

namespace SGKPortalApp.BusinessLogicLayer.Services.PersonelIslemleri
{
    public class PersonelService : IPersonelService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<PersonelService> _logger;
        private readonly IFieldPermissionValidationService _fieldPermissionService;

        public PersonelService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<PersonelService> logger,
            IFieldPermissionValidationService fieldPermissionService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _fieldPermissionService = fieldPermissionService;
        }

        private async Task<(bool Ok, string? ErrorMessage)> ValidateRequestorAsync(string? requestorTcKimlikNo, string? requestorSessionId)
        {
            if (string.IsNullOrWhiteSpace(requestorTcKimlikNo))
                return (false, "Oturum bilgisi bulunamadı");

            if (string.IsNullOrWhiteSpace(requestorSessionId))
                return (false, "Oturum bilgisi bulunamadı");

            var userRepo = _unitOfWork.Repository<User>();
            var user = await userRepo.FirstOrDefaultAsync(u => u.TcKimlikNo == requestorTcKimlikNo);

            if (user == null)
                return (false, "Kullanıcı bulunamadı");

            if (string.IsNullOrWhiteSpace(user.SessionID) || !string.Equals(user.SessionID, requestorSessionId, StringComparison.Ordinal))
                return (false, "Oturum süresi dolmuş olabilir. Lütfen tekrar giriş yapın");

            return (true, null);
        }

        private async Task<YetkiSeviyesi> GetPermissionLevelAsync(string tcKimlikNo, string permissionKey)
        {
            if (string.IsNullOrWhiteSpace(tcKimlikNo) || string.IsNullOrWhiteSpace(permissionKey))
                return YetkiSeviyesi.None;

            // PermissionKey ile eşleşen ModulControllerIslem'i bul
            var islem = await _unitOfWork.Repository<ModulControllerIslem>()
                .FirstOrDefaultAsync(m => m.PermissionKey == permissionKey);

            if (islem == null)
                return YetkiSeviyesi.Edit; // Tanımsız permission key = full access

            // Personelin bu işlem için atanmış seviyesini bul
            var personelYetki = await _unitOfWork.Repository<PersonelYetki>()
                .FirstOrDefaultAsync(py => py.TcKimlikNo == tcKimlikNo && py.ModulControllerIslemId == islem.ModulControllerIslemId);

            if (personelYetki == null)
                return YetkiSeviyesi.None;

            return personelYetki.YetkiSeviyesi;
        }

        /// <summary>
        /// Kullanıcının tüm field permission'larını dictionary olarak döndürür
        /// Key: PermissionKey, Value: YetkiSeviyesi
        /// </summary>
        private async Task<Dictionary<string, YetkiSeviyesi>> GetUserFieldPermissionsAsync(string tcKimlikNo)
        {
            var result = new Dictionary<string, YetkiSeviyesi>();
            
            if (string.IsNullOrWhiteSpace(tcKimlikNo))
                return result;

            // Kullanıcının tüm yetkilerini çek (filter ile)
            var personelYetkiler = await _unitOfWork.Repository<PersonelYetki>()
                .FindAsync(py => py.TcKimlikNo == tcKimlikNo && !py.SilindiMi);

            foreach (var yetki in personelYetkiler)
            {
                var islem = await _unitOfWork.Repository<ModulControllerIslem>()
                    .GetByIdAsync(yetki.ModulControllerIslemId);
                
                if (islem != null && !string.IsNullOrEmpty(islem.PermissionKey))
                {
                    result[islem.PermissionKey] = yetki.YetkiSeviyesi;
                }
            }

            return result;
        }

        public async Task<ApiResponseDto<List<PersonelResponseDto>>> GetAllAsync()
        {
            try
            {
                var personelRepo = _unitOfWork.GetRepository<IPersonelRepository>();
                var personeller = await personelRepo.GetAllWithDetailsAsync();
                
                var personelDtos = _mapper.Map<List<PersonelResponseDto>>(personeller);

                return ApiResponseDto<List<PersonelResponseDto>>
                    .SuccessResult(personelDtos, "Personeller başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel listesi getirilirken hata oluştu");
                return ApiResponseDto<List<PersonelResponseDto>>
                    .ErrorResult("Personeller getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<PersonelResponseDto>> UpdateAsync(string tcKimlikNo, PersonelUpdateRequestDto request)
        {
            try
            {
                var validation = await ValidateRequestorAsync(request.RequestorTcKimlikNo, request.RequestorSessionId);
                if (!validation.Ok)
                    return ApiResponseDto<PersonelResponseDto>.ErrorResult(validation.ErrorMessage ?? "Yetkisiz işlem");

                var level = await GetPermissionLevelAsync(request.RequestorTcKimlikNo!, "PER.PERSONEL.MANAGE");
                if (level < YetkiSeviyesi.Edit)
                    return ApiResponseDto<PersonelResponseDto>.ErrorResult("Bu işlem için yetkiniz bulunmuyor");

                var personel = await _unitOfWork.Repository<Personel>().GetByIdAsync(tcKimlikNo);
                if (personel == null)
                    return ApiResponseDto<PersonelResponseDto>.ErrorResult("Personel bulunamadı");

                // ⭐ Field-level permission enforcement
                // Kullanıcının tüm field permission'larını yükle
                var userPermissions = await GetUserFieldPermissionsAsync(request.RequestorTcKimlikNo!);

                // Orijinal DTO'yu oluştur (mevcut entity'den)
                var originalDto = _mapper.Map<PersonelUpdateRequestDto>(personel);

                // Yetkisiz field değişikliklerini tespit et (convention-based: PER.PERSONEL.MANAGE.FIELD.{FIELDNAME})
                var unauthorizedFields = await _fieldPermissionService.ValidateFieldPermissionsAsync(
                    request,
                    userPermissions,
                    originalDto,
                    "PER.PERSONEL.MANAGE", // pagePermissionKey
                    request.RequestorTcKimlikNo); // userTcKimlikNo for audit logging

                // Yetkisiz alanları orijinal değerlere geri al (sessiz revert)
                if (unauthorizedFields.Any())
                {
                    _fieldPermissionService.RevertUnauthorizedFields(request, originalDto, unauthorizedFields);
                    _logger.LogWarning(
                        "UpdateAsync - Field-level permission enforcement: {Count} alan revert edildi. TC: {TcKimlikNo}, Alanlar: {Fields}",
                        unauthorizedFields.Count, tcKimlikNo, string.Join(", ", unauthorizedFields));
                }

                _mapper.Map(request, personel);

                _unitOfWork.Repository<Personel>().Update(personel);
                await _unitOfWork.SaveChangesAsync();

                var personelRepo = _unitOfWork.GetRepository<IPersonelRepository>();
                var savedPersonel = await personelRepo.GetByTcKimlikNoWithDetailsAsync(tcKimlikNo);
                var dto = _mapper.Map<PersonelResponseDto>(savedPersonel);

                return ApiResponseDto<PersonelResponseDto>.SuccessResult(dto, "Personel başarıyla güncellendi");
            }
            catch (DatabaseException ex)
            {
                _logger.LogWarning(ex, "Veritabanı kısıtlama hatası: {ErrorType}", ex.ErrorType);
                return ApiResponseDto<PersonelResponseDto>
                    .ErrorResult(ex.UserFriendlyMessage, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel güncellenirken hata oluştu. TC: {TcKimlikNo}", tcKimlikNo);
                return ApiResponseDto<PersonelResponseDto>
                    .ErrorResult("Personel güncellenirken bir hata oluştu", ex.Message);
            }
        }


        public async Task<ApiResponseDto<PersonelResponseDto>> GetByTcKimlikNoAsync(string tcKimlikNo)
        {
            try
            {
                var personelRepo = _unitOfWork.GetRepository<IPersonelRepository>();
                var personel = await personelRepo.GetByTcKimlikNoWithDetailsAsync(tcKimlikNo);

                if (personel == null)
                {
                    return ApiResponseDto<PersonelResponseDto>
                        .ErrorResult("Personel bulunamadı");
                }

                var personelDto = _mapper.Map<PersonelResponseDto>(personel);

                return ApiResponseDto<PersonelResponseDto>
                    .SuccessResult(personelDto, "Personel başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel getirilirken hata oluştu. TC: {TcKimlikNo}", tcKimlikNo);
                return ApiResponseDto<PersonelResponseDto>
                    .ErrorResult("Personel getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteAsync(string tcKimlikNo)
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                try
                {
                    var personelRepo = _unitOfWork.GetRepository<IPersonelRepository>();
                    var personel = await personelRepo.GetByTcKimlikNoAsync(tcKimlikNo);

                    if (personel == null)
                    {
                        return ApiResponseDto<bool>
                            .ErrorResult("Personel bulunamadı");
                    }

                    // 1. İlişkili User kaydını sil
                    var user = await _unitOfWork.Repository<User>().GetByIdAsync(tcKimlikNo);
                    if (user != null)
                    {
                        _unitOfWork.Repository<User>().Delete(user);
                        _logger.LogInformation("User kaydı silindi. TC: {TcKimlikNo}", tcKimlikNo);
                    }

                    // 2. İlişkili PersonelCocuk kayıtlarını sil
                    var cocuklar = await _unitOfWork.Repository<PersonelCocuk>()
                        .FindAsync(c => c.PersonelTcKimlikNo == tcKimlikNo);
                    foreach (var cocuk in cocuklar)
                    {
                        _unitOfWork.Repository<PersonelCocuk>().Delete(cocuk);
                    }

                    // 3. İlişkili PersonelHizmet kayıtlarını sil
                    var hizmetler = await _unitOfWork.Repository<PersonelHizmet>()
                        .FindAsync(h => h.TcKimlikNo == tcKimlikNo);
                    foreach (var hizmet in hizmetler)
                    {
                        _unitOfWork.Repository<PersonelHizmet>().Delete(hizmet);
                    }

                    // 4. İlişkili PersonelEgitim kayıtlarını sil
                    var egitimler = await _unitOfWork.Repository<PersonelEgitim>()
                        .FindAsync(e => e.TcKimlikNo == tcKimlikNo);
                    foreach (var egitim in egitimler)
                    {
                        _unitOfWork.Repository<PersonelEgitim>().Delete(egitim);
                    }

                    // 5. İlişkili PersonelImzaYetkisi kayıtlarını sil
                    var yetkiler = await _unitOfWork.Repository<PersonelImzaYetkisi>()
                        .FindAsync(y => y.TcKimlikNo == tcKimlikNo);
                    foreach (var yetki in yetkiler)
                    {
                        _unitOfWork.Repository<PersonelImzaYetkisi>().Delete(yetki);
                    }

                    // 6. İlişkili PersonelCeza kayıtlarını sil
                    var cezalar = await _unitOfWork.Repository<PersonelCeza>()
                        .FindAsync(c => c.TcKimlikNo == tcKimlikNo);
                    foreach (var ceza in cezalar)
                    {
                        _unitOfWork.Repository<PersonelCeza>().Delete(ceza);
                    }

                    // 7. İlişkili PersonelEngel kayıtlarını sil
                    var engeller = await _unitOfWork.Repository<PersonelEngel>()
                        .FindAsync(e => e.TcKimlikNo == tcKimlikNo);
                    foreach (var engel in engeller)
                    {
                        _unitOfWork.Repository<PersonelEngel>().Delete(engel);
                    }

                    // 8. Personel kaydını sil
                    personelRepo.Delete(personel);

                    // Tüm değişiklikleri kaydet
                    await _unitOfWork.SaveChangesAsync();

                    _logger.LogInformation(
                        "Personel ve tüm ilişkili kayıtlar silindi. TC: {TcKimlikNo}, Ad Soyad: {AdSoyad}",
                        tcKimlikNo, personel.AdSoyad);

                    return ApiResponseDto<bool>
                        .SuccessResult(true, "Personel ve tüm ilişkili kayıtlar başarıyla silindi");
                }
                catch (DatabaseException ex)
                {
                    _logger.LogWarning(ex, "Veritabanı kısıtlama hatası: {ErrorType}", ex.ErrorType);
                    return ApiResponseDto<bool>
                        .ErrorResult(ex.UserFriendlyMessage, ex.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Personel silinirken hata oluştu. TC: {TcKimlikNo}", tcKimlikNo);
                    return ApiResponseDto<bool>
                        .ErrorResult("Personel silinirken bir hata oluştu. Tüm işlemler geri alındı.", ex.Message);
                }
            });
        }

        public async Task<ApiResponseDto<List<PersonelResponseDto>>> GetActiveAsync()
        {
            try
            {
                var personelRepo = _unitOfWork.GetRepository<IPersonelRepository>();
                var personeller = await personelRepo.GetActiveAsync();
                var personelDtos = _mapper.Map<List<PersonelResponseDto>>(personeller);

                return ApiResponseDto<List<PersonelResponseDto>>
                    .SuccessResult(personelDtos, "Aktif personeller başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Aktif personeller getirilirken hata oluştu");
                return ApiResponseDto<List<PersonelResponseDto>>
                    .ErrorResult("Aktif personeller getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<PagedResponseDto<PersonelListResponseDto>>> GetPagedAsync(PersonelFilterRequestDto filter)
        {
            try
            {
                var personelRepo = _unitOfWork.GetRepository<IPersonelRepository>();
                var pagedResult = await personelRepo.GetPagedAsync(filter);
                
                return ApiResponseDto<PagedResponseDto<PersonelListResponseDto>>
                    .SuccessResult(pagedResult, "Sayfalı personel listesi başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sayfalı personel listesi getirilirken hata oluştu");
                return ApiResponseDto<PagedResponseDto<PersonelListResponseDto>>
                    .ErrorResult("Sayfalı personel listesi getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<PersonelResponseDto>>> GetByDepartmanAsync(int departmanId)
        {
            try
            {
                var personelRepo = _unitOfWork.GetRepository<IPersonelRepository>();
                var personeller = await personelRepo.GetByDepartmanAsync(departmanId);
                var personelDtos = _mapper.Map<List<PersonelResponseDto>>(personeller);

                return ApiResponseDto<List<PersonelResponseDto>>
                    .SuccessResult(personelDtos, "Departman personelleri başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Departman personelleri getirilirken hata oluştu. DepartmanId: {DepartmanId}", departmanId);
                return ApiResponseDto<List<PersonelResponseDto>>
                    .ErrorResult("Departman personelleri getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<PersonelResponseDto>>> GetByServisAsync(int servisId)
        {
            try
            {
                var personelRepo = _unitOfWork.GetRepository<IPersonelRepository>();
                var personeller = await personelRepo.GetByServisAsync(servisId);
                var personelDtos = _mapper.Map<List<PersonelResponseDto>>(personeller);

                return ApiResponseDto<List<PersonelResponseDto>>
                    .SuccessResult(personelDtos, "Servis personelleri başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Servis personelleri getirilirken hata oluştu. ServisId: {ServisId}", servisId);
                return ApiResponseDto<List<PersonelResponseDto>>
                    .ErrorResult("Servis personelleri getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<PersonelResponseDto>>> GetByHizmetBinasiIdAsync(int hizmetBinasiId)
        {
            try
            {
                var personelRepo = _unitOfWork.GetRepository<IPersonelRepository>();
                var personeller = await personelRepo.GetByHizmetBinasiIdAsync(hizmetBinasiId);
                var personelDtos = _mapper.Map<List<PersonelResponseDto>>(personeller);

                return ApiResponseDto<List<PersonelResponseDto>>
                    .SuccessResult(personelDtos, "Hizmet binası personelleri başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hizmet binası personelleri getirilirken hata oluştu. HizmetBinasiId: {HizmetBinasiId}", hizmetBinasiId);
                return ApiResponseDto<List<PersonelResponseDto>>
                    .ErrorResult("Hizmet binası personelleri getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<PersonelResponseDto>> CreateCompleteAsync(PersonelCompleteRequestDto request)
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                try
                {
                    var validation = await ValidateRequestorAsync(request.RequestorTcKimlikNo, request.RequestorSessionId);
                    if (!validation.Ok)
                        return ApiResponseDto<PersonelResponseDto>.ErrorResult(validation.ErrorMessage ?? "Yetkisiz işlem");

                    var level = await GetPermissionLevelAsync(request.RequestorTcKimlikNo!, "PER.PERSONEL.MANAGE");
                    if (level < YetkiSeviyesi.Edit)
                        return ApiResponseDto<PersonelResponseDto>.ErrorResult("Bu işlem için yetkiniz bulunmuyor");

                    // 1. Personel kaydet
                    var personel = _mapper.Map<Personel>(request.Personel);
                    await _unitOfWork.Repository<Personel>().AddAsync(personel);
                    await _unitOfWork.SaveChangesAsync();

                    var tcKimlikNo = personel.TcKimlikNo;

                    // 2. User otomatik oluştur (One-to-One ilişki)
                    var user = new User
                    {
                        TcKimlikNo = personel.TcKimlikNo,
                        PassWord = personel.TcKimlikNo, // Varsayılan şifre: TC Kimlik No
                        AktifMi = personel.PersonelAktiflikDurum == PersonelAktiflikDurum.Aktif,
                        BasarisizGirisSayisi = 0
                    };
                    await _unitOfWork.Repository<User>().AddAsync(user);

                    _logger.LogInformation(
                        "Personel ve User kaydı oluşturuldu. TC: {TcKimlikNo}, Ad Soyad: {AdSoyad}",
                        personel.TcKimlikNo, personel.AdSoyad);

                    // 3. Çocukları kaydet
                    if (request.Cocuklar?.Any() == true)
                    {
                        foreach (var cocukDto in request.Cocuklar)
                        {
                            cocukDto.PersonelTcKimlikNo = tcKimlikNo;
                            var cocuk = _mapper.Map<PersonelCocuk>(cocukDto);
                            await _unitOfWork.Repository<PersonelCocuk>().AddAsync(cocuk);
                        }
                    }

                    // 4. Hizmetleri kaydet
                    if (request.Hizmetler?.Any() == true)
                    {
                        foreach (var hizmetDto in request.Hizmetler)
                        {
                            hizmetDto.TcKimlikNo = tcKimlikNo;
                            var hizmet = _mapper.Map<PersonelHizmet>(hizmetDto);
                            await _unitOfWork.Repository<PersonelHizmet>().AddAsync(hizmet);
                        }
                    }

                    // 5. Eğitimleri kaydet
                    if (request.Egitimler?.Any() == true)
                    {
                        foreach (var egitimDto in request.Egitimler)
                        {
                            egitimDto.TcKimlikNo = tcKimlikNo;
                            var egitim = _mapper.Map<PersonelEgitim>(egitimDto);
                            await _unitOfWork.Repository<PersonelEgitim>().AddAsync(egitim);
                        }
                    }

                    // 6. İmza Yetkilerini kaydet
                    if (request.ImzaYetkileri?.Any() == true)
                    {
                        foreach (var yetkiDto in request.ImzaYetkileri)
                        {
                            yetkiDto.TcKimlikNo = tcKimlikNo;
                            var yetki = _mapper.Map<PersonelImzaYetkisi>(yetkiDto);
                            await _unitOfWork.Repository<PersonelImzaYetkisi>().AddAsync(yetki);
                        }
                    }

                    // 7. Cezaları kaydet
                    if (request.Cezalar?.Any() == true)
                    {
                        foreach (var cezaDto in request.Cezalar)
                        {
                            cezaDto.TcKimlikNo = tcKimlikNo;
                            var ceza = _mapper.Map<PersonelCeza>(cezaDto);
                            await _unitOfWork.Repository<PersonelCeza>().AddAsync(ceza);
                        }
                    }

                    // 8. Engelleri kaydet
                    if (request.Engeller?.Any() == true)
                    {
                        foreach (var engelDto in request.Engeller)
                        {
                            engelDto.TcKimlikNo = tcKimlikNo;
                            var engel = _mapper.Map<PersonelEngel>(engelDto);
                            await _unitOfWork.Repository<PersonelEngel>().AddAsync(engel);
                        }
                    }

                    // Tüm değişiklikleri kaydet
                    await _unitOfWork.SaveChangesAsync();

                    // Personel bilgisini geri döndür
                    var personelRepo = _unitOfWork.GetRepository<IPersonelRepository>();
                    var savedPersonel = await personelRepo.GetByTcKimlikNoWithDetailsAsync(tcKimlikNo);
                    var personelDto = _mapper.Map<PersonelResponseDto>(savedPersonel);

                    return ApiResponseDto<PersonelResponseDto>
                        .SuccessResult(personelDto, "Personel ve tüm bilgileri başarıyla kaydedildi");
                }
                catch (DatabaseException ex)
                {
                    _logger.LogWarning(ex, "Veritabanı kısıtlama hatası: {ErrorType}", ex.ErrorType);
                    return ApiResponseDto<PersonelResponseDto>
                        .ErrorResult(ex.UserFriendlyMessage, ex.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Personel toplu kayıt sırasında hata oluştu");
                    return ApiResponseDto<PersonelResponseDto>
                        .ErrorResult("Personel kaydedilirken bir hata oluştu. Tüm işlemler geri alındı.", ex.Message);
                }
            });
        }
        
        public async Task<ApiResponseDto<PersonelResponseDto>> UpdateCompleteAsync(string tcKimlikNo, PersonelCompleteRequestDto request)
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                try
                {
                    var validation = await ValidateRequestorAsync(request.RequestorTcKimlikNo, request.RequestorSessionId);
                    if (!validation.Ok)
                        return ApiResponseDto<PersonelResponseDto>.ErrorResult(validation.ErrorMessage ?? "Yetkisiz işlem");

                    var level = await GetPermissionLevelAsync(request.RequestorTcKimlikNo!, "PER.PERSONEL.MANAGE");
                    if (level < YetkiSeviyesi.Edit)
                        return ApiResponseDto<PersonelResponseDto>.ErrorResult("Bu işlem için yetkiniz bulunmuyor");

                    // 1. Personel güncelle
                    var personel = await _unitOfWork.Repository<Personel>().GetByIdAsync(tcKimlikNo);
                    if (personel == null)
                        return ApiResponseDto<PersonelResponseDto>.ErrorResult("Personel bulunamadı");

                    // ⭐ Field-level permission enforcement (B seçeneği: yetkisiz alanları sessizce revert et)
                    // Kullanıcının tüm field permission'larını yükle
                    var userPermissions = await GetUserFieldPermissionsAsync(request.RequestorTcKimlikNo!);
                    
                    // Orijinal DTO'yu oluştur (mevcut entity'den)
                    var originalDto = _mapper.Map<PersonelCreateRequestDto>(personel);
                    
                    // Yetkisiz field değişikliklerini tespit et (convention-based: PER.PERSONEL.MANAGE.FIELD.{FIELDNAME})
                    var unauthorizedFields = await _fieldPermissionService.ValidateFieldPermissionsAsync(
                        request.Personel,
                        userPermissions,
                        originalDto,
                        "PER.PERSONEL.MANAGE", // pagePermissionKey
                        request.RequestorTcKimlikNo); // userTcKimlikNo for audit logging
                    
                    // Yetkisiz alanları orijinal değerlere geri al (sessiz revert)
                    if (unauthorizedFields.Any())
                    {
                        _fieldPermissionService.RevertUnauthorizedFields(request.Personel, originalDto, unauthorizedFields);
                        _logger.LogWarning(
                            "Field-level permission enforcement: {Count} alan revert edildi. TC: {TcKimlikNo}, Alanlar: {Fields}",
                            unauthorizedFields.Count, tcKimlikNo, string.Join(", ", unauthorizedFields));
                    }

                    // Hizmet binası veya departman değişikliği kontrolü
                    var eskiHizmetBinasiId = personel.HizmetBinasiId;
                    var eskiDepartmanId = personel.DepartmanId;

                    _mapper.Map(request.Personel, personel);

                    _unitOfWork.Repository<Personel>().Update(personel);
                    await _unitOfWork.SaveChangesAsync();

                    // ⭐ Hizmet binası VEYA departman değiştiyse, eski banko atamasını temizle
                    if (eskiHizmetBinasiId != request.Personel.HizmetBinasiId ||
                        eskiDepartmanId != request.Personel.DepartmanId)
                    {
                        var bankoKullaniciRepo = _unitOfWork.GetRepository<IBankoKullaniciRepository>();
                        var eskiBankoAtamasi = await bankoKullaniciRepo.GetByPersonelAsync(tcKimlikNo);

                        if (eskiBankoAtamasi != null)
                        {
                            bankoKullaniciRepo.Delete(eskiBankoAtamasi);

                            var degisiklikTipi = eskiHizmetBinasiId != request.Personel.HizmetBinasiId
                                ? $"hizmet binası ({eskiHizmetBinasiId} -> {request.Personel.HizmetBinasiId})"
                                : $"departman ({eskiDepartmanId} -> {request.Personel.DepartmanId})";

                            _logger.LogInformation(
                                "UpdateCompleteAsync: Personelin {DegisiklikTipi} değişti. Eski banko ataması temizlendi. TC: {TcKimlikNo}",
                                degisiklikTipi, tcKimlikNo);
                        }

                        // Banko modundaysa çıkar
                        var user = await _unitOfWork.Repository<User>().GetByIdAsync(tcKimlikNo);
                        if (user != null && user.BankoModuAktif)
                        {
                            user.BankoModuAktif = false;
                            user.AktifBankoId = null;
                            user.BankoModuBaslangic = null;
                            _unitOfWork.Repository<User>().Update(user);
                            _logger.LogInformation("UpdateCompleteAsync: Personel hizmet binası/departman değiştiği için banko modundan çıkarıldı. TC: {TcKimlikNo}", tcKimlikNo);
                        }

                        await _unitOfWork.SaveChangesAsync();
                    }

                    // 2. User kontrolü - yoksa oluştur
                    var existingUser = await _unitOfWork.Repository<User>().GetByIdAsync(tcKimlikNo);
                    if (existingUser == null)
                    {
                        var user = new User
                        {
                            TcKimlikNo = personel.TcKimlikNo,
                            PassWord = personel.TcKimlikNo, // Varsayılan şifre: TC Kimlik No
                            AktifMi = personel.PersonelAktiflikDurum == PersonelAktiflikDurum.Aktif,
                            BasarisizGirisSayisi = 0
                        };
                        await _unitOfWork.Repository<User>().AddAsync(user);
                        await _unitOfWork.SaveChangesAsync();

                        _logger.LogInformation(
                            "Personel güncelleme sırasında User kaydı oluşturuldu. TC: {TcKimlikNo}",
                            personel.TcKimlikNo);
                    }
                    else
                    {
                        // Mevcut user'ı güncelle (aktiflik durumu)
                        existingUser.AktifMi = personel.PersonelAktiflikDurum == PersonelAktiflikDurum.Aktif;
                        _unitOfWork.Repository<User>().Update(existingUser);
                        await _unitOfWork.SaveChangesAsync();
                    }

                    // 3. Mevcut alt kayıtları sil
                    var mevcutCocuklar = await _unitOfWork.Repository<PersonelCocuk>()
                        .FindAsync(c => c.PersonelTcKimlikNo == tcKimlikNo);
                    foreach (var cocuk in mevcutCocuklar)
                        _unitOfWork.Repository<PersonelCocuk>().Delete(cocuk);

                    var mevcutHizmetler = await _unitOfWork.Repository<PersonelHizmet>()
                        .FindAsync(h => h.TcKimlikNo == tcKimlikNo);
                    foreach (var hizmet in mevcutHizmetler)
                        _unitOfWork.Repository<PersonelHizmet>().Delete(hizmet);

                    var mevcutEgitimler = await _unitOfWork.Repository<PersonelEgitim>()
                        .FindAsync(e => e.TcKimlikNo == tcKimlikNo);
                    foreach (var egitim in mevcutEgitimler)
                        _unitOfWork.Repository<PersonelEgitim>().Delete(egitim);

                    var mevcutYetkiler = await _unitOfWork.Repository<PersonelImzaYetkisi>()
                        .FindAsync(y => y.TcKimlikNo == tcKimlikNo);
                    foreach (var yetki in mevcutYetkiler)
                        _unitOfWork.Repository<PersonelImzaYetkisi>().Delete(yetki);

                    var mevcutCezalar = await _unitOfWork.Repository<PersonelCeza>()
                        .FindAsync(c => c.TcKimlikNo == tcKimlikNo);
                    foreach (var ceza in mevcutCezalar)
                        _unitOfWork.Repository<PersonelCeza>().Delete(ceza);

                    var mevcutEngeller = await _unitOfWork.Repository<PersonelEngel>()
                        .FindAsync(e => e.TcKimlikNo == tcKimlikNo);
                    foreach (var engel in mevcutEngeller)
                        _unitOfWork.Repository<PersonelEngel>().Delete(engel);

                    await _unitOfWork.SaveChangesAsync();

                    // 4. Yeni kayıtları ekle
                    if (request.Cocuklar?.Any() == true)
                    {
                        foreach (var cocukDto in request.Cocuklar)
                        {
                            cocukDto.PersonelTcKimlikNo = tcKimlikNo;
                            var cocuk = _mapper.Map<PersonelCocuk>(cocukDto);
                            await _unitOfWork.Repository<PersonelCocuk>().AddAsync(cocuk);
                        }
                    }

                    if (request.Hizmetler?.Any() == true)
                    {
                        foreach (var hizmetDto in request.Hizmetler)
                        {
                            hizmetDto.TcKimlikNo = tcKimlikNo;
                            var hizmet = _mapper.Map<PersonelHizmet>(hizmetDto);
                            await _unitOfWork.Repository<PersonelHizmet>().AddAsync(hizmet);
                        }
                    }

                    if (request.Egitimler?.Any() == true)
                    {
                        foreach (var egitimDto in request.Egitimler)
                        {
                            egitimDto.TcKimlikNo = tcKimlikNo;
                            var egitim = _mapper.Map<PersonelEgitim>(egitimDto);
                            await _unitOfWork.Repository<PersonelEgitim>().AddAsync(egitim);
                        }
                    }

                    if (request.ImzaYetkileri?.Any() == true)
                    {
                        foreach (var yetkiDto in request.ImzaYetkileri)
                        {
                            yetkiDto.TcKimlikNo = tcKimlikNo;
                            var yetki = _mapper.Map<PersonelImzaYetkisi>(yetkiDto);
                            await _unitOfWork.Repository<PersonelImzaYetkisi>().AddAsync(yetki);
                        }
                    }

                    if (request.Cezalar?.Any() == true)
                    {
                        foreach (var cezaDto in request.Cezalar)
                        {
                            cezaDto.TcKimlikNo = tcKimlikNo;
                            var ceza = _mapper.Map<PersonelCeza>(cezaDto);
                            await _unitOfWork.Repository<PersonelCeza>().AddAsync(ceza);
                        }
                    }

                    if (request.Engeller?.Any() == true)
                    {
                        foreach (var engelDto in request.Engeller)
                        {
                            engelDto.TcKimlikNo = tcKimlikNo;
                            var engel = _mapper.Map<PersonelEngel>(engelDto);
                            await _unitOfWork.Repository<PersonelEngel>().AddAsync(engel);
                        }
                    }

                    await _unitOfWork.SaveChangesAsync();

                    // Güncellenmiş personel bilgisini döndür
                    var personelRepo = _unitOfWork.GetRepository<IPersonelRepository>();
                    var updatedPersonel = await personelRepo.GetByTcKimlikNoWithDetailsAsync(tcKimlikNo);
                    var personelDto = _mapper.Map<PersonelResponseDto>(updatedPersonel);

                    // Mesajı oluştur - revert edilen alanlar varsa bildir
                    var message = "Personel ve tüm bilgileri başarıyla güncellendi";
                    if (unauthorizedFields.Any())
                    {
                        message += $". Uyarı: Yetkiniz olmayan {unauthorizedFields.Count} alan değişikliği geri alındı ({string.Join(", ", unauthorizedFields)})";
                    }

                    return ApiResponseDto<PersonelResponseDto>.SuccessResult(personelDto, message);
                }
                catch (DatabaseException ex)
                {
                    _logger.LogWarning(ex, "Veritabanı kısıtlama hatası: {ErrorType}", ex.ErrorType);
                    return ApiResponseDto<PersonelResponseDto>
                        .ErrorResult(ex.UserFriendlyMessage, ex.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Personel toplu güncelleme sırasında hata oluştu. TC: {TcKimlikNo}", tcKimlikNo);
                    return ApiResponseDto<PersonelResponseDto>
                        .ErrorResult("Personel güncellenirken bir hata oluştu. Tüm işlemler geri alındı.", ex.Message);
                }
            });
        }
    }
}
