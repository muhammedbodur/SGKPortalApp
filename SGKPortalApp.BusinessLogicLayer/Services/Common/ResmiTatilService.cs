using AutoMapper;
using Microsoft.Extensions.Logging;
using Nager.Date;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common;

namespace SGKPortalApp.BusinessLogicLayer.Services.Common
{
    public class ResmiTatilService : IResmiTatilService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ResmiTatilService> _logger;

        public ResmiTatilService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<ResmiTatilService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponseDto<List<ResmiTatilResponseDto>>> GetAllAsync()
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IResmiTatilRepository>();
                var tatiller = await repo.GetAllAsync();

                var response = _mapper.Map<List<ResmiTatilResponseDto>>(tatiller);
                return ApiResponseDto<List<ResmiTatilResponseDto>>.SuccessResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Resmi tatiller getirilirken hata oluştu");
                return ApiResponseDto<List<ResmiTatilResponseDto>>.ErrorResult("Resmi tatiller getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<ResmiTatilResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IResmiTatilRepository>();
                var tatil = await repo.GetByIdAsync(id);

                if (tatil == null)
                    return ApiResponseDto<ResmiTatilResponseDto>.ErrorResult("Resmi tatil bulunamadı");

                var response = _mapper.Map<ResmiTatilResponseDto>(tatil);
                return ApiResponseDto<ResmiTatilResponseDto>.SuccessResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Resmi tatil getirilirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<ResmiTatilResponseDto>.ErrorResult("Resmi tatil getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<List<ResmiTatilResponseDto>>> GetByYearAsync(int year)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IResmiTatilRepository>();
                var tatiller = await repo.GetByYearAsync(year);

                var response = _mapper.Map<List<ResmiTatilResponseDto>>(tatiller);
                return ApiResponseDto<List<ResmiTatilResponseDto>>.SuccessResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Yıla göre resmi tatiller getirilirken hata oluştu. Yıl: {Year}", year);
                return ApiResponseDto<List<ResmiTatilResponseDto>>.ErrorResult("Resmi tatiller getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<ResmiTatilResponseDto>> CreateAsync(ResmiTatilCreateRequestDto request)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IResmiTatilRepository>();

                var exists = await repo.ExistsByDateAsync(request.Tarih);
                if (exists)
                    return ApiResponseDto<ResmiTatilResponseDto>.ErrorResult("Bu tarihte zaten bir tatil tanımlı");

                var tatil = _mapper.Map<ResmiTatil>(request);
                tatil.Yil = request.Tarih.Year;
                tatil.OtomatikSenkronize = false;

                await repo.AddAsync(tatil);
                await _unitOfWork.SaveChangesAsync();

                var response = _mapper.Map<ResmiTatilResponseDto>(tatil);
                return ApiResponseDto<ResmiTatilResponseDto>.SuccessResult(response, "Resmi tatil başarıyla eklendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Resmi tatil eklenirken hata oluştu");
                return ApiResponseDto<ResmiTatilResponseDto>.ErrorResult("Resmi tatil eklenirken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<ResmiTatilResponseDto>> UpdateAsync(ResmiTatilUpdateRequestDto request)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IResmiTatilRepository>();
                var tatil = await repo.GetByIdAsync(request.TatilId);

                if (tatil == null)
                    return ApiResponseDto<ResmiTatilResponseDto>.ErrorResult("Resmi tatil bulunamadı");

                var exists = await repo.ExistsByDateAsync(request.Tarih, request.TatilId);
                if (exists)
                    return ApiResponseDto<ResmiTatilResponseDto>.ErrorResult("Bu tarihte zaten başka bir tatil tanımlı");

                _mapper.Map(request, tatil);
                tatil.Yil = request.Tarih.Year;

                repo.Update(tatil);
                await _unitOfWork.SaveChangesAsync();

                var response = _mapper.Map<ResmiTatilResponseDto>(tatil);
                return ApiResponseDto<ResmiTatilResponseDto>.SuccessResult(response, "Resmi tatil başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Resmi tatil güncellenirken hata oluştu. ID: {Id}", request.TatilId);
                return ApiResponseDto<ResmiTatilResponseDto>.ErrorResult("Resmi tatil güncellenirken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteAsync(int id)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IResmiTatilRepository>();
                var tatil = await repo.GetByIdAsync(id);

                if (tatil == null)
                    return ApiResponseDto<bool>.ErrorResult("Resmi tatil bulunamadı");

                tatil.SilindiMi = true;
                repo.Update(tatil);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponseDto<bool>.SuccessResult(true, "Resmi tatil başarıyla silindi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Resmi tatil silinirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<bool>.ErrorResult("Resmi tatil silinirken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<bool>> IsHolidayAsync(DateTime date)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IResmiTatilRepository>();
                var isHoliday = await repo.IsHolidayAsync(date);

                return ApiResponseDto<bool>.SuccessResult(isHoliday);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Tatil kontrolü yapılırken hata oluştu. Tarih: {Date}", date);
                return ApiResponseDto<bool>.ErrorResult("Tatil kontrolü yapılırken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<string>> GetHolidayNameAsync(DateTime date)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IResmiTatilRepository>();
                var holidayName = await repo.GetHolidayNameAsync(date);

                return ApiResponseDto<string>.SuccessResult(holidayName ?? string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Tatil adı getirilirken hata oluştu. Tarih: {Date}", date);
                return ApiResponseDto<string>.ErrorResult("Tatil adı getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<int>> SyncHolidaysFromNagerDateAsync(ResmiTatilSyncRequestDto request)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IResmiTatilRepository>();

                if (request.MevcutlariSil)
                {
                    var mevcutTatiller = await repo.FindAsync(t => t.Yil == request.Yil && t.OtomatikSenkronize);
                    foreach (var tatil in mevcutTatiller)
                    {
                        tatil.SilindiMi = true;
                        repo.Update(tatil);
                    }
                }

                var holidays = HolidaySystem.GetHolidays(request.Yil, "TR");
                int addedCount = 0;

                foreach (var holiday in holidays)
                {
                    var exists = await repo.ExistsByDateAsync(holiday.Date);
                    if (!exists)
                    {
                        var tatil = new ResmiTatil
                        {
                            TatilAdi = holiday.LocalName ?? holiday.EnglishName,
                            Tarih = holiday.Date,
                            Yil = request.Yil,
                            TatilTipi = holiday.NationalHoliday ? TatilTipi.SabitTatil : TatilTipi.DiniTatil,
                            YariGun = false,
                            OtomatikSenkronize = true,
                            Aciklama = "Nager.Date'ten otomatik senkronize edildi"
                        };

                        await repo.AddAsync(tatil);
                        addedCount++;
                    }
                }

                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("{Year} yılı için {Count} resmi tatil senkronize edildi", request.Yil, addedCount);
                return ApiResponseDto<int>.SuccessResult(addedCount, $"{addedCount} resmi tatil başarıyla senkronize edildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Resmi tatiller senkronize edilirken hata oluştu. Yıl: {Year}", request.Yil);
                return ApiResponseDto<int>.ErrorResult("Resmi tatiller senkronize edilirken hata oluştu");
            }
        }
    }
}
