using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.PdksIslemleri;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PdksIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Services.PdksIslemleri
{
    public class IzinMazeretTuruTanimService : IIzinMazeretTuruTanimService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<IzinMazeretTuruTanimService> _logger;

        public IzinMazeretTuruTanimService(
            IUnitOfWork unitOfWork,
            ILogger<IzinMazeretTuruTanimService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponseDto<List<IzinMazeretTuruResponseDto>>> GetAllAsync()
        {
            try
            {
                var repository = _unitOfWork.GetRepository<IIzinMazeretTuruTanimRepository>();
                var entities = await repository.GetAllActiveAsync();

                var dtos = entities.Select(MapToDto).ToList();

                return ApiResponseDto<List<IzinMazeretTuruResponseDto>>
                    .SuccessResult(dtos, $"{dtos.Count} adet izin türü bulundu");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İzin türleri getirilirken hata oluştu");
                return ApiResponseDto<List<IzinMazeretTuruResponseDto>>
                    .ErrorResult("İzin türleri getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<IzinMazeretTuruResponseDto>>> GetActiveAsync()
        {
            try
            {
                var repository = _unitOfWork.GetRepository<IIzinMazeretTuruTanimRepository>();
                var entities = await repository.GetAllActiveAsync();

                var dtos = entities.Select(MapToDto).ToList();

                return ApiResponseDto<List<IzinMazeretTuruResponseDto>>
                    .SuccessResult(dtos, $"{dtos.Count} adet aktif izin türü bulundu");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Aktif izin türleri getirilirken hata oluştu");
                return ApiResponseDto<List<IzinMazeretTuruResponseDto>>
                    .ErrorResult("Aktif izin türleri getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<IzinMazeretTuruResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var repository = _unitOfWork.GetRepository<IIzinMazeretTuruTanimRepository>();
                var entity = await repository.GetByIdAsync(id);

                if (entity == null)
                {
                    return ApiResponseDto<IzinMazeretTuruResponseDto>
                        .ErrorResult("İzin türü bulunamadı");
                }

                var dto = MapToDto(entity);

                return ApiResponseDto<IzinMazeretTuruResponseDto>
                    .SuccessResult(dto, "İzin türü başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İzin türü getirilirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<IzinMazeretTuruResponseDto>
                    .ErrorResult("İzin türü getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<IzinMazeretTuruResponseDto>> CreateAsync(IzinMazeretTuruResponseDto dto)
        {
            try
            {
                var repository = _unitOfWork.GetRepository<IIzinMazeretTuruTanimRepository>();

                var entity = new IzinMazeretTuruTanim
                {
                    TuruAdi = dto.TuruAdi,
                    KisaKod = dto.KisaKod,
                    Aciklama = dto.Aciklama,
                    BirinciOnayciGerekli = dto.BirinciOnayciGerekli,
                    IkinciOnayciGerekli = dto.IkinciOnayciGerekli,
                    PlanliIzinMi = dto.PlanliIzinMi,
                    Sira = dto.Sira,
                    IsActive = dto.IsActive,
                    RenkKodu = dto.RenkKodu
                };

                await repository.AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();

                var resultDto = MapToDto(entity);

                return ApiResponseDto<IzinMazeretTuruResponseDto>
                    .SuccessResult(resultDto, "İzin türü başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İzin türü oluşturulurken hata oluştu");
                return ApiResponseDto<IzinMazeretTuruResponseDto>
                    .ErrorResult("İzin türü oluşturulurken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<IzinMazeretTuruResponseDto>> UpdateAsync(int id, IzinMazeretTuruResponseDto dto)
        {
            try
            {
                var repository = _unitOfWork.GetRepository<IIzinMazeretTuruTanimRepository>();
                var entity = await repository.GetByIdAsync(id);

                if (entity == null)
                {
                    return ApiResponseDto<IzinMazeretTuruResponseDto>
                        .ErrorResult("İzin türü bulunamadı");
                }

                entity.TuruAdi = dto.TuruAdi;
                entity.KisaKod = dto.KisaKod;
                entity.Aciklama = dto.Aciklama;
                entity.BirinciOnayciGerekli = dto.BirinciOnayciGerekli;
                entity.IkinciOnayciGerekli = dto.IkinciOnayciGerekli;
                entity.PlanliIzinMi = dto.PlanliIzinMi;
                entity.Sira = dto.Sira;
                entity.IsActive = dto.IsActive;
                entity.RenkKodu = dto.RenkKodu;

                repository.Update(entity);
                await _unitOfWork.SaveChangesAsync();

                var resultDto = MapToDto(entity);

                return ApiResponseDto<IzinMazeretTuruResponseDto>
                    .SuccessResult(resultDto, "İzin türü başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İzin türü güncellenirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<IzinMazeretTuruResponseDto>
                    .ErrorResult("İzin türü güncellenirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> ToggleActiveAsync(int id)
        {
            try
            {
                var repository = _unitOfWork.GetRepository<IIzinMazeretTuruTanimRepository>();
                var entity = await repository.GetByIdAsync(id);

                if (entity == null)
                {
                    return ApiResponseDto<bool>.ErrorResult("İzin türü bulunamadı");
                }

                entity.IsActive = !entity.IsActive;
                repository.Update(entity);
                await _unitOfWork.SaveChangesAsync();

                var status = entity.IsActive ? "aktif" : "pasif";
                return ApiResponseDto<bool>
                    .SuccessResult(true, $"İzin türü başarıyla {status} hale getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İzin türü aktiflik durumu değiştirilirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<bool>
                    .ErrorResult("İzin türü aktiflik durumu değiştirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteAsync(int id)
        {
            try
            {
                var repository = _unitOfWork.GetRepository<IIzinMazeretTuruTanimRepository>();
                var entity = await repository.GetByIdAsync(id);

                if (entity == null)
                {
                    return ApiResponseDto<bool>.ErrorResult("İzin türü bulunamadı");
                }

                repository.Delete(entity);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponseDto<bool>
                    .SuccessResult(true, "İzin türü başarıyla silindi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İzin türü silinirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<bool>
                    .ErrorResult("İzin türü silinirken bir hata oluştu", ex.Message);
            }
        }

        private static IzinMazeretTuruResponseDto MapToDto(IzinMazeretTuruTanim entity)
        {
            return new IzinMazeretTuruResponseDto
            {
                IzinMazeretTuruId = entity.IzinMazeretTuruId,
                TuruAdi = entity.TuruAdi,
                KisaKod = entity.KisaKod,
                Aciklama = entity.Aciklama,
                BirinciOnayciGerekli = entity.BirinciOnayciGerekli,
                IkinciOnayciGerekli = entity.IkinciOnayciGerekli,
                PlanliIzinMi = entity.PlanliIzinMi,
                Sira = entity.Sira,
                IsActive = entity.IsActive,
                RenkKodu = entity.RenkKodu
            };
        }
    }
}
