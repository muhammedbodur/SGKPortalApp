using AutoMapper;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Services.SiramatikIslemleri
{
    public class KanalAltService : IKanalAltService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<KanalAltService> _logger;

        public KanalAltService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<KanalAltService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponseDto<List<KanalAltResponseDto>>> GetAllAsync()
        {
            try
            {
                var repository = _unitOfWork.GetRepository<IKanalAltRepository>();
                var kanalAltlar = await repository.GetAllWithDetailsAsync();
                var kanalAltDtos = _mapper.Map<List<KanalAltResponseDto>>(kanalAltlar);
                return ApiResponseDto<List<KanalAltResponseDto>>.SuccessResult(kanalAltDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllAsync error");
                return ApiResponseDto<List<KanalAltResponseDto>>.ErrorResult("Alt kanallar listelenirken hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<KanalAltResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var repository = _unitOfWork.GetRepository<IKanalAltRepository>();
                var kanalAlt = await repository.GetWithDetailsAsync(id);
                
                if (kanalAlt == null)
                    return ApiResponseDto<KanalAltResponseDto>.ErrorResult("Alt kanal bulunamadı");

                var kanalAltDto = _mapper.Map<KanalAltResponseDto>(kanalAlt);
                return ApiResponseDto<KanalAltResponseDto>.SuccessResult(kanalAltDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByIdAsync error");
                return ApiResponseDto<KanalAltResponseDto>.ErrorResult("Alt kanal getirilirken hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<KanalAltResponseDto>> CreateAsync(KanalAltKanalCreateRequestDto request)
        {
            try
            {
                var kanalAlt = _mapper.Map<KanalAlt>(request);
                var repository = _unitOfWork.Repository<KanalAlt>();
                await repository.AddAsync(kanalAlt);
                await _unitOfWork.SaveChangesAsync();

                var kanalAltDto = _mapper.Map<KanalAltResponseDto>(kanalAlt);
                return ApiResponseDto<KanalAltResponseDto>.SuccessResult(kanalAltDto, "Alt kanal başarıyla eklendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateAsync error");
                return ApiResponseDto<KanalAltResponseDto>.ErrorResult("Alt kanal eklenirken hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<KanalAltResponseDto>> UpdateAsync(int id, KanalAltKanalUpdateRequestDto request)
        {
            try
            {
                var repository = _unitOfWork.Repository<KanalAlt>();
                var kanalAlt = await repository.GetByIdAsync(id);
                
                if (kanalAlt == null)
                    return ApiResponseDto<KanalAltResponseDto>.ErrorResult("Alt kanal bulunamadı");

                _mapper.Map(request, kanalAlt);
                repository.Update(kanalAlt);
                await _unitOfWork.SaveChangesAsync();

                var kanalAltDto = _mapper.Map<KanalAltResponseDto>(kanalAlt);
                return ApiResponseDto<KanalAltResponseDto>.SuccessResult(kanalAltDto, "Alt kanal başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateAsync error");
                return ApiResponseDto<KanalAltResponseDto>.ErrorResult("Alt kanal güncellenirken hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteAsync(int id)
        {
            try
            {
                var repository = _unitOfWork.Repository<KanalAlt>();
                var kanalAlt = await repository.GetByIdAsync(id);
                
                if (kanalAlt == null)
                    return ApiResponseDto<bool>.ErrorResult("Alt kanal bulunamadı");

                repository.Delete(kanalAlt);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponseDto<bool>.SuccessResult(true, "Alt kanal başarıyla silindi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteAsync error");
                return ApiResponseDto<bool>.ErrorResult("Alt kanal silinirken hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<KanalAltResponseDto>>> GetActiveAsync()
        {
            try
            {
                var repository = _unitOfWork.GetRepository<IKanalAltRepository>();
                var kanalAltlar = await repository.GetActiveAsync();
                var kanalAltDtos = _mapper.Map<List<KanalAltResponseDto>>(kanalAltlar);
                return ApiResponseDto<List<KanalAltResponseDto>>.SuccessResult(kanalAltDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetActiveAsync error");
                return ApiResponseDto<List<KanalAltResponseDto>>.ErrorResult("Aktif alt kanallar listelenirken hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<KanalAltResponseDto>>> GetByKanalIdAsync(int kanalId)
        {
            try
            {
                var repository = _unitOfWork.GetRepository<IKanalAltRepository>();
                var kanalAltlar = await repository.GetByKanalAsync(kanalId);
                var kanalAltDtos = _mapper.Map<List<KanalAltResponseDto>>(kanalAltlar);
                return ApiResponseDto<List<KanalAltResponseDto>>.SuccessResult(kanalAltDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByKanalIdAsync error");
                return ApiResponseDto<List<KanalAltResponseDto>>.ErrorResult("Alt kanallar listelenirken hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<KanalAltResponseDto>> GetWithDetailsAsync(int id)
        {
            try
            {
                var repository = _unitOfWork.GetRepository<IKanalAltRepository>();
                var kanalAlt = await repository.GetWithDetailsAsync(id);
                
                if (kanalAlt == null)
                    return ApiResponseDto<KanalAltResponseDto>.ErrorResult("Alt kanal bulunamadı");

                var kanalAltDto = _mapper.Map<KanalAltResponseDto>(kanalAlt);
                return ApiResponseDto<KanalAltResponseDto>.SuccessResult(kanalAltDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetWithDetailsAsync error");
                return ApiResponseDto<KanalAltResponseDto>.ErrorResult("Alt kanal getirilirken hata oluştu", ex.Message);
            }
        }
    }
}
