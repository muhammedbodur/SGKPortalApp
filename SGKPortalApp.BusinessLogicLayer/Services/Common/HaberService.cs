using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Dashboard;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common;

namespace SGKPortalApp.BusinessLogicLayer.Services.Common
{
    public class HaberService : IHaberService
    {
        private readonly IHaberRepository _haberRepository;
        private readonly ILogger<HaberService> _logger;

        public HaberService(IHaberRepository haberRepository, ILogger<HaberService> logger)
        {
            _haberRepository = haberRepository;
            _logger = logger;
        }

        public async Task<ApiResponseDto<List<HaberResponseDto>>> GetSliderHaberleriAsync(int count = 5)
        {
            try
            {
                var haberler = await _haberRepository.GetSliderHaberleriAsync(count);
                var result = new List<HaberResponseDto>();

                foreach (var haber in haberler)
                {
                    var resimler = await _haberRepository.GetHaberResimleriAsync(haber.HaberId);
                    var vitrin = resimler.FirstOrDefault(r => r.IsVitrin);

                    result.Add(new HaberResponseDto
                    {
                        HaberId = haber.HaberId,
                        Baslik = haber.Baslik,
                        Icerik = haber.Icerik,
                        VitrinResimUrl = vitrin?.ResimUrl ?? haber.GorselUrl,
                        Resimler = resimler.Select(r => new HaberResimResponseDto
                        {
                            HaberResimId = r.HaberResimId,
                            ResimUrl = r.ResimUrl,
                            IsVitrin = r.IsVitrin,
                            Sira = r.Sira
                        }).ToList(),
                        Sira = haber.Sira,
                        YayinTarihi = haber.YayinTarihi,
                        BitisTarihi = haber.BitisTarihi
                    });
                }

                return ApiResponseDto<List<HaberResponseDto>>.SuccessResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Slider haberleri getirilirken hata oluştu");
                return ApiResponseDto<List<HaberResponseDto>>.ErrorResult("Slider haberleri getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<HaberListeResponseDto>> GetHaberListeAsync(
            int pageNumber = 1, int pageSize = 12, string? searchTerm = null)
        {
            try
            {
                var (items, totalCount) = await _haberRepository.GetHaberListeAsync(pageNumber, pageSize, searchTerm);

                var haberList = new List<HaberResponseDto>();
                foreach (var haber in items)
                {
                    var resimler = await _haberRepository.GetHaberResimleriAsync(haber.HaberId);
                    var vitrin = resimler.FirstOrDefault(r => r.IsVitrin);

                    haberList.Add(new HaberResponseDto
                    {
                        HaberId = haber.HaberId,
                        Baslik = haber.Baslik,
                        Icerik = haber.Icerik,
                        VitrinResimUrl = vitrin?.ResimUrl ?? haber.GorselUrl,
                        Resimler = resimler.Select(r => new HaberResimResponseDto
                        {
                            HaberResimId = r.HaberResimId,
                            ResimUrl = r.ResimUrl,
                            IsVitrin = r.IsVitrin,
                            Sira = r.Sira
                        }).ToList(),
                        Sira = haber.Sira,
                        YayinTarihi = haber.YayinTarihi,
                        BitisTarihi = haber.BitisTarihi
                    });
                }

                var response = new HaberListeResponseDto
                {
                    Items = haberList,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                return ApiResponseDto<HaberListeResponseDto>.SuccessResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Haber listesi getirilirken hata oluştu");
                return ApiResponseDto<HaberListeResponseDto>.ErrorResult("Haber listesi getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<HaberResponseDto?>> GetHaberByIdAsync(int haberId)
        {
            try
            {
                var haber = await _haberRepository.GetHaberByIdAsync(haberId);
                if (haber == null)
                    return ApiResponseDto<HaberResponseDto?>.ErrorResult("Haber bulunamadı");

                var resimler = await _haberRepository.GetHaberResimleriAsync(haberId);
                var vitrin = resimler.FirstOrDefault(r => r.IsVitrin);

                var result = new HaberResponseDto
                {
                    HaberId = haber.HaberId,
                    Baslik = haber.Baslik,
                    Icerik = haber.Icerik,
                    VitrinResimUrl = vitrin?.ResimUrl ?? haber.GorselUrl,
                    Resimler = resimler.Select(r => new HaberResimResponseDto
                    {
                        HaberResimId = r.HaberResimId,
                        ResimUrl = r.ResimUrl,
                        IsVitrin = r.IsVitrin,
                        Sira = r.Sira
                    }).ToList(),
                    Sira = haber.Sira,
                    YayinTarihi = haber.YayinTarihi,
                    BitisTarihi = haber.BitisTarihi
                };

                // Eğer resim yoksa ama GorselUrl varsa, onu da resim listesine ekle
                if (result.Resimler.Count == 0 && !string.IsNullOrEmpty(haber.GorselUrl))
                {
                    result.Resimler.Add(new HaberResimResponseDto
                    {
                        HaberResimId = 0,
                        ResimUrl = haber.GorselUrl,
                        IsVitrin = true,
                        Sira = 1
                    });
                }

                return ApiResponseDto<HaberResponseDto?>.SuccessResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Haber detay getirilirken hata oluştu");
                return ApiResponseDto<HaberResponseDto?>.ErrorResult("Haber detay getirilirken hata oluştu");
            }
        }

        // ─── CRUD ───────────────────────────────────────────

        public async Task<ApiResponseDto<HaberResponseDto>> CreateHaberAsync(HaberCreateRequestDto request)
        {
            try
            {
                var haber = new Haber
                {
                    Baslik = request.Baslik,
                    Icerik = request.Icerik,
                    Sira = request.Sira,
                    YayinTarihi = request.YayinTarihi,
                    BitisTarihi = request.BitisTarihi,
                    Aktiflik = request.Aktiflik
                };

                var created = await _haberRepository.CreateAsync(haber);

                var response = new HaberResponseDto
                {
                    HaberId = created.HaberId,
                    Baslik = created.Baslik,
                    Icerik = created.Icerik,
                    Sira = created.Sira,
                    YayinTarihi = created.YayinTarihi,
                    BitisTarihi = created.BitisTarihi,
                    Resimler = new List<HaberResimResponseDto>()
                };

                return ApiResponseDto<HaberResponseDto>.SuccessResult(response, "Haber başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Haber oluşturulurken hata oluştu");
                return ApiResponseDto<HaberResponseDto>.ErrorResult("Haber oluşturulurken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<HaberResponseDto>> UpdateHaberAsync(HaberUpdateRequestDto request)
        {
            try
            {
                var existing = await _haberRepository.GetHaberByIdForAdminAsync(request.HaberId);
                if (existing == null)
                    return ApiResponseDto<HaberResponseDto>.ErrorResult("Haber bulunamadı");

                existing.Baslik = request.Baslik;
                existing.Icerik = request.Icerik;
                existing.Sira = request.Sira;
                existing.YayinTarihi = request.YayinTarihi;
                existing.BitisTarihi = request.BitisTarihi;
                existing.Aktiflik = request.Aktiflik;

                var updated = await _haberRepository.UpdateAsync(existing);

                var resimler = await _haberRepository.GetHaberResimleriAsync(updated.HaberId);

                var response = new HaberResponseDto
                {
                    HaberId = updated.HaberId,
                    Baslik = updated.Baslik,
                    Icerik = updated.Icerik,
                    VitrinResimUrl = resimler.FirstOrDefault(r => r.IsVitrin)?.ResimUrl ?? updated.GorselUrl,
                    Resimler = resimler.Select(r => new HaberResimResponseDto
                    {
                        HaberResimId = r.HaberResimId,
                        ResimUrl = r.ResimUrl,
                        IsVitrin = r.IsVitrin,
                        Sira = r.Sira
                    }).ToList(),
                    Sira = updated.Sira,
                    YayinTarihi = updated.YayinTarihi,
                    BitisTarihi = updated.BitisTarihi
                };

                return ApiResponseDto<HaberResponseDto>.SuccessResult(response, "Haber başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Haber güncellenirken hata oluştu");
                return ApiResponseDto<HaberResponseDto>.ErrorResult("Haber güncellenirken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteHaberAsync(int haberId)
        {
            try
            {
                var deleted = await _haberRepository.DeleteAsync(haberId);
                if (!deleted)
                    return ApiResponseDto<bool>.ErrorResult("Haber bulunamadı");

                return ApiResponseDto<bool>.SuccessResult(true, "Haber başarıyla silindi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Haber silinirken hata oluştu");
                return ApiResponseDto<bool>.ErrorResult("Haber silinirken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<HaberListeResponseDto>> GetAdminHaberListeAsync(
            int pageNumber = 1, int pageSize = 12, string? searchTerm = null)
        {
            try
            {
                var (items, totalCount) = await _haberRepository.GetAllForAdminAsync(pageNumber, pageSize, searchTerm);

                var haberList = new List<HaberResponseDto>();
                foreach (var haber in items)
                {
                    var resimler = await _haberRepository.GetHaberResimleriAsync(haber.HaberId);
                    var vitrin = resimler.FirstOrDefault(r => r.IsVitrin);

                    haberList.Add(new HaberResponseDto
                    {
                        HaberId = haber.HaberId,
                        Baslik = haber.Baslik,
                        Icerik = haber.Icerik,
                        VitrinResimUrl = vitrin?.ResimUrl ?? haber.GorselUrl,
                        Resimler = resimler.Select(r => new HaberResimResponseDto
                        {
                            HaberResimId = r.HaberResimId,
                            ResimUrl = r.ResimUrl,
                            IsVitrin = r.IsVitrin,
                            Sira = r.Sira
                        }).ToList(),
                        Sira = haber.Sira,
                        YayinTarihi = haber.YayinTarihi,
                        BitisTarihi = haber.BitisTarihi
                    });
                }

                var response = new HaberListeResponseDto
                {
                    Items = haberList,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                return ApiResponseDto<HaberListeResponseDto>.SuccessResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Admin haber listesi getirilirken hata oluştu");
                return ApiResponseDto<HaberListeResponseDto>.ErrorResult("Admin haber listesi getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<HaberResimResponseDto>> AddResimAsync(HaberResimCreateRequestDto request)
        {
            try
            {
                var haberResim = new HaberResim
                {
                    HaberId = request.HaberId,
                    ResimUrl = request.ResimUrl,
                    IsVitrin = request.IsVitrin,
                    Sira = request.Sira,
                    Aktiflik = Aktiflik.Aktif
                };

                var created = await _haberRepository.CreateResimAsync(haberResim);

                var response = new HaberResimResponseDto
                {
                    HaberResimId = created.HaberResimId,
                    ResimUrl = created.ResimUrl,
                    IsVitrin = created.IsVitrin,
                    Sira = created.Sira
                };

                return ApiResponseDto<HaberResimResponseDto>.SuccessResult(response, "Resim başarıyla eklendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Haber resimi eklenirken hata oluştu");
                return ApiResponseDto<HaberResimResponseDto>.ErrorResult("Haber resimi eklenirken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteResimAsync(int haberResimId)
        {
            try
            {
                var deleted = await _haberRepository.DeleteResimAsync(haberResimId);
                if (!deleted)
                    return ApiResponseDto<bool>.ErrorResult("Resim bulunamadı");

                return ApiResponseDto<bool>.SuccessResult(true, "Resim başarıyla silindi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Haber resimi silinirken hata oluştu");
                return ApiResponseDto<bool>.ErrorResult("Haber resimi silinirken hata oluştu");
            }
        }
    }
}
