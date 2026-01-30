using AutoMapper;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using SGKPortalApp.BusinessLogicLayer.Interfaces.SiramatikIslemleri;
using SGKPortalApp.Common.Extensions;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SGKPortalApp.Common.Helpers;

namespace SGKPortalApp.BusinessLogicLayer.Services.SiramatikIslemleri
{
    public class TvService : ITvService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<TvService> _logger;
        private readonly ICascadeHelper _cascadeHelper;

        public TvService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<TvService> logger,
            ICascadeHelper cascadeHelper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _cascadeHelper = cascadeHelper;
        }

        public async Task<ApiResponseDto<TvResponseDto>> CreateAsync(TvCreateRequestDto request)
        {
            try
            {
                var tvRepo = _unitOfWork.GetRepository<ITvRepository>();

                // Aynı isimde TV var mı kontrol et
                var existingTv = await tvRepo.FindAsync(t => t.TvAdi == request.TvAdi && !t.SilindiMi);
                if (existingTv.Any())
                {
                    return ApiResponseDto<TvResponseDto>.ErrorResult("Bu isimde bir TV zaten mevcut");
                }
                
                var tv = _mapper.Map<Tv>(request);
                tv.IslemZamani = DateTimeHelper.Now;

                var createdTv = await tvRepo.AddAsync(tv);
                await _unitOfWork.SaveChangesAsync();

                // TV için User oluştur
                await CreateTvUserAsync(createdTv.TvId);

                var response = _mapper.Map<TvResponseDto>(createdTv);

                _logger.LogInformation($"TV oluşturuldu: {createdTv.TvId} - {createdTv.TvAdi}");
                return ApiResponseDto<TvResponseDto>.SuccessResult(response, "TV başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TV oluşturulurken hata oluştu");
                return ApiResponseDto<TvResponseDto>.ErrorResult($"TV oluşturulurken bir hata oluştu: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<TvResponseDto>> UpdateAsync(TvUpdateRequestDto request)
        {
            try
            {
                var tvRepo = _unitOfWork.GetRepository<ITvRepository>();
                var existingTv = await tvRepo.GetByIdAsync(request.TvId);
                if (existingTv == null)
                {
                    return ApiResponseDto<TvResponseDto>.ErrorResult("TV bulunamadı");
                }

                // Aynı isimde başka TV var mı kontrol et (kendisi hariç)
                var duplicateTv = await tvRepo.FindAsync(t => t.TvAdi == request.TvAdi && t.TvId != request.TvId && !t.SilindiMi);
                if (duplicateTv.Any())
                {
                    return ApiResponseDto<TvResponseDto>.ErrorResult("Bu isimde bir TV zaten mevcut");
                }

                var oldAktiflik = existingTv.Aktiflik;

                _mapper.Map(request, existingTv);
                existingTv.DuzenlenmeTarihi = DateTimeHelper.Now;
                existingTv.IslemZamani = DateTimeHelper.Now;

                tvRepo.Update(existingTv);

                // Cascade: Aktiflik değişmişse child kayıtları da güncelle
                if (oldAktiflik != existingTv.Aktiflik && existingTv.Aktiflik == Aktiflik.Pasif)
                {
                    await CascadeAktiflikUpdateAsync(existingTv.TvId);
                }

                await _unitOfWork.SaveChangesAsync();

                // Eğer TV için User yoksa oluştur
                if (string.IsNullOrEmpty(existingTv.TcKimlikNo))
                {
                    await CreateTvUserAsync(existingTv.TvId);
                }

                var response = _mapper.Map<TvResponseDto>(existingTv);

                _logger.LogInformation($"TV güncellendi: {existingTv.TvId} - {existingTv.TvAdi}");
                return ApiResponseDto<TvResponseDto>.SuccessResult(response, "TV başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TV güncellenirken hata oluştu");
                return ApiResponseDto<TvResponseDto>.ErrorResult($"TV güncellenirken bir hata oluştu: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteAsync(int tvId)
        {
            try
            {
                var tvRepo = _unitOfWork.GetRepository<ITvRepository>();
                var tv = await tvRepo.GetByIdAsync(tvId);
                if (tv == null)
                {
                    return ApiResponseDto<bool>.ErrorResult("TV bulunamadı");
                }

                // Cascade: Child kayıtları da sil (soft delete)
                await CascadeDeleteAsync(tvId);

                // Soft delete
                tv.SilindiMi = true;
                tv.DuzenlenmeTarihi = DateTimeHelper.Now;
                tvRepo.Update(tv);

                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation($"TV silindi: {tvId}");
                return ApiResponseDto<bool>.SuccessResult(true, "TV başarıyla silindi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TV silinirken hata oluştu");
                return ApiResponseDto<bool>.ErrorResult($"TV silinirken bir hata oluştu: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<TvResponseDto>> GetByIdAsync(int tvId)
        {
            try
            {
                var tvRepo = _unitOfWork.GetRepository<ITvRepository>();
                var tv = await tvRepo.GetByIdAsync(tvId);
                if (tv == null)
                {
                    return ApiResponseDto<TvResponseDto>.ErrorResult("TV bulunamadı");
                }

                var response = _mapper.Map<TvResponseDto>(tv);
                return ApiResponseDto<TvResponseDto>.SuccessResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TV getirilirken hata oluştu");
                return ApiResponseDto<TvResponseDto>.ErrorResult($"TV getirilirken bir hata oluştu: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<List<TvResponseDto>>> GetAllAsync()
        {
            try
            {
                var tvRepo = _unitOfWork.GetRepository<ITvRepository>();
                var tvler = await tvRepo.GetAllWithDetailsAsync();
                var response = _mapper.Map<List<TvResponseDto>>(tvler);
                
                // Her TV için banko sayısını ve bağlantı durumunu set et
                foreach (var tv in response)
                {
                    var entity = tvler.FirstOrDefault(t => t.TvId == tv.TvId);
                    if (entity != null)
                    {
                        tv.BankoSayisi = entity.TvBankolar?.Count(tb => tb.Aktiflik == Aktiflik.Aktif) ?? 0;
                        tv.IsConnected = entity.HubTvConnections?.Any(htc => 
                                         htc.HubConnection != null && 
                                         htc.HubConnection.ConnectionStatus == ConnectionStatus.online) ?? false;
                    }
                }
                
                return ApiResponseDto<List<TvResponseDto>>.SuccessResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TV'ler getirilirken hata oluştu");
                return ApiResponseDto<List<TvResponseDto>>.ErrorResult($"TV'ler getirilirken bir hata oluştu: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<List<TvResponseDto>>> GetByDepartmanHizmetBinasiAsync(int departmanHizmetBinasiId)
        {
            try
            {
                var tvRepo = _unitOfWork.GetRepository<ITvRepository>();
                var tvler = await tvRepo.GetAllWithDetailsAsync();
                var filteredTvler = tvler.Where(t => t.DepartmanHizmetBinasiId == departmanHizmetBinasiId).ToList();
                var response = _mapper.Map<List<TvResponseDto>>(filteredTvler);
                
                // Her TV için banko sayısını ve bağlantı durumunu set et
                foreach (var tv in response)
                {
                    var entity = filteredTvler.FirstOrDefault(t => t.TvId == tv.TvId);
                    if (entity != null)
                    {
                        tv.BankoSayisi = entity.TvBankolar?.Count(tb => tb.Aktiflik == Aktiflik.Aktif) ?? 0;
                        tv.IsConnected = entity.HubTvConnections?.Any(htc => 
                                         htc.HubConnection != null && 
                                         htc.HubConnection.ConnectionStatus == ConnectionStatus.online) ?? false;
                    }
                }
                return ApiResponseDto<List<TvResponseDto>>.SuccessResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hizmet binası TV'leri getirilirken hata oluştu");
                return ApiResponseDto<List<TvResponseDto>>.ErrorResult($"Hizmet binası TV'leri getirilirken bir hata oluştu: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<List<TvResponseDto>>> GetByKatTipiAsync(KatTipi katTipi)
        {
            try
            {
                var tvRepo = _unitOfWork.GetRepository<ITvRepository>();
                var tvler = await tvRepo.GetAllWithDetailsAsync();
                var filteredTvler = tvler.Where(t => t.KatTipi == katTipi).ToList();
                var response = _mapper.Map<List<TvResponseDto>>(filteredTvler);
                
                // Her TV için banko sayısını ve bağlantı durumunu set et
                foreach (var tv in response)
                {
                    var entity = filteredTvler.FirstOrDefault(t => t.TvId == tv.TvId);
                    if (entity != null)
                    {
                        tv.BankoSayisi = entity.TvBankolar?.Count(tb => tb.Aktiflik == Aktiflik.Aktif) ?? 0;
                        tv.IsConnected = entity.HubTvConnections?.Any(htc => 
                                         htc.HubConnection != null && 
                                         htc.HubConnection.ConnectionStatus == ConnectionStatus.online) ?? false;
                    }
                }
                
                return ApiResponseDto<List<TvResponseDto>>.SuccessResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kat tipi TV'leri getirilirken hata oluştu");
                return ApiResponseDto<List<TvResponseDto>>.ErrorResult($"Kat tipi TV'leri getirilirken bir hata oluştu: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<List<TvResponseDto>>> GetActiveAsync()
        {
            try
            {
                var tvRepo = _unitOfWork.GetRepository<ITvRepository>();
                var tvler = await tvRepo.GetAllWithDetailsAsync();
                var filteredTvler = tvler.Where(t => t.Aktiflik == Aktiflik.Aktif).ToList();
                var response = _mapper.Map<List<TvResponseDto>>(filteredTvler);
                
                // Her TV için banko sayısını ve bağlantı durumunu set et
                foreach (var tv in response)
                {
                    var entity = filteredTvler.FirstOrDefault(t => t.TvId == tv.TvId);
                    if (entity != null)
                    {
                        tv.BankoSayisi = entity.TvBankolar?.Count(tb => tb.Aktiflik == Aktiflik.Aktif) ?? 0;
                        tv.IsConnected = entity.HubTvConnections?.Any(htc => 
                                         htc.HubConnection != null && 
                                         htc.HubConnection.ConnectionStatus == ConnectionStatus.online) ?? false;
                    }
                }
                
                return ApiResponseDto<List<TvResponseDto>>.SuccessResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Aktif TV'ler getirilirken hata oluştu");
                return ApiResponseDto<List<TvResponseDto>>.ErrorResult($"Aktif TV'ler getirilirken bir hata oluştu: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<TvResponseDto>> GetWithDetailsAsync(int tvId)
        {
            try
            {
                var tvRepo = _unitOfWork.GetRepository<ITvRepository>();
                var tv = await tvRepo.GetWithDetailsAsync(tvId);
                if (tv == null)
                {
                    return ApiResponseDto<TvResponseDto>.ErrorResult("TV bulunamadı");
                }

                var response = _mapper.Map<TvResponseDto>(tv);
                
                // Banko sayısını hesapla
                response.BankoSayisi = tv.TvBankolar?.Count(tb => tb.Aktiflik == Aktiflik.Aktif) ?? 0;
                
                // Eşleşmiş banko ID'lerini ekle
                response.EslesmiBankoIdler = tv.TvBankolar?
                    .Where(tb => tb.Aktiflik == Aktiflik.Aktif)
                    .Select(tb => tb.BankoId)
                    .ToList() ?? new List<int>();
                
                // Bağlantı durumunu kontrol et (En az 1 online bağlantı varsa connected)
                response.IsConnected = tv.HubTvConnections?.Any(htc => 
                                      htc.HubConnection != null && 
                                      htc.HubConnection.ConnectionStatus == ConnectionStatus.online) ?? false;

                return ApiResponseDto<TvResponseDto>.SuccessResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TV detayları getirilirken hata oluştu");
                return ApiResponseDto<TvResponseDto>.ErrorResult($"TV detayları getirilirken bir hata oluştu: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<List<(int Id, string Ad)>>> GetDropdownAsync()
        {
            try
            {
                var tvRepo = _unitOfWork.GetRepository<ITvRepository>();
                var tvler = await tvRepo.GetDropdownAsync();
                return ApiResponseDto<List<(int Id, string Ad)>>.SuccessResult(tvler.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TV dropdown getirilirken hata oluştu");
                return ApiResponseDto<List<(int Id, string Ad)>>.ErrorResult($"TV dropdown getirilirken bir hata oluştu: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<List<(int Id, string Ad)>>> GetByDepartmanHizmetBinasiDropdownAsync(int departmanHizmetBinasiId)
        {
            try
            {
                var tvRepo = _unitOfWork.GetRepository<ITvRepository>();
                var tvler = await tvRepo.GetByDepartmanHizmetBinasiDropdownAsync(departmanHizmetBinasiId);
                return ApiResponseDto<List<(int Id, string Ad)>>.SuccessResult(tvler.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Departman-Hizmet binası TV dropdown getirilirken hata oluştu");
                return ApiResponseDto<List<(int Id, string Ad)>>.ErrorResult($"Departman-Hizmet binası TV dropdown getirilirken bir hata oluştu: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<bool>> AddBankoToTvAsync(int tvId, int bankoId)
        {
            try
            {
                var tvRepo = _unitOfWork.GetRepository<ITvRepository>();
                var tvBankoRepo = _unitOfWork.GetRepository<ITvBankoRepository>();
                
                // TV var mı kontrol et
                var tv = await tvRepo.GetByIdAsync(tvId);
                if (tv == null)
                {
                    return ApiResponseDto<bool>.ErrorResult("TV bulunamadı");
                }

                // Zaten ekli mi kontrol et
                var existingTvBanko = await tvBankoRepo.GetByTvAndBankoAsync(tvId, bankoId);
                if (existingTvBanko != null && existingTvBanko.Aktiflik == Aktiflik.Aktif)
                {
                    return ApiResponseDto<bool>.ErrorResult("Bu banko zaten TV'ye atanmış");
                }

                // Eğer daha önce eklenmişse ama pasifse, aktif et
                if (existingTvBanko != null && existingTvBanko.Aktiflik == Aktiflik.Pasif)
                {
                    existingTvBanko.Aktiflik = Aktiflik.Aktif;
                    existingTvBanko.DuzenlenmeTarihi = DateTimeHelper.Now;
                    tvBankoRepo.Update(existingTvBanko);
                }
                else
                {
                    // Yeni TvBanko ilişkisi oluştur
                    var tvBanko = new TvBanko
                    {
                        TvId = tvId,
                        BankoId = bankoId,
                        Aktiflik = Aktiflik.Aktif,
                        EklenmeTarihi = DateTimeHelper.Now
                    };

                    await tvBankoRepo.AddAsync(tvBanko);
                }
                
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation($"Banko {bankoId} TV {tvId}'ye eklendi");
                return ApiResponseDto<bool>.SuccessResult(true, "Banko başarıyla eklendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Banko TV'ye eklenirken hata oluştu");
                return ApiResponseDto<bool>.ErrorResult($"Banko eklenirken bir hata oluştu: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<bool>> RemoveBankoFromTvAsync(int tvId, int bankoId)
        {
            try
            {
                var tvBankoRepo = _unitOfWork.GetRepository<ITvBankoRepository>();
                
                var tvBanko = await tvBankoRepo.GetByTvAndBankoAsync(tvId, bankoId);
                
                if (tvBanko == null || tvBanko.Aktiflik == Aktiflik.Pasif)
                {
                    return ApiResponseDto<bool>.ErrorResult("Bu banko TV'ye atanmamış");
                }

                // Soft delete
                tvBanko.Aktiflik = Aktiflik.Pasif;
                tvBanko.DuzenlenmeTarihi = DateTimeHelper.Now;
                tvBankoRepo.Update(tvBanko);
                
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation($"Banko {bankoId} TV {tvId}'den kaldırıldı");
                return ApiResponseDto<bool>.SuccessResult(true, "Banko başarıyla kaldırıldı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Banko TV'den kaldırılırken hata oluştu");
                return ApiResponseDto<bool>.ErrorResult($"Banko kaldırılırken bir hata oluştu: {ex.Message}");
            }
        }

        // ═══════════════════════════════════════════════════════
        // PUBLIC UTILITY METHODS
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Tüm mevcut TV'ler için User oluşturur (Migration sonrası kullanım için)
        /// </summary>
        public async Task<ApiResponseDto<int>> CreateUsersForAllTvsAsync()
        {
            try
            {
                _logger.LogInformation("🔄 Tüm TV'ler için User oluşturma başladı...");
                
                var tvRepo = _unitOfWork.GetRepository<ITvRepository>();
                var allTvs = await tvRepo.GetAllAsync();
                
                int createdCount = 0;
                int skippedCount = 0;
                int errorCount = 0;

                // Her TV için ayrı transaction kullan
                // Böylece bir TV'de hata olursa diğerleri etkilenmez
                foreach (var tv in allTvs)
                {
                    try
                    {
                        // TcKimlikNo zaten varsa atla
                        if (!string.IsNullOrEmpty(tv.TcKimlikNo))
                        {
                            _logger.LogInformation($"⏭️ TV#{tv.TvId} zaten User'a sahip: {tv.TcKimlikNo}");
                            skippedCount++;
                            continue;
                        }

                        // User oluştur (kendi transaction'ı içinde)
                        await CreateTvUserAsync(tv.TvId);
                        createdCount++;
                        
                        _logger.LogInformation($"✅ TV#{tv.TvId} için User oluşturuldu");
                    }
                    catch (Exception ex)
                    {
                        errorCount++;
                        _logger.LogError(ex, $"❌ TV#{tv.TvId} için User oluşturulamadı - Rollback yapıldı");
                        // Bu TV için rollback yapıldı, diğer TV'ler etkilenmedi
                    }
                }

                var message = $"Toplam: {allTvs.Count()} TV | Oluşturulan: {createdCount} | Atlanan: {skippedCount} | Hata: {errorCount}";
                _logger.LogInformation($"✅ {message}");
                
                return ApiResponseDto<int>.SuccessResult(createdCount, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Toplu TV User oluşturma hatası");
                return ApiResponseDto<int>.ErrorResult($"Toplu User oluşturma hatası: {ex.Message}");
            }
        }

        // ═══════════════════════════════════════════════════════
        // PRIVATE HELPER METHODS
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// TV için otomatik User oluşturur
        /// TcKimlikNo: TV{TvId:D7} formatında (örn: TV0000001)
        /// Password: TV{TvId} formatında (örn: TV1)
        /// </summary>
        private async Task CreateTvUserAsync(int tvId)
        {
            // Transaction kullan - ya hepsi başarılı olur, ya hiçbiri
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var tvRepo = _unitOfWork.GetRepository<ITvRepository>();
                var userRepo = _unitOfWork.GetRepository<IUserRepository>();

                // TV'yi bul
                var tv = await tvRepo.GetByIdAsync(tvId);
                if (tv == null)
                {
                    _logger.LogWarning($"TV bulunamadı: {tvId}");
                    return;
                }

                // TcKimlikNo oluştur: TV0000001
                var tcKimlikNo = $"TV{tvId:D7}";

                // User zaten var mı kontrol et
                var existingUser = await userRepo.GetByTcKimlikNoAsync(tcKimlikNo);
                if (existingUser != null)
                {
                    _logger.LogInformation($"TV User zaten mevcut: {tcKimlikNo}");
                    
                    // TV'ye bağla
                    tv.TcKimlikNo = tcKimlikNo;
                    tvRepo.Update(tv);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitTransactionAsync(transaction);
                    return;
                }

                // Şifre: TV1 (düz metin)
                var password = $"TV{tvId}";

                // User oluştur
                var tvUser = new User
                {
                    TcKimlikNo = tcKimlikNo,
                    UserType = UserType.TvUser,
                    PassWord = password, // Düz metin şifre
                    AktifMi = true,
                    EklenmeTarihi = DateTimeHelper.Now,
                    DuzenlenmeTarihi = DateTimeHelper.Now
                };

                // ÖNEMLİ: User'ı önce kaydet (FK constraint için gerekli)
                await userRepo.AddAsync(tvUser);
                await _unitOfWork.SaveChangesAsync();

                // User kaydedildikten SONRA TV'yi bağla
                tv.TcKimlikNo = tcKimlikNo;
                tvRepo.Update(tv);
                await _unitOfWork.SaveChangesAsync();

                // Transaction başarılı - commit et
                await _unitOfWork.CommitTransactionAsync(transaction);

                _logger.LogInformation($"✅ TV User oluşturuldu ve bağlandı: {tcKimlikNo} (Şifre: {password})");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ TV User oluşturulurken hata: TvId={tvId} - Transaction rollback yapılıyor");
                // Transaction otomatik rollback olacak (using block)
                throw; // Hatayı üst katmana ilet
            }
        }

        /// <summary>
        /// TV'ye bağlı bankolardaki aktif (çağrılmış) sıraları getirir
        /// Tüm bankolardan son çağrılan sıralar, çağrılma zamanına göre sıralı döner
        /// Client-side'da TV resolution'a göre satır sayısı belirlenir
        /// </summary>
        public async Task<ApiResponseDto<List<TvSiraDto>>> GetActiveSiralarByTvIdAsync(int tvId)
        {
            try
            {
                var tvRepo = _unitOfWork.GetRepository<ITvRepository>();
                var bankoHareketRepo = _unitOfWork.GetRepository<IBankoHareketRepository>();

                // TV'yi kontrol et
                var tv = await tvRepo.GetByIdAsync(tvId);
                if (tv == null || tv.SilindiMi)
                {
                    return ApiResponseDto<List<TvSiraDto>>.ErrorResult("TV bulunamadı");
                }

                // TV'ye bağlı bankoları al
                var tvBankolar = await tvRepo.GetTvBankolarAsync(tvId);
                if (tvBankolar == null || !tvBankolar.Any())
                {
                    return ApiResponseDto<List<TvSiraDto>>.SuccessResult(new List<TvSiraDto>(), "TV'ye bağlı banko bulunamadı");
                }

                var bankoIds = tvBankolar.Select(tb => tb.BankoId).ToList();

                // Tüm bankolardaki aktif sıraları tek sorguda al
                var aktifHareketler = await bankoHareketRepo.GetAktifSiralarByBankoIdsAsync(bankoIds);

                // DTO'ya dönüştür (çağrılma zamanına göre zaten sıralı geliyor)
                var siralar = aktifHareketler.Select(bh => new TvSiraDto
                {
                    BankoId = bh.BankoId,
                    BankoNo = bh.Banko?.BankoNo ?? 0,
                    KatTipi = bh.Banko?.KatTipi.GetDisplayName() ?? "",
                    BankoTipi = bh.Banko?.BankoTipi.GetDisplayName() ?? "",
                    SiraNo = bh.SiraNo
                }).ToList();

                _logger.LogInformation($"TV#{tvId} için {siralar.Count} aktif sıra bulundu");
                return ApiResponseDto<List<TvSiraDto>>.SuccessResult(siralar);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"TV sıraları getirilirken hata: TvId={tvId}");
                return ApiResponseDto<List<TvSiraDto>>.ErrorResult($"Sıralar getirilirken hata oluştu: {ex.Message}");
            }
        }

        /// <summary>
        /// Cascade delete: TV silindiğinde child kayıtları da siler
        /// CascadeHelper kullanarak tracking conflict'leri otomatik handle eder
        /// </summary>
        private async Task CascadeDeleteAsync(int tvId)
        {
            // TvBanko kayıtlarını soft delete
            await _cascadeHelper.CascadeSoftDeleteAsync<TvBanko>(x => x.TvId == tvId);
            
            // HubTvConnection kayıtlarını soft delete
            await _cascadeHelper.CascadeSoftDeleteAsync<HubTvConnection>(x => x.TvId == tvId);

            _logger.LogInformation("Cascade delete: TvId={TvId}", tvId);
        }

        /// <summary>
        /// Cascade update: TV pasif yapıldığında child kayıtları da soft delete
        /// CascadeHelper kullanarak tracking conflict'leri otomatik handle eder
        /// </summary>
        private async Task CascadeAktiflikUpdateAsync(int tvId)
        {
            // TvBanko kayıtlarını soft delete (pasif TV'nin bankoları kaldırılmalı)
            await _cascadeHelper.CascadeSoftDeleteAsync<TvBanko>(x => x.TvId == tvId);
            
            // HubTvConnection kayıtlarını soft delete
            await _cascadeHelper.CascadeSoftDeleteAsync<HubTvConnection>(x => x.TvId == tvId);

            _logger.LogInformation("Cascade pasif: TvId={TvId}", tvId);
        }
    }
}
