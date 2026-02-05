using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Dashboard;
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
    }
}
