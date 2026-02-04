using AutoMapper;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Dashboard;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common;
using SGKPortalApp.Common.Helpers;

namespace SGKPortalApp.BusinessLogicLayer.Services.Common
{
    public class HaberService : IHaberService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<HaberService> _logger;

        public HaberService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<HaberService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponseDto<List<HaberResponseDto>>> GetAllAsync()
        {
            try
            {
                var haberRepo = _unitOfWork.GetRepository<IHaberRepository>();
                var haberler = await haberRepo.GetAllAsync();

                var haberDtos = _mapper.Map<List<HaberResponseDto>>(haberler.OrderByDescending(h => h.YayinTarihi).ToList());

                return ApiResponseDto<List<HaberResponseDto>>
                    .SuccessResult(haberDtos, "Haberler başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Haber listesi getirilirken hata oluştu");
                return ApiResponseDto<List<HaberResponseDto>>
                    .ErrorResult("Haberler getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<HaberResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return ApiResponseDto<HaberResponseDto>
                        .ErrorResult("Geçersiz haber ID");

                var haberRepo = _unitOfWork.GetRepository<IHaberRepository>();
                var haber = await haberRepo.GetByIdAsync(id);

                if (haber == null)
                    return ApiResponseDto<HaberResponseDto>
                        .ErrorResult("Haber bulunamadı");

                var haberDto = _mapper.Map<HaberResponseDto>(haber);

                return ApiResponseDto<HaberResponseDto>
                    .SuccessResult(haberDto, "Haber başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Haber getirilirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<HaberResponseDto>
                    .ErrorResult("Haber getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<HaberResponseDto>> GetByIdWithGorsellerAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return ApiResponseDto<HaberResponseDto>
                        .ErrorResult("Geçersiz haber ID");

                var haberRepo = _unitOfWork.GetRepository<IHaberRepository>();
                var haber = await haberRepo.GetByIdWithGorsellerAsync(id);

                if (haber == null)
                    return ApiResponseDto<HaberResponseDto>
                        .ErrorResult("Haber bulunamadı");

                var haberDto = _mapper.Map<HaberResponseDto>(haber);
                haberDto.Gorseller = _mapper.Map<List<HaberGorselResponseDto>>(haber.Gorseller.ToList());

                return ApiResponseDto<HaberResponseDto>
                    .SuccessResult(haberDto, "Haber başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Haber getirilirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<HaberResponseDto>
                    .ErrorResult("Haber getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<HaberResponseDto>> CreateAsync(HaberCreateRequestDto request)
        {
            try
            {
                var haberRepo = _unitOfWork.GetRepository<IHaberRepository>();

                var haber = _mapper.Map<Haber>(request);
                await haberRepo.AddAsync(haber);
                await _unitOfWork.SaveChangesAsync();

                var haberDto = _mapper.Map<HaberResponseDto>(haber);
                _logger.LogInformation("Yeni haber oluşturuldu. ID: {Id}", haber.HaberId);

                return ApiResponseDto<HaberResponseDto>
                    .SuccessResult(haberDto, "Haber başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Haber oluşturulurken hata oluştu");
                return ApiResponseDto<HaberResponseDto>
                    .ErrorResult("Haber oluşturulurken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<HaberResponseDto>> UpdateAsync(int id, HaberUpdateRequestDto request)
        {
            try
            {
                if (id <= 0)
                    return ApiResponseDto<HaberResponseDto>
                        .ErrorResult("Geçersiz haber ID");

                var haberRepo = _unitOfWork.GetRepository<IHaberRepository>();
                var haber = await haberRepo.GetByIdAsync(id);

                if (haber == null)
                    return ApiResponseDto<HaberResponseDto>
                        .ErrorResult("Haber bulunamadı");

                haber.Baslik = request.Baslik;
                haber.Icerik = request.Icerik;
                haber.GorselUrl = request.GorselUrl;
                haber.Sira = request.Sira;
                haber.YayinTarihi = request.YayinTarihi;
                haber.BitisTarihi = request.BitisTarihi;
                haber.Aktiflik = request.Aktiflik;
                haber.DuzenlenmeTarihi = DateTimeHelper.Now;

                haberRepo.Update(haber);
                await _unitOfWork.SaveChangesAsync();

                var haberDto = _mapper.Map<HaberResponseDto>(haber);
                _logger.LogInformation("Haber güncellendi. ID: {Id}", id);

                return ApiResponseDto<HaberResponseDto>
                    .SuccessResult(haberDto, "Haber başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Haber güncellenirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<HaberResponseDto>
                    .ErrorResult("Haber güncellenirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return ApiResponseDto<bool>
                        .ErrorResult("Geçersiz haber ID");

                var haberRepo = _unitOfWork.GetRepository<IHaberRepository>();
                var haber = await haberRepo.GetByIdAsync(id);

                if (haber == null)
                    return ApiResponseDto<bool>
                        .ErrorResult("Haber bulunamadı");

                haberRepo.Delete(haber);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Haber silindi. ID: {Id}", id);
                return ApiResponseDto<bool>
                    .SuccessResult(true, "Haber başarıyla silindi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Haber silinirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<bool>
                    .ErrorResult("Haber silinirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<HaberResponseDto>>> GetSliderHaberlerAsync(int count = 5)
        {
            try
            {
                var haberRepo = _unitOfWork.GetRepository<IHaberRepository>();
                var haberler = await haberRepo.GetSliderHaberlerAsync(count);

                var haberDtos = _mapper.Map<List<HaberResponseDto>>(haberler.ToList());

                return ApiResponseDto<List<HaberResponseDto>>
                    .SuccessResult(haberDtos, "Slider haberleri başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Slider haberleri getirilirken hata oluştu");
                return ApiResponseDto<List<HaberResponseDto>>
                    .ErrorResult("Slider haberleri getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<HaberResponseDto>>> GetListeHaberlerAsync(int count = 10)
        {
            try
            {
                var haberRepo = _unitOfWork.GetRepository<IHaberRepository>();
                var haberler = await haberRepo.GetListeHaberlerAsync(count);

                var haberDtos = _mapper.Map<List<HaberResponseDto>>(haberler.ToList());

                return ApiResponseDto<List<HaberResponseDto>>
                    .SuccessResult(haberDtos, "Liste haberleri başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Liste haberleri getirilirken hata oluştu");
                return ApiResponseDto<List<HaberResponseDto>>
                    .ErrorResult("Liste haberleri getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<HaberResponseDto>>> GetAktifHaberlerAsync()
        {
            try
            {
                var haberRepo = _unitOfWork.GetRepository<IHaberRepository>();
                var haberler = await haberRepo.GetActiveHaberlerAsync();

                var haberDtos = _mapper.Map<List<HaberResponseDto>>(haberler.ToList());

                return ApiResponseDto<List<HaberResponseDto>>
                    .SuccessResult(haberDtos, "Aktif haberler başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Aktif haberler getirilirken hata oluştu");
                return ApiResponseDto<List<HaberResponseDto>>
                    .ErrorResult("Aktif haberler getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<HaberGorselResponseDto>> AddGorselAsync(int haberId, string gorselUrl, bool vitrinFoto = false)
        {
            try
            {
                var haberRepo = _unitOfWork.GetRepository<IHaberRepository>();
                var haber = await haberRepo.GetByIdAsync(haberId);

                if (haber == null)
                    return ApiResponseDto<HaberGorselResponseDto>
                        .ErrorResult("Haber bulunamadı");

                var gorsel = new HaberGorsel
                {
                    HaberId = haberId,
                    GorselUrl = gorselUrl,
                    VitrinFoto = vitrinFoto,
                    Sira = 1
                };

                _unitOfWork.Context.Set<HaberGorsel>().Add(gorsel);

                // Eğer vitrin foto olarak işaretlendiyse, ana haber görsel URL'ini de güncelle
                if (vitrinFoto)
                {
                    haber.GorselUrl = gorselUrl;
                    haberRepo.Update(haber);
                }

                await _unitOfWork.SaveChangesAsync();

                var gorselDto = _mapper.Map<HaberGorselResponseDto>(gorsel);
                _logger.LogInformation("Haber görseli eklendi. HaberId: {HaberId}, GorselId: {GorselId}", haberId, gorsel.HaberGorselId);

                return ApiResponseDto<HaberGorselResponseDto>
                    .SuccessResult(gorselDto, "Görsel başarıyla eklendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Görsel eklenirken hata oluştu. HaberId: {HaberId}", haberId);
                return ApiResponseDto<HaberGorselResponseDto>
                    .ErrorResult("Görsel eklenirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> RemoveGorselAsync(int haberGorselId)
        {
            try
            {
                var gorselSet = _unitOfWork.Context.Set<HaberGorsel>();
                var gorsel = await gorselSet.FindAsync(haberGorselId);

                if (gorsel == null)
                    return ApiResponseDto<bool>
                        .ErrorResult("Görsel bulunamadı");

                gorsel.SilindiMi = true;
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Haber görseli silindi. GorselId: {GorselId}", haberGorselId);
                return ApiResponseDto<bool>
                    .SuccessResult(true, "Görsel başarıyla silindi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Görsel silinirken hata oluştu. GorselId: {GorselId}", haberGorselId);
                return ApiResponseDto<bool>
                    .ErrorResult("Görsel silinirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> SetVitrinFotoAsync(int haberGorselId)
        {
            try
            {
                var gorselSet = _unitOfWork.Context.Set<HaberGorsel>();
                var gorsel = await gorselSet.FindAsync(haberGorselId);

                if (gorsel == null)
                    return ApiResponseDto<bool>
                        .ErrorResult("Görsel bulunamadı");

                // Diğer görsellerin vitrin durumunu kaldır
                var digerleri = gorselSet.Where(g => g.HaberId == gorsel.HaberId && g.HaberGorselId != haberGorselId && !g.SilindiMi);
                foreach (var diger in digerleri)
                {
                    diger.VitrinFoto = false;
                }

                gorsel.VitrinFoto = true;

                // Haber'in ana görsel URL'ini güncelle
                var haberRepo = _unitOfWork.GetRepository<IHaberRepository>();
                var haber = await haberRepo.GetByIdAsync(gorsel.HaberId);
                if (haber != null)
                {
                    haber.GorselUrl = gorsel.GorselUrl;
                    haberRepo.Update(haber);
                }

                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Vitrin fotoğrafı güncellendi. GorselId: {GorselId}", haberGorselId);
                return ApiResponseDto<bool>
                    .SuccessResult(true, "Vitrin fotoğrafı güncellendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Vitrin fotoğrafı güncellenirken hata oluştu. GorselId: {GorselId}", haberGorselId);
                return ApiResponseDto<bool>
                    .ErrorResult("Vitrin fotoğrafı güncellenirken bir hata oluştu", ex.Message);
            }
        }
    }
}
