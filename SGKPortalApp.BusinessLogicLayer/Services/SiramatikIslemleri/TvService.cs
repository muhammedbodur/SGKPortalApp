using AutoMapper;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using SGKPortalApp.BusinessLogicLayer.Interfaces.SiramatikIslemleri;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.BusinessLogicLayer.Services.SiramatikIslemleri
{
    public class TvService : ITvService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<TvService> _logger;

        public TvService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<TvService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponseDto<TvResponseDto>> CreateAsync(TvCreateRequestDto request)
        {
            try
            {
                var tvRepo = _unitOfWork.GetRepository<ITvRepository>();
                
                var tv = _mapper.Map<Tv>(request);
                tv.IslemZamani = DateTime.Now;

                var createdTv = await tvRepo.AddAsync(tv);
                await _unitOfWork.SaveChangesAsync();

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

                _mapper.Map(request, existingTv);
                existingTv.DuzenlenmeTarihi = DateTime.Now;
                existingTv.IslemZamani = DateTime.Now;

                tvRepo.Update(existingTv);
                await _unitOfWork.SaveChangesAsync();

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

                tvRepo.Delete(tv);
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
                        tv.IsConnected = entity.HubTvConnection != null && entity.HubTvConnection.ConnectionStatus == ConnectionStatus.online;
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

        public async Task<ApiResponseDto<List<TvResponseDto>>> GetByHizmetBinasiAsync(int hizmetBinasiId)
        {
            try
            {
                var tvRepo = _unitOfWork.GetRepository<ITvRepository>();
                var tvler = await tvRepo.GetAllWithDetailsAsync();
                var filteredTvler = tvler.Where(t => t.HizmetBinasiId == hizmetBinasiId).ToList();
                var response = _mapper.Map<List<TvResponseDto>>(filteredTvler);
                
                // Her TV için banko sayısını ve bağlantı durumunu set et
                foreach (var tv in response)
                {
                    var entity = filteredTvler.FirstOrDefault(t => t.TvId == tv.TvId);
                    if (entity != null)
                    {
                        tv.BankoSayisi = entity.TvBankolar?.Count(tb => tb.Aktiflik == Aktiflik.Aktif) ?? 0;
                        tv.IsConnected = entity.HubTvConnection != null && entity.HubTvConnection.ConnectionStatus == ConnectionStatus.online;
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
                        tv.IsConnected = entity.HubTvConnection != null && entity.HubTvConnection.ConnectionStatus == ConnectionStatus.online;
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
                var filteredTvler = tvler.Where(t => t.TvAktiflik == Aktiflik.Aktif).ToList();
                var response = _mapper.Map<List<TvResponseDto>>(filteredTvler);
                
                // Her TV için banko sayısını ve bağlantı durumunu set et
                foreach (var tv in response)
                {
                    var entity = filteredTvler.FirstOrDefault(t => t.TvId == tv.TvId);
                    if (entity != null)
                    {
                        tv.BankoSayisi = entity.TvBankolar?.Count(tb => tb.Aktiflik == Aktiflik.Aktif) ?? 0;
                        tv.IsConnected = entity.HubTvConnection != null && entity.HubTvConnection.ConnectionStatus == ConnectionStatus.online;
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
                
                // Bağlantı durumunu kontrol et
                response.IsConnected = tv.HubTvConnection != null && 
                                      tv.HubTvConnection.ConnectionStatus == ConnectionStatus.online;

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

        public async Task<ApiResponseDto<List<(int Id, string Ad)>>> GetByHizmetBinasiDropdownAsync(int hizmetBinasiId)
        {
            try
            {
                var tvRepo = _unitOfWork.GetRepository<ITvRepository>();
                var tvler = await tvRepo.GetByHizmetBinasiDropdownAsync(hizmetBinasiId);
                return ApiResponseDto<List<(int Id, string Ad)>>.SuccessResult(tvler.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hizmet binası TV dropdown getirilirken hata oluştu");
                return ApiResponseDto<List<(int Id, string Ad)>>.ErrorResult($"Hizmet binası TV dropdown getirilirken bir hata oluştu: {ex.Message}");
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
                    existingTvBanko.DuzenlenmeTarihi = DateTime.Now;
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
                        EklenmeTarihi = DateTime.Now
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
                tvBanko.DuzenlenmeTarihi = DateTime.Now;
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
    }
}
