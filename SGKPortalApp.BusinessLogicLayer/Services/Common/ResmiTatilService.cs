using AutoMapper;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;

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

        public async Task<ApiResponseDto<int>> SyncHolidaysFromGoogleCalendarAsync(ResmiTatilSyncRequestDto request)
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

                // Tatil isimlerini Türkçeleştirme mapping
                var turkishNameMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    // Resmi Tatiller
                    { "New Year's Day", "Yılbaşı" },
                    { "New Year's Eve", "Yılbaşı Gecesi" },
                    { "National Sovereignty and Children's Day", "Ulusal Egemenlik ve Çocuk Bayramı" },
                    { "Labour Day", "Emek ve Dayanışma Günü" },
                    { "Labor Day", "Emek ve Dayanışma Günü" },
                    { "Commemoration of Atatürk, Youth and Sports Day", "Atatürk'ü Anma, Gençlik ve Spor Bayramı" },
                    { "Democracy and National Unity Day", "Demokrasi ve Millî Birlik Günü" },
                    { "Victory Day", "Zafer Bayramı" },
                    { "Republic Day", "Cumhuriyet Bayramı" },
                    { "Republic Day Eve", "Cumhuriyet Bayramı Arifesi" },
                    { "Atatürk Memorial Day", "Atatürk'ü Anma Günü" },
                    
                    // Ramazan Bayramı
                    { "Ramadan Feast Eve", "Ramazan Bayramı Arifesi" },
                    { "Ramadan Feast", "Ramazan Bayramı" },
                    { "Ramadan Feast Holiday", "Ramazan Bayramı" },
                    { "Eid al-Fitr", "Ramazan Bayramı" },
                    
                    // Kurban Bayramı
                    { "Sacrifice Feast Eve", "Kurban Bayramı Arifesi" },
                    { "Sacrifice Feast", "Kurban Bayramı" },
                    { "Sacrifice Feast Holiday", "Kurban Bayramı" },
                    { "Kurban Bayrami Day 2", "Kurban Bayramı 2. Gün" },
                    { "Kurban Bayrami Day 3", "Kurban Bayramı 3. Gün" },
                    { "Kurban Bayrami Day 4", "Kurban Bayramı 4. Gün" },
                    { "Eid al-Adha", "Kurban Bayramı" },
                    
                    // Dini Günler
                    { "1 Ramadan", "Ramazan Başlangıcı" },
                    { "Ramadan Starts", "Ramazan Başlangıcı" },
                    { "Lailat al-Qadr", "Kadir Gecesi" },
                    { "Isra and Mi'raj", "Miraç Kandili" },
                    { "Muharram", "Hicri Yılbaşı" },
                    { "Ashura", "Aşure Günü" },
                    { "The Prophet's Birthday", "Mevlid Kandili" }
                };

                // Google Calendar API kullan (resmi tatiller + dini bayramlar)
                var service = new CalendarService(new BaseClientService.Initializer
                {
                    ApiKey = "AIzaSyDBsBYRevTBzYgoxbfkpmJDOkkfUE60F3Q",
                    ApplicationName = "SGKPortalApp"
                });

                var calendars = new[]
                {
                    ("tr.turkish#holiday@group.v.calendar.google.com", TatilTipi.SabitTatil),  // Türkiye resmi tatilleri
                    ("tr.islamic#holiday@group.v.calendar.google.com", TatilTipi.DiniTatil)    // İslami tatiller
                };

                int addedCount = 0;

                foreach (var (calendarId, tatilTipi) in calendars)
                {
                    var eventsRequest = service.Events.List(calendarId);
                    eventsRequest.TimeMin = new DateTime(request.Yil, 1, 1);
                    eventsRequest.TimeMax = new DateTime(request.Yil + 1, 1, 1);
                    eventsRequest.SingleEvents = true;
                    eventsRequest.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
                    eventsRequest.ShowDeleted = false;

                    var events = await eventsRequest.ExecuteAsync();

                    foreach (var evt in events.Items)
                    {
                        if (evt.Start.Date != null) // All-day event
                        {
                            var date = DateTime.Parse(evt.Start.Date);
                            
                            // Sadece istenen yıl içindeki tatilleri al
                            if (date.Year != request.Yil)
                                continue;

                            var exists = await repo.ExistsByDateAsync(date);
                            if (!exists)
                            {
                                // Tatil adını temizle ve Türkçeleştir
                                var originalName = evt.Summary ?? "Tatil";
                                
                                // Gereksiz ekleri temizle
                                var cleanName = originalName
                                    .Replace(" (tentative)", "")
                                    .Replace(" (kesin değil)", "")
                                    .Replace(" (half-day)", "")
                                    .Replace(" (yarım gün)", "")
                                    .Trim();
                                
                                // Türkçe karşılığını bul
                                string turkishName = cleanName;
                                if (turkishNameMapping.TryGetValue(cleanName, out var mappedName))
                                {
                                    turkishName = mappedName;
                                }
                                
                                // Yarım gün kontrolü
                                bool isHalfDay = originalName.Contains("half-day", StringComparison.OrdinalIgnoreCase) || 
                                                originalName.Contains("yarım gün", StringComparison.OrdinalIgnoreCase) ||
                                                originalName.Contains("Eve", StringComparison.OrdinalIgnoreCase) ||
                                                originalName.Contains("Arifesi", StringComparison.OrdinalIgnoreCase);

                                var tatil = new ResmiTatil
                                {
                                    TatilAdi = turkishName,
                                    Tarih = date,
                                    Yil = request.Yil,
                                    TatilTipi = tatilTipi,
                                    YariGun = isHalfDay,
                                    OtomatikSenkronize = true,
                                    Aciklama = $"Google Calendar'dan otomatik senkronize edildi ({(tatilTipi == TatilTipi.SabitTatil ? "Resmi Tatil" : "Dini Bayram")})"
                                };

                                await repo.AddAsync(tatil);
                                addedCount++;
                            }
                        }
                    }
                }

                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("{Year} yılı için {Count} resmi tatil senkronize edildi", request.Yil, addedCount);
                return ApiResponseDto<int>.SuccessResult(addedCount, $"{addedCount} resmi tatil başarıyla senkronize edildi (resmi tatiller + dini bayramlar)");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Resmi tatiller senkronize edilirken hata oluştu. Yıl: {Year}", request.Yil);
                return ApiResponseDto<int>.ErrorResult($"Resmi tatiller senkronize edilirken hata oluştu: {ex.Message}");
            }
        }
    }
}
