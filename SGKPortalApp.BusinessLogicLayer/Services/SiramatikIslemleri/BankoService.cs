using AutoMapper;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Common;
using SGKPortalApp.Common.Extensions;
using SGKPortalApp.BusinessLogicLayer.Interfaces.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;

namespace SGKPortalApp.BusinessLogicLayer.Services.SiramatikIslemleri
{
    public class BankoService : IBankoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<BankoService> _logger;
        private readonly ICascadeHelper _cascadeHelper;

        public BankoService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<BankoService> logger,
            ICascadeHelper cascadeHelper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _cascadeHelper = cascadeHelper;
        }

        // ═══════════════════════════════════════════════════════
        // CRUD OPERATIONS
        // ═══════════════════════════════════════════════════════

        public async Task<ApiResponseDto<List<BankoResponseDto>>> GetAllAsync()
        {
            try
            {
                var bankoRepo = _unitOfWork.GetRepository<IBankoRepository>();
                var bankolar = await bankoRepo.GetAllWithHizmetBinasiAsync();

                var bankoDtos = _mapper.Map<List<BankoResponseDto>>(bankolar);

                // IsConnected durumunu set et
                foreach (var bankoDto in bankoDtos)
                {
                    var entity = bankolar.FirstOrDefault(b => b.BankoId == bankoDto.BankoId);
                    if (entity != null)
                    {
                        bankoDto.IsConnected = entity.HubBankoConnection != null && 
                                              entity.HubBankoConnection.HubConnection != null && 
                                              entity.HubBankoConnection.HubConnection.ConnectionStatus == ConnectionStatus.online;
                    }
                }

                return ApiResponseDto<List<BankoResponseDto>>
                    .SuccessResult(bankoDtos, "Bankolar başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Banko listesi getirilirken hata oluştu");
                return ApiResponseDto<List<BankoResponseDto>>
                    .ErrorResult("Bankolar getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<BankoResponseDto>> GetByIdAsync(int bankoId)
        {
            try
            {
                if (bankoId <= 0)
                {
                    return ApiResponseDto<BankoResponseDto>
                        .ErrorResult("Geçersiz banko ID");
                }

                var bankoRepo = _unitOfWork.GetRepository<IBankoRepository>();
                var banko = await bankoRepo.GetWithPersonelAsync(bankoId);

                if (banko == null)
                {
                    return ApiResponseDto<BankoResponseDto>
                        .ErrorResult("Banko bulunamadı");
                }

                var bankoDto = _mapper.Map<BankoResponseDto>(banko);

                return ApiResponseDto<BankoResponseDto>
                    .SuccessResult(bankoDto, "Banko başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Banko getirilirken hata oluştu. ID: {BankoId}", bankoId);
                return ApiResponseDto<BankoResponseDto>
                    .ErrorResult("Banko getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<BankoResponseDto>> CreateAsync(BankoCreateRequestDto request)
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                try
                {
                    var bankoRepo = _unitOfWork.GetRepository<IBankoRepository>();

                    // Aynı binada, aynı katta, aynı numara kontrolü
                    var existingBanko = await bankoRepo.GetByBankoNoAsync(request.BankoNo, request.HizmetBinasiId);
                    if (existingBanko != null && existingBanko.KatTipi == request.KatTipi)
                    {
                        return ApiResponseDto<BankoResponseDto>
                            .ErrorResult($"Bu hizmet binasında {request.KatTipi.GetDisplayName()} katında {request.BankoNo} numaralı banko zaten mevcut");
                    }

                    var banko = _mapper.Map<Banko>(request);
                    banko.Aktiflik = Aktiflik.Aktif;

                    await bankoRepo.AddAsync(banko);
                    await _unitOfWork.SaveChangesAsync();

                    var bankoDto = _mapper.Map<BankoResponseDto>(banko);

                    _logger.LogInformation("Yeni banko oluşturuldu. BankoId: {BankoId}, BankoNo: {BankoNo}, Kat: {Kat}",
                        banko.BankoId, banko.BankoNo, banko.KatTipi);

                    return ApiResponseDto<BankoResponseDto>
                        .SuccessResult(bankoDto, "Banko başarıyla oluşturuldu");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Banko oluşturulurken hata oluştu");
                    return ApiResponseDto<BankoResponseDto>
                        .ErrorResult("Banko oluşturulurken bir hata oluştu", ex.Message);
                }
            });
        }

        public async Task<ApiResponseDto<BankoResponseDto>> UpdateAsync(int bankoId, BankoUpdateRequestDto request)
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                try
                {
                    var bankoRepo = _unitOfWork.GetRepository<IBankoRepository>();
                    var banko = await bankoRepo.GetByIdAsync(bankoId);

                    if (banko == null)
                    {
                        return ApiResponseDto<BankoResponseDto>
                            .ErrorResult("Banko bulunamadı");
                    }

                    // Banko No veya Kat değişiyorsa, unique kontrolü yap
                    if (banko.BankoNo != request.BankoNo || banko.KatTipi != request.KatTipi)
                    {
                        var existingBanko = await bankoRepo.GetByBankoNoAsync(request.BankoNo, banko.HizmetBinasiId);
                        if (existingBanko != null && existingBanko.BankoId != bankoId && existingBanko.KatTipi == request.KatTipi)
                        {
                            return ApiResponseDto<BankoResponseDto>
                                .ErrorResult($"Bu hizmet binasında {request.KatTipi.GetDisplayName()} katında {request.BankoNo} numaralı banko zaten mevcut");
                        }
                    }

                    banko.BankoNo = request.BankoNo;
                    banko.KatTipi = request.KatTipi;
                    banko.BankoTipi = request.BankoTipi;
                    banko.BankoAciklama = request.BankoAciklama;
                    banko.DuzenlenmeTarihi = DateTime.Now;

                    bankoRepo.Update(banko);
                    await _unitOfWork.SaveChangesAsync();

                    var bankoDto = _mapper.Map<BankoResponseDto>(banko);

                    _logger.LogInformation("Banko güncellendi. BankoId: {BankoId}, BankoNo: {BankoNo}, Kat: {Kat}", 
                        bankoId, banko.BankoNo, banko.KatTipi);

                    return ApiResponseDto<BankoResponseDto>
                        .SuccessResult(bankoDto, "Banko başarıyla güncellendi");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Banko güncellenirken hata oluştu. BankoId: {BankoId}", bankoId);
                    return ApiResponseDto<BankoResponseDto>
                        .ErrorResult("Banko güncellenirken bir hata oluştu", ex.Message);
                }
            });
        }

        public async Task<ApiResponseDto<bool>> DeleteAsync(int bankoId)
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                try
                {
                    var bankoRepo = _unitOfWork.GetRepository<IBankoRepository>();
                    var banko = await bankoRepo.GetByIdAsync(bankoId);

                    if (banko == null)
                    {
                        return ApiResponseDto<bool>
                            .ErrorResult("Banko bulunamadı");
                    }

                    // Cascade: Child kayıtları da sil (soft delete)
                    await CascadeDeleteAsync(bankoId);

                    // Soft delete
                    banko.SilindiMi = true;
                    banko.DuzenlenmeTarihi = DateTime.Now;

                    bankoRepo.Update(banko);
                    await _unitOfWork.SaveChangesAsync();

                    _logger.LogInformation("Banko silindi. BankoId: {BankoId}", bankoId);

                    return ApiResponseDto<bool>
                        .SuccessResult(true, "Banko başarıyla silindi");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Banko silinirken hata oluştu. BankoId: {BankoId}", bankoId);
                    return ApiResponseDto<bool>
                        .ErrorResult("Banko silinirken bir hata oluştu", ex.Message);
                }
            });
        }

        // ═══════════════════════════════════════════════════════
        // QUERY OPERATIONS
        // ═══════════════════════════════════════════════════════

        public async Task<ApiResponseDto<List<BankoResponseDto>>> GetByHizmetBinasiAsync(int hizmetBinasiId)
        {
            try
            {
                var bankoRepo = _unitOfWork.GetRepository<IBankoRepository>();
                var bankolar = await bankoRepo.GetByHizmetBinasiAsync(hizmetBinasiId);

                var bankoDtos = _mapper.Map<List<BankoResponseDto>>(bankolar);

                return ApiResponseDto<List<BankoResponseDto>>
                    .SuccessResult(bankoDtos, "Bankolar başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hizmet binası bankoları getirilirken hata oluştu. BinaId: {BinaId}", hizmetBinasiId);
                return ApiResponseDto<List<BankoResponseDto>>
                    .ErrorResult("Bankolar getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<BankoKatGrupluResponseDto>>> GetGroupedByKatAsync(int hizmetBinasiId)
        {
            try
            {
                var bankoRepo = _unitOfWork.GetRepository<IBankoRepository>();
                var bankoDict = await bankoRepo.GetGroupedByKatAsync(hizmetBinasiId);

                var result = bankoDict.Select(kvp => new BankoKatGrupluResponseDto
                {
                    KatTipi = kvp.Key,
                    KatTipiAdi = kvp.Key.GetDisplayName(),
                    Bankolar = _mapper.Map<List<BankoResponseDto>>(kvp.Value)
                }).ToList();

                return ApiResponseDto<List<BankoKatGrupluResponseDto>>
                    .SuccessResult(result, "Kat gruplu bankolar başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kat gruplu bankolar getirilirken hata oluştu. BinaId: {BinaId}", hizmetBinasiId);
                return ApiResponseDto<List<BankoKatGrupluResponseDto>>
                    .ErrorResult("Bankolar getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<BankoResponseDto>>> GetAvailableBankosAsync(int hizmetBinasiId)
        {
            try
            {
                var bankoRepo = _unitOfWork.GetRepository<IBankoRepository>();
                var bankolar = await bankoRepo.GetAvailableBankosAsync(hizmetBinasiId);

                var bankoDtos = _mapper.Map<List<BankoResponseDto>>(bankolar);

                return ApiResponseDto<List<BankoResponseDto>>
                    .SuccessResult(bankoDtos, "Boş bankolar başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Boş bankolar getirilirken hata oluştu. BinaId: {BinaId}", hizmetBinasiId);
                return ApiResponseDto<List<BankoResponseDto>>
                    .ErrorResult("Bankolar getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<BankoResponseDto>>> GetActiveAsync()
        {
            try
            {
                var bankoRepo = _unitOfWork.GetRepository<IBankoRepository>();
                var bankolar = await bankoRepo.GetActiveAsync();

                var bankoDtos = _mapper.Map<List<BankoResponseDto>>(bankolar);

                return ApiResponseDto<List<BankoResponseDto>>
                    .SuccessResult(bankoDtos, "Aktif bankolar başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Aktif bankolar getirilirken hata oluştu");
                return ApiResponseDto<List<BankoResponseDto>>
                    .ErrorResult("Bankolar getirilirken bir hata oluştu", ex.Message);
            }
        }

        // ═══════════════════════════════════════════════════════
        // PERSONEL ATAMA OPERATIONS
        // ═══════════════════════════════════════════════════════

        public async Task<ApiResponseDto<bool>> AssignPersonelToBankoAsync(BankoPersonelAtaDto request)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(request.TcKimlikNo))
                {
                    return ApiResponseDto<bool>.ErrorResult("TC Kimlik No boş olamaz");
                }

                if (request.BankoId <= 0)
                {
                    return ApiResponseDto<bool>.ErrorResult("Geçersiz banko ID");
                }

                var bankoKullaniciRepo = _unitOfWork.GetRepository<IBankoKullaniciRepository>();
                var bankoRepo = _unitOfWork.GetRepository<IBankoRepository>();
                var personelRepo = _unitOfWork.GetRepository<IPersonelRepository>();

                // Önce ilişkili kayıtların varlığını kontrol et
                var personelExists = await personelRepo.GetByTcKimlikNoAsync(request.TcKimlikNo);
                if (personelExists == null)
                {
                    return ApiResponseDto<bool>.ErrorResult("Personel bulunamadı");
                }

                var bankoExists = await bankoRepo.GetByIdAsync(request.BankoId);
                if (bankoExists == null)
                {
                    return ApiResponseDto<bool>.ErrorResult("Banko bulunamadı");
                }

                if (bankoExists.Aktiflik != Aktiflik.Aktif)
                {
                    return ApiResponseDto<bool>.ErrorResult("Banko aktif değil");
                }

                // Bankonun başka personele atanmış olup olmadığını kontrol et
                var existingBankoAssignment = await bankoKullaniciRepo.GetByBankoAsync(request.BankoId);
                if (existingBankoAssignment != null)
                {
                    _logger.LogWarning(
                        "Banko zaten atanmış. BankoId: {BankoId}, AtananPersonel: {TcKimlikNo}, SilindiMi: {SilindiMi}",
                        request.BankoId, existingBankoAssignment.TcKimlikNo, existingBankoAssignment.SilindiMi);
                    
                    return ApiResponseDto<bool>.ErrorResult($"Bu banko zaten {existingBankoAssignment.Personel?.AdSoyad ?? existingBankoAssignment.TcKimlikNo} personeline atanmış");
                }

                // Personelin başka bankoda olup olmadığını kontrol et
                var existingAssignment = await bankoKullaniciRepo.GetByPersonelAsync(request.TcKimlikNo);
                if (existingAssignment != null)
                {
                    // Önce eski atamayı kaldır
                    await bankoKullaniciRepo.UnassignPersonelAsync(request.TcKimlikNo);
                    await _unitOfWork.SaveChangesAsync();
                }

                // ⭐ YENİ: HizmetBinasiId kontrolü - Personel ve Banko aynı hizmet binasında olmalı
                if (personelExists.HizmetBinasiId != bankoExists.HizmetBinasiId)
                {
                    return ApiResponseDto<bool>.ErrorResult(
                        $"Personel ({personelExists.HizmetBinasi?.HizmetBinasiAdi}) ve Banko ({bankoExists.HizmetBinasi?.HizmetBinasiAdi}) farklı hizmet binalarında. Atama yapılamaz.");
                }

                // Yeni atama yap
                var bankoKullanici = new BankoKullanici
                {
                    BankoId = request.BankoId,
                    TcKimlikNo = request.TcKimlikNo,
                    // ⭐ ÖNEMLİ: Banko'nun HizmetBinasiId'si kullanılmalı (personel'in değil)
                    // Çünkü bu field "Bu banko hangi hizmet binasında?" sorusunu cevaplar
                    HizmetBinasiId = bankoExists.HizmetBinasiId,
                    EklenmeTarihi = DateTime.Now,
                    DuzenlenmeTarihi = DateTime.Now,
                    Banko = null!,
                    Personel = null!,
                    HizmetBinasi = null!
                };

                await bankoKullaniciRepo.AddAsync(bankoKullanici);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation(
                    "Personel bankoya atandı. Personel: {TcKimlikNo}, Banko: {BankoId}",
                    request.TcKimlikNo, request.BankoId);

                return ApiResponseDto<bool>.SuccessResult(true, "Personel bankoya başarıyla atandı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel atanırken hata oluştu");
                return ApiResponseDto<bool>.ErrorResult("Personel atanırken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> UnassignPersonelFromBankoAsync(string tcKimlikNo)
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                try
                {
                    var bankoKullaniciRepo = _unitOfWork.GetRepository<IBankoKullaniciRepository>();
                    await bankoKullaniciRepo.UnassignPersonelAsync(tcKimlikNo);
                    await _unitOfWork.SaveChangesAsync();

                    _logger.LogInformation("Personel bankodan çıkarıldı. TcKimlikNo: {TcKimlikNo}", tcKimlikNo);

                    return ApiResponseDto<bool>
                        .SuccessResult(true, "Personel bankodan başarıyla çıkarıldı");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Personel çıkarılırken hata oluştu. TcKimlikNo: {TcKimlikNo}", tcKimlikNo);
                    return ApiResponseDto<bool>
                        .ErrorResult("Personel çıkarılırken bir hata oluştu", ex.Message);
                }
            });
        }

        public async Task<ApiResponseDto<bool>> UnassignBankoAsync(int bankoId)
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                try
                {
                    var bankoKullaniciRepo = _unitOfWork.GetRepository<IBankoKullaniciRepository>();
                    await bankoKullaniciRepo.UnassignBankoAsync(bankoId);
                    await _unitOfWork.SaveChangesAsync();

                    _logger.LogInformation("Banko boşaltıldı. BankoId: {BankoId}", bankoId);

                    return ApiResponseDto<bool>
                        .SuccessResult(true, "Banko başarıyla boşaltıldı");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Banko boşaltılırken hata oluştu. BankoId: {BankoId}", bankoId);
                    return ApiResponseDto<bool>
                        .ErrorResult("Banko boşaltılırken bir hata oluştu", ex.Message);
                }
            });
        }

        public async Task<ApiResponseDto<BankoResponseDto>> GetPersonelCurrentBankoAsync(string tcKimlikNo)
        {
            try
            {
                var bankoRepo = _unitOfWork.GetRepository<IBankoRepository>();
                var banko = await bankoRepo.GetByKullaniciAsync(tcKimlikNo);

                if (banko == null)
                {
                    return ApiResponseDto<BankoResponseDto>
                        .ErrorResult("Personel şu anda bir bankoda değil");
                }

                var bankoDto = _mapper.Map<BankoResponseDto>(banko);

                return ApiResponseDto<BankoResponseDto>
                    .SuccessResult(bankoDto, "Personelin bankosu başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel bankosu getirilirken hata oluştu. TcKimlikNo: {TcKimlikNo}", tcKimlikNo);
                return ApiResponseDto<BankoResponseDto>
                    .ErrorResult("Personel bankosu getirilirken bir hata oluştu", ex.Message);
            }
        }

        // ═══════════════════════════════════════════════════════
        // MAINTENANCE OPERATIONS
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Tutarsız BankoKullanici kayıtlarını tespit eder ve temizler
        /// Personel.HizmetBinasiId != BankoKullanici.HizmetBinasiId olan kayıtlar
        /// </summary>
        public async Task<ApiResponseDto<int>> CleanupInconsistentBankoAssignmentsAsync()
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                try
                {
                    var bankoKullaniciRepo = _unitOfWork.GetRepository<IBankoKullaniciRepository>();
                    var personelRepo = _unitOfWork.GetRepository<IPersonelRepository>();

                    // Tüm aktif banko kullanıcılarını getir
                    var allAssignments = await bankoKullaniciRepo.GetAllWithDetailsAsync();

                    int cleanedCount = 0;

                    foreach (var assignment in allAssignments)
                    {
                        // Personel bilgisini al
                        var personel = await personelRepo.GetByTcKimlikNoAsync(assignment.TcKimlikNo);

                        if (personel == null)
                        {
                            _logger.LogWarning(
                                "BankoKullanici kaydı var ama personel bulunamadı. TC: {TcKimlikNo}, BankoKullaniciId: {Id}",
                                assignment.TcKimlikNo, assignment.BankoKullaniciId);
                            continue;
                        }

                        // Tutarsızlık kontrolü: Personel.HizmetBinasiId != BankoKullanici.HizmetBinasiId
                        if (personel.HizmetBinasiId != assignment.HizmetBinasiId)
                        {
                            _logger.LogWarning(
                                "Tutarsız kayıt tespit edildi. TC: {TcKimlikNo}, " +
                                "Personel HizmetBinasi: {PersonelHizmetBinasi}, " +
                                "BankoKullanici HizmetBinasi: {BankoKullaniciHizmetBinasi}",
                                assignment.TcKimlikNo,
                                personel.HizmetBinasiId,
                                assignment.HizmetBinasiId);

                            // Soft delete
                            bankoKullaniciRepo.Delete(assignment);
                            cleanedCount++;

                            // Eğer kullanıcı banko modundaysa, çıkar
                            var user = await _unitOfWork.Repository<User>().GetByIdAsync(assignment.TcKimlikNo);
                            if (user != null && user.BankoModuAktif)
                            {
                                user.BankoModuAktif = false;
                                user.AktifBankoId = null;
                                user.BankoModuBaslangic = null;
                                _unitOfWork.Repository<User>().Update(user);

                                _logger.LogInformation(
                                    "Tutarsız kayıt nedeniyle kullanıcı banko modundan çıkarıldı. TC: {TcKimlikNo}",
                                    assignment.TcKimlikNo);
                            }
                        }
                    }

                    await _unitOfWork.SaveChangesAsync();

                    _logger.LogInformation(
                        "Tutarsız banko atama temizleme işlemi tamamlandı. Temizlenen kayıt sayısı: {Count}",
                        cleanedCount);

                    return ApiResponseDto<int>.SuccessResult(
                        cleanedCount,
                        $"{cleanedCount} tutarsız kayıt başarıyla temizlendi");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Tutarsız kayıt temizleme işlemi sırasında hata oluştu");
                    return ApiResponseDto<int>.ErrorResult(
                        "Tutarsız kayıt temizleme işlemi başarısız oldu",
                        ex.Message);
                }
            });
        }

        // ═══════════════════════════════════════════════════════
        // AKTIFLIK OPERATIONS
        // ═══════════════════════════════════════════════════════

        public async Task<ApiResponseDto<bool>> ToggleAktiflikAsync(int bankoId)
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                try
                {
                    var bankoRepo = _unitOfWork.GetRepository<IBankoRepository>();
                    var bankoKullaniciRepo = _unitOfWork.GetRepository<IBankoKullaniciRepository>();
                    
                    var banko = await bankoRepo.GetByIdAsync(bankoId);

                    if (banko == null)
                    {
                        return ApiResponseDto<bool>.ErrorResult("Banko bulunamadı");
                    }

                    var yeniDurum = banko.Aktiflik == Aktiflik.Aktif
                        ? Aktiflik.Pasif
                        : Aktiflik.Aktif;

                    // Eğer pasif yapılıyorsa, önce personeli çıkar
                    if (yeniDurum == Aktiflik.Pasif)
                    {
                        var atananPersonel = await bankoKullaniciRepo.GetByBankoAsync(bankoId);
                        if (atananPersonel != null)
                        {
                            await bankoKullaniciRepo.UnassignBankoAsync(bankoId);
                            _logger.LogInformation(
                                "Banko pasif yapılırken personel çıkarıldı. BankoId: {BankoId}, TcKimlikNo: {TcKimlikNo}",
                                bankoId, atananPersonel.TcKimlikNo);
                        }
                    }

                    banko.Aktiflik = yeniDurum;
                    banko.DuzenlenmeTarihi = DateTime.Now;

                    bankoRepo.Update(banko);

                    // Cascade: Pasif yapıldıysa child kayıtları da soft delete
                    if (yeniDurum == Aktiflik.Pasif)
                    {
                        await CascadeAktiflikUpdateAsync(bankoId);
                    }

                    await _unitOfWork.SaveChangesAsync();

                    _logger.LogInformation("Banko aktiflik durumu değiştirildi. BankoId: {BankoId}, Durum: {Durum}",
                        bankoId, banko.Aktiflik);

                    return ApiResponseDto<bool>
                        .SuccessResult(true, $"Banko {(banko.Aktiflik == Aktiflik.Aktif ? "aktif" : "pasif")} yapıldı");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Banko aktiflik durumu değiştirilirken hata oluştu. BankoId: {BankoId}", bankoId);
                    return ApiResponseDto<bool>
                        .ErrorResult("Aktiflik durumu değiştirilirken bir hata oluştu", ex.Message);
                }
            });
        }

        /// <summary>
        /// Cascade delete: Banko silindiğinde child kayıtları da siler
        /// CascadeHelper kullanarak tracking conflict'leri otomatik handle eder
        /// </summary>
        private async Task CascadeDeleteAsync(int bankoId)
        {
            await _cascadeHelper.CascadeSoftDeleteAsync<TvBanko>(x => x.BankoId == bankoId);
            await _cascadeHelper.CascadeSoftDeleteAsync<BankoKullanici>(x => x.BankoId == bankoId);
            await _cascadeHelper.CascadeSoftDeleteAsync<BankoHareket>(x => x.BankoId == bankoId);
            await _cascadeHelper.CascadeSoftDeleteAsync<HubBankoConnection>(x => x.BankoId == bankoId);

            _logger.LogInformation("Cascade delete: BankoId={BankoId}", bankoId);
        }

        /// <summary>
        /// Cascade update: Banko pasif yapıldığında child kayıtları da soft delete
        /// CascadeHelper kullanarak tracking conflict'leri otomatik handle eder
        /// </summary>
        private async Task CascadeAktiflikUpdateAsync(int bankoId)
        {
            await _cascadeHelper.CascadeSoftDeleteAsync<TvBanko>(x => x.BankoId == bankoId);
            await _cascadeHelper.CascadeSoftDeleteAsync<BankoHareket>(x => x.BankoId == bankoId);
            await _cascadeHelper.CascadeSoftDeleteAsync<HubBankoConnection>(x => x.BankoId == bankoId);

            _logger.LogInformation("Cascade pasif: BankoId={BankoId}", bankoId);
        }
    }
}
