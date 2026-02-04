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
    public class DuyuruService : IDuyuruService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<DuyuruService> _logger;

        public DuyuruService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<DuyuruService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponseDto<List<DuyuruResponseDto>>> GetAllAsync()
        {
            try
            {
                var duyuruRepo = _unitOfWork.GetRepository<IDuyuruRepository>();
                var duyurular = await duyuruRepo.GetAllAsync();

                var duyuruDtos = _mapper.Map<List<DuyuruResponseDto>>(duyurular.OrderByDescending(d => d.YayinTarihi).ToList());

                return ApiResponseDto<List<DuyuruResponseDto>>
                    .SuccessResult(duyuruDtos, "Duyurular başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Duyuru listesi getirilirken hata oluştu");
                return ApiResponseDto<List<DuyuruResponseDto>>
                    .ErrorResult("Duyurular getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<DuyuruResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return ApiResponseDto<DuyuruResponseDto>
                        .ErrorResult("Geçersiz duyuru ID");

                var duyuruRepo = _unitOfWork.GetRepository<IDuyuruRepository>();
                var duyuru = await duyuruRepo.GetByIdAsync(id);

                if (duyuru == null)
                    return ApiResponseDto<DuyuruResponseDto>
                        .ErrorResult("Duyuru bulunamadı");

                var duyuruDto = _mapper.Map<DuyuruResponseDto>(duyuru);

                return ApiResponseDto<DuyuruResponseDto>
                    .SuccessResult(duyuruDto, "Duyuru başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Duyuru getirilirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<DuyuruResponseDto>
                    .ErrorResult("Duyuru getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<DuyuruResponseDto>> CreateAsync(DuyuruCreateRequestDto request)
        {
            try
            {
                var duyuruRepo = _unitOfWork.GetRepository<IDuyuruRepository>();

                var duyuru = _mapper.Map<Duyuru>(request);
                await duyuruRepo.AddAsync(duyuru);
                await _unitOfWork.SaveChangesAsync();

                var duyuruDto = _mapper.Map<DuyuruResponseDto>(duyuru);
                _logger.LogInformation("Yeni duyuru oluşturuldu. ID: {Id}", duyuru.DuyuruId);

                return ApiResponseDto<DuyuruResponseDto>
                    .SuccessResult(duyuruDto, "Duyuru başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Duyuru oluşturulurken hata oluştu");
                return ApiResponseDto<DuyuruResponseDto>
                    .ErrorResult("Duyuru oluşturulurken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<DuyuruResponseDto>> UpdateAsync(int id, DuyuruUpdateRequestDto request)
        {
            try
            {
                if (id <= 0)
                    return ApiResponseDto<DuyuruResponseDto>
                        .ErrorResult("Geçersiz duyuru ID");

                var duyuruRepo = _unitOfWork.GetRepository<IDuyuruRepository>();
                var duyuru = await duyuruRepo.GetByIdAsync(id);

                if (duyuru == null)
                    return ApiResponseDto<DuyuruResponseDto>
                        .ErrorResult("Duyuru bulunamadı");

                duyuru.Baslik = request.Baslik;
                duyuru.Icerik = request.Icerik;
                duyuru.GorselUrl = request.GorselUrl;
                duyuru.Sira = request.Sira;
                duyuru.YayinTarihi = request.YayinTarihi;
                duyuru.BitisTarihi = request.BitisTarihi;
                duyuru.Aktiflik = request.Aktiflik;
                duyuru.DuzenlenmeTarihi = DateTimeHelper.Now;

                duyuruRepo.Update(duyuru);
                await _unitOfWork.SaveChangesAsync();

                var duyuruDto = _mapper.Map<DuyuruResponseDto>(duyuru);
                _logger.LogInformation("Duyuru güncellendi. ID: {Id}", id);

                return ApiResponseDto<DuyuruResponseDto>
                    .SuccessResult(duyuruDto, "Duyuru başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Duyuru güncellenirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<DuyuruResponseDto>
                    .ErrorResult("Duyuru güncellenirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return ApiResponseDto<bool>
                        .ErrorResult("Geçersiz duyuru ID");

                var duyuruRepo = _unitOfWork.GetRepository<IDuyuruRepository>();
                var duyuru = await duyuruRepo.GetByIdAsync(id);

                if (duyuru == null)
                    return ApiResponseDto<bool>
                        .ErrorResult("Duyuru bulunamadı");

                duyuruRepo.Delete(duyuru);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Duyuru silindi. ID: {Id}", id);
                return ApiResponseDto<bool>
                    .SuccessResult(true, "Duyuru başarıyla silindi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Duyuru silinirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<bool>
                    .ErrorResult("Duyuru silinirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<DuyuruResponseDto>>> GetSliderDuyurularAsync(int count = 5)
        {
            try
            {
                var duyuruRepo = _unitOfWork.GetRepository<IDuyuruRepository>();
                var duyurular = await duyuruRepo.GetSliderDuyurularAsync(count);

                var duyuruDtos = _mapper.Map<List<DuyuruResponseDto>>(duyurular.ToList());

                return ApiResponseDto<List<DuyuruResponseDto>>
                    .SuccessResult(duyuruDtos, "Slider duyuruları başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Slider duyuruları getirilirken hata oluştu");
                return ApiResponseDto<List<DuyuruResponseDto>>
                    .ErrorResult("Slider duyuruları getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<DuyuruResponseDto>>> GetListeDuyurularAsync(int count = 10)
        {
            try
            {
                var duyuruRepo = _unitOfWork.GetRepository<IDuyuruRepository>();
                var duyurular = await duyuruRepo.GetListeDuyurularAsync(count);

                var duyuruDtos = _mapper.Map<List<DuyuruResponseDto>>(duyurular.ToList());

                return ApiResponseDto<List<DuyuruResponseDto>>
                    .SuccessResult(duyuruDtos, "Liste duyuruları başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Liste duyuruları getirilirken hata oluştu");
                return ApiResponseDto<List<DuyuruResponseDto>>
                    .ErrorResult("Liste duyuruları getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<DuyuruResponseDto>>> GetAktifDuyurularAsync()
        {
            try
            {
                var duyuruRepo = _unitOfWork.GetRepository<IDuyuruRepository>();
                var duyurular = await duyuruRepo.GetActiveDuyurularAsync();

                var duyuruDtos = _mapper.Map<List<DuyuruResponseDto>>(duyurular.ToList());

                return ApiResponseDto<List<DuyuruResponseDto>>
                    .SuccessResult(duyuruDtos, "Aktif duyurular başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Aktif duyurular getirilirken hata oluştu");
                return ApiResponseDto<List<DuyuruResponseDto>>
                    .ErrorResult("Aktif duyurular getirilirken bir hata oluştu", ex.Message);
            }
        }
    }
}
