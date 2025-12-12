using AutoMapper;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Exceptions;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;

namespace SGKPortalApp.BusinessLogicLayer.Services.PersonelIslemleri
{
    public class AtanmaNedeniService : IAtanmaNedeniService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<AtanmaNedeniService> _logger;

        public AtanmaNedeniService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<AtanmaNedeniService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponseDto<List<AtanmaNedeniResponseDto>>> GetAllAsync()
        {
            try
            {
                var atanmaNedenleri = await _unitOfWork.Repository<AtanmaNedenleri>().GetAllAsync();
                var atanmaNedenleriDto = _mapper.Map<List<AtanmaNedeniResponseDto>>(atanmaNedenleri);

                return ApiResponseDto<List<AtanmaNedeniResponseDto>>
                    .SuccessResult(atanmaNedenleriDto, "Atanma nedenleri başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Atanma nedenleri getirilirken hata oluştu");
                return ApiResponseDto<List<AtanmaNedeniResponseDto>>
                    .ErrorResult("Atanma nedenleri getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<AtanmaNedeniResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var atanmaNedeni = await _unitOfWork.Repository<AtanmaNedenleri>().GetByIdAsync(id);
                
                if (atanmaNedeni == null)
                    return ApiResponseDto<AtanmaNedeniResponseDto>.ErrorResult("Atanma nedeni bulunamadı");

                var atanmaNedeniDto = _mapper.Map<AtanmaNedeniResponseDto>(atanmaNedeni);
                return ApiResponseDto<AtanmaNedeniResponseDto>
                    .SuccessResult(atanmaNedeniDto, "Atanma nedeni başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Atanma nedeni getirilirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<AtanmaNedeniResponseDto>
                    .ErrorResult("Atanma nedeni getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<AtanmaNedeniResponseDto>> CreateAsync(AtanmaNedeniCreateRequestDto request)
        {
            try
            {
                var atanmaNedeni = _mapper.Map<AtanmaNedenleri>(request);
                await _unitOfWork.Repository<AtanmaNedenleri>().AddAsync(atanmaNedeni);
                await _unitOfWork.SaveChangesAsync();

                var atanmaNedeniDto = _mapper.Map<AtanmaNedeniResponseDto>(atanmaNedeni);
                return ApiResponseDto<AtanmaNedeniResponseDto>
                    .SuccessResult(atanmaNedeniDto, "Atanma nedeni başarıyla oluşturuldu");
            }
            catch (DatabaseException ex)
            {
                _logger.LogWarning(ex, "Veritabanı kısıtlama hatası: {ErrorType}", ex.ErrorType);
                return ApiResponseDto<AtanmaNedeniResponseDto>
                    .ErrorResult(ex.UserFriendlyMessage, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Atanma nedeni oluşturulurken hata oluştu");
                return ApiResponseDto<AtanmaNedeniResponseDto>
                    .ErrorResult("Atanma nedeni oluşturulurken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<AtanmaNedeniResponseDto>> UpdateAsync(int id, AtanmaNedeniUpdateRequestDto request)
        {
            try
            {
                var atanmaNedeni = await _unitOfWork.Repository<AtanmaNedenleri>().GetByIdAsync(id);
                
                if (atanmaNedeni == null)
                    return ApiResponseDto<AtanmaNedeniResponseDto>.ErrorResult("Atanma nedeni bulunamadı");

                _mapper.Map(request, atanmaNedeni);
                _unitOfWork.Repository<AtanmaNedenleri>().Update(atanmaNedeni);
                await _unitOfWork.SaveChangesAsync();

                var atanmaNedeniDto = _mapper.Map<AtanmaNedeniResponseDto>(atanmaNedeni);
                return ApiResponseDto<AtanmaNedeniResponseDto>
                    .SuccessResult(atanmaNedeniDto, "Atanma nedeni başarıyla güncellendi");
            }
            catch (DatabaseException ex)
            {
                _logger.LogWarning(ex, "Veritabanı kısıtlama hatası: {ErrorType}", ex.ErrorType);
                return ApiResponseDto<AtanmaNedeniResponseDto>
                    .ErrorResult(ex.UserFriendlyMessage, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Atanma nedeni güncellenirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<AtanmaNedeniResponseDto>
                    .ErrorResult("Atanma nedeni güncellenirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteAsync(int id)
        {
            try
            {
                var atanmaNedeni = await _unitOfWork.Repository<AtanmaNedenleri>().GetByIdAsync(id);

                if (atanmaNedeni == null)
                    return ApiResponseDto<bool>.ErrorResult("Atanma nedeni bulunamadı");

                // Atanma nedenine bağlı aktif personel var mı kontrol et
                var personelCount = await _unitOfWork.Repository<Personel>()
                    .CountAsync(p => p.AtanmaNedeniId == id && !p.SilindiMi && p.PersonelAktiflikDurum == PersonelAktiflikDurum.Aktif);

                if (personelCount > 0)
                    return ApiResponseDto<bool>
                        .ErrorResult($"Bu atanma nedenine bağlı {personelCount} personel bulunmaktadır. Önce personelleri düzenleyiniz");

                _unitOfWork.Repository<AtanmaNedenleri>().Delete(atanmaNedeni);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponseDto<bool>
                    .SuccessResult(true, "Atanma nedeni başarıyla silindi");
            }
            catch (DatabaseException ex)
            {
                _logger.LogWarning(ex, "Veritabanı kısıtlama hatası: {ErrorType}", ex.ErrorType);
                return ApiResponseDto<bool>
                    .ErrorResult(ex.UserFriendlyMessage, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Atanma nedeni silinirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<bool>
                    .ErrorResult("Atanma nedeni silinirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<int>> GetPersonelCountAsync(int atanmaNedeniId)
        {
            try
            {
                var count = await _unitOfWork.Repository<Personel>()
                    .CountAsync(p => p.AtanmaNedeniId == atanmaNedeniId && !p.SilindiMi && p.PersonelAktiflikDurum == PersonelAktiflikDurum.Aktif);

                return ApiResponseDto<int>
                    .SuccessResult(count, "Personel sayısı başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel sayısı alınırken hata oluştu. AtanmaNedeniId: {Id}", atanmaNedeniId);
                return ApiResponseDto<int>
                    .ErrorResult("Personel sayısı alınırken bir hata oluştu", ex.Message);
            }
        }
    }
}
