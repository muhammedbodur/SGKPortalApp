using AutoMapper;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Dashboard;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Services.Common
{
    /// <summary>
    /// Dashboard Service Implementation
    /// Ana sayfa için gerekli tüm verileri sağlar
    /// </summary>
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<DashboardService> _logger;

        public DashboardService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<DashboardService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponseDto<DashboardDataResponseDto>> GetDashboardDataAsync()
        {
            try
            {
                var dashboardData = new DashboardDataResponseDto();
                
                dashboardData.SliderDuyurular = await GetSliderDuyurularInternalAsync(5);
                
                dashboardData.ListeDuyurular = await GetListeDuyurularInternalAsync(10);
                
                dashboardData.SikKullanilanProgramlar = await GetSikKullanilanProgramlarInternalAsync(8);
                
                dashboardData.OnemliLinkler = await GetOnemliLinklerInternalAsync(10);
                
                dashboardData.GununMenusu = await GetGununMenusuInternalAsync();
                
                dashboardData.BugunDoganlar = await GetBugunDoganlarInternalAsync();

                return ApiResponseDto<DashboardDataResponseDto>.SuccessResult(dashboardData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dashboard verileri getirilirken hata oluştu");
                return ApiResponseDto<DashboardDataResponseDto>.ErrorResult("Dashboard verileri getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<List<DuyuruResponseDto>>> GetSliderDuyurularAsync(int count = 5)
        {
            try
            {
                var result = await GetSliderDuyurularInternalAsync(count);
                return ApiResponseDto<List<DuyuruResponseDto>>.SuccessResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Slider duyuruları getirilirken hata oluştu");
                return ApiResponseDto<List<DuyuruResponseDto>>.ErrorResult("Slider duyuruları getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<List<DuyuruResponseDto>>> GetListeDuyurularAsync(int count = 10)
        {
            try
            {
                var result = await GetListeDuyurularInternalAsync(count);
                return ApiResponseDto<List<DuyuruResponseDto>>.SuccessResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Liste duyuruları getirilirken hata oluştu");
                return ApiResponseDto<List<DuyuruResponseDto>>.ErrorResult("Liste duyuruları getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<List<SikKullanilanProgramResponseDto>>> GetSikKullanilanProgramlarAsync(int count = 8)
        {
            try
            {
                var result = await GetSikKullanilanProgramlarInternalAsync(count);
                return ApiResponseDto<List<SikKullanilanProgramResponseDto>>.SuccessResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sık kullanılan programlar getirilirken hata oluştu");
                return ApiResponseDto<List<SikKullanilanProgramResponseDto>>.ErrorResult("Sık kullanılan programlar getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<List<OnemliLinkResponseDto>>> GetOnemliLinklerAsync(int count = 10)
        {
            try
            {
                var result = await GetOnemliLinklerInternalAsync(count);
                return ApiResponseDto<List<OnemliLinkResponseDto>>.SuccessResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Önemli linkler getirilirken hata oluştu");
                return ApiResponseDto<List<OnemliLinkResponseDto>>.ErrorResult("Önemli linkler getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<GununMenusuResponseDto?>> GetGununMenusuAsync()
        {
            try
            {
                var result = await GetGununMenusuInternalAsync();
                return ApiResponseDto<GununMenusuResponseDto?>.SuccessResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Günün menüsü getirilirken hata oluştu");
                return ApiResponseDto<GununMenusuResponseDto?>.ErrorResult("Günün menüsü getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<List<BugunDoganResponseDto>>> GetBugunDoganlarAsync()
        {
            try
            {
                var result = await GetBugunDoganlarInternalAsync();
                return ApiResponseDto<List<BugunDoganResponseDto>>.SuccessResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bugün doğanlar getirilirken hata oluştu");
                return ApiResponseDto<List<BugunDoganResponseDto>>.ErrorResult("Bugün doğanlar getirilirken hata oluştu");
            }
        }

        #region Internal Methods

        private async Task<List<DuyuruResponseDto>> GetSliderDuyurularInternalAsync(int count)
        {
            var repo = _unitOfWork.GetRepository<IDuyuruRepository>();
            var duyurular = await repo.GetSliderDuyurularAsync(count);
            return _mapper.Map<List<DuyuruResponseDto>>(duyurular);
        }

        private async Task<List<DuyuruResponseDto>> GetListeDuyurularInternalAsync(int count)
        {
            var repo = _unitOfWork.GetRepository<IDuyuruRepository>();
            var duyurular = await repo.GetListeDuyurularAsync(count);
            return _mapper.Map<List<DuyuruResponseDto>>(duyurular);
        }

        private async Task<List<SikKullanilanProgramResponseDto>> GetSikKullanilanProgramlarInternalAsync(int count)
        {
            var repo = _unitOfWork.GetRepository<ISikKullanilanProgramRepository>();
            var programlar = await repo.GetDashboardProgramsAsync(count);
            return _mapper.Map<List<SikKullanilanProgramResponseDto>>(programlar);
        }

        private async Task<List<OnemliLinkResponseDto>> GetOnemliLinklerInternalAsync(int count)
        {
            var repo = _unitOfWork.GetRepository<IOnemliLinkRepository>();
            var linkler = await repo.GetDashboardLinksAsync(count);
            return _mapper.Map<List<OnemliLinkResponseDto>>(linkler);
        }

        private async Task<GununMenusuResponseDto?> GetGununMenusuInternalAsync()
        {
            var repo = _unitOfWork.GetRepository<IGununMenusuRepository>();
            var menu = await repo.GetTodaysMenuAsync();
            return menu != null ? _mapper.Map<GununMenusuResponseDto>(menu) : null;
        }

        private async Task<List<BugunDoganResponseDto>> GetBugunDoganlarInternalAsync()
        {
            var repo = _unitOfWork.GetRepository<IPersonelRepository>();
            var personeller = await repo.GetBugunDoganlarAsync();

            return personeller.Select(p => new BugunDoganResponseDto
            {
                TcKimlikNo = p.TcKimlikNo,
                AdSoyad = p.AdSoyad,
                Resim = GetPersonelImagePath(p.Resim),
                DepartmanAdi = p.Departman?.DepartmanAdi,
                ServisAdi = p.Servis?.ServisAdi,
                DogumTarihi = p.DogumTarihi
            }).ToList();
        }

        private static string? GetPersonelImagePath(string? resim)
        {
            if (string.IsNullOrWhiteSpace(resim))
                return null;

            if (resim.StartsWith("/") || resim.StartsWith("http://") || resim.StartsWith("https://"))
                return resim;

            if (resim.StartsWith("images/"))
                return "/" + resim;

            return "/images/avatars/" + resim;
        }

        #endregion
    }
}
