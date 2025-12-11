using AutoMapper;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Services.SiramatikIslemleri
{
    public class KanalService : IKanalService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<KanalService> _logger;

        public KanalService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<KanalService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponseDto<List<KanalResponseDto>>> GetAllAsync()
        {
            try
            {
                var kanalRepo = _unitOfWork.GetRepository<IKanalRepository>();
                var kanallar = await kanalRepo.GetAllAsync();
                
                var kanalDtos = _mapper.Map<List<KanalResponseDto>>(kanallar);

                return ApiResponseDto<List<KanalResponseDto>>
                    .SuccessResult(kanalDtos, "Kanallar başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kanal listesi getirilirken hata oluştu");
                return ApiResponseDto<List<KanalResponseDto>>
                    .ErrorResult("Kanallar getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<KanalResponseDto>> GetByIdAsync(int kanalId)
        {
            try
            {
                if (kanalId <= 0)
                {
                    return ApiResponseDto<KanalResponseDto>
                        .ErrorResult("Geçersiz kanal ID");
                }

                var kanalRepo = _unitOfWork.GetRepository<IKanalRepository>();
                var kanal = await kanalRepo.GetByIdAsync(kanalId);

                if (kanal == null)
                {
                    return ApiResponseDto<KanalResponseDto>
                        .ErrorResult("Kanal bulunamadı");
                }

                var kanalDto = _mapper.Map<KanalResponseDto>(kanal);

                return ApiResponseDto<KanalResponseDto>
                    .SuccessResult(kanalDto, "Kanal başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kanal getirilirken hata oluştu. ID: {KanalId}", kanalId);
                return ApiResponseDto<KanalResponseDto>
                    .ErrorResult("Kanal getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<KanalResponseDto>> CreateAsync(KanalCreateRequestDto request)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(request.KanalAdi))
                {
                    return ApiResponseDto<KanalResponseDto>
                        .ErrorResult("Kanal adı boş olamaz");
                }

                var kanal = _mapper.Map<Kanal>(request);
                kanal.Aktiflik = Aktiflik.Aktif;

                var kanalRepo = _unitOfWork.GetRepository<IKanalRepository>();
                await kanalRepo.AddAsync(kanal);
                await _unitOfWork.SaveChangesAsync();

                var kanalDto = _mapper.Map<KanalResponseDto>(kanal);

                _logger.LogInformation("Yeni kanal oluşturuldu. ID: {KanalId}, Ad: {KanalAdi}", 
                    kanal.KanalId, kanal.KanalAdi);

                return ApiResponseDto<KanalResponseDto>
                    .SuccessResult(kanalDto, "Kanal başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kanal oluşturulurken hata oluştu. Ad: {KanalAdi}", request.KanalAdi);
                return ApiResponseDto<KanalResponseDto>
                    .ErrorResult("Kanal oluşturulurken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<KanalResponseDto>> UpdateAsync(int kanalId, KanalUpdateRequestDto request)
        {
            try
            {
                // Validation
                if (kanalId <= 0)
                {
                    return ApiResponseDto<KanalResponseDto>
                        .ErrorResult("Geçersiz kanal ID");
                }

                if (string.IsNullOrWhiteSpace(request.KanalAdi))
                {
                    return ApiResponseDto<KanalResponseDto>
                        .ErrorResult("Kanal adı boş olamaz");
                }

                var kanalRepo = _unitOfWork.GetRepository<IKanalRepository>();
                var kanal = await kanalRepo.GetByIdAsync(kanalId);

                if (kanal == null)
                {
                    return ApiResponseDto<KanalResponseDto>
                        .ErrorResult("Kanal bulunamadı");
                }

                var oldAktiflik = kanal.Aktiflik;

                // Update
                kanal.KanalAdi = request.KanalAdi.Trim();
                kanal.Aktiflik = request.Aktiflik;
                kanal.DuzenlenmeTarihi = DateTime.Now;

                kanalRepo.Update(kanal);

                // Cascade: Aktiflik değişmişse child kayıtları da güncelle
                if (oldAktiflik != request.Aktiflik)
                {
                    await CascadeAktiflikUpdateAsync(kanalId, request.Aktiflik);
                }

                await _unitOfWork.SaveChangesAsync();

                var kanalDto = _mapper.Map<KanalResponseDto>(kanal);

                _logger.LogInformation("Kanal güncellendi. ID: {KanalId}, Ad: {KanalAdi}",
                    kanal.KanalId, kanal.KanalAdi);

                return ApiResponseDto<KanalResponseDto>
                    .SuccessResult(kanalDto, "Kanal başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kanal güncellenirken hata oluştu. ID: {KanalId}", kanalId);
                return ApiResponseDto<KanalResponseDto>
                    .ErrorResult("Kanal güncellenirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteAsync(int kanalId)
        {
            try
            {
                if (kanalId <= 0)
                {
                    return ApiResponseDto<bool>
                        .ErrorResult("Geçersiz kanal ID");
                }

                var kanalRepo = _unitOfWork.GetRepository<IKanalRepository>();
                var kanal = await kanalRepo.GetByIdAsync(kanalId);

                if (kanal == null)
                {
                    return ApiResponseDto<bool>
                        .ErrorResult("Kanal bulunamadı");
                }

                // Cascade: Child kayıtları da sil (soft delete)
                await CascadeDeleteAsync(kanalId);

                // Soft delete
                kanal.SilindiMi = true;
                kanal.DuzenlenmeTarihi = DateTime.Now;

                kanalRepo.Update(kanal);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Kanal silindi. ID: {KanalId}", kanalId);

                return ApiResponseDto<bool>
                    .SuccessResult(true, "Kanal işlem başarıyla silindi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kanal silinirken hata oluştu. ID: {KanalId}", kanalId);
                return ApiResponseDto<bool>
                    .ErrorResult("Kanal silinirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<KanalResponseDto>>> GetActiveAsync()
        {
            try
            {
                var kanalRepo = _unitOfWork.GetRepository<IKanalRepository>();
                var kanallar = await kanalRepo.GetActiveAsync();
                
                var kanalDtos = _mapper.Map<List<KanalResponseDto>>(kanallar);

                return ApiResponseDto<List<KanalResponseDto>>
                    .SuccessResult(kanalDtos, "Aktif kanallar başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Aktif kanal listesi getirilirken hata oluştu");
                return ApiResponseDto<List<KanalResponseDto>>
                    .ErrorResult("Aktif kanallar getirilirken bir hata oluştu", ex.Message);
            }
        }

        /// <summary>
        /// Cascade delete: Kanal silindiğinde child kayıtları da siler
        /// </summary>
        private async Task CascadeDeleteAsync(int kanalId)
        {
            // KanalAlt kayıtlarını soft delete
            var kanalAltRepo = _unitOfWork.Repository<KanalAlt>();
            var kanalAltlar = await kanalAltRepo.GetAllAsync(x => x.KanalId == kanalId);

            foreach (var kanalAlt in kanalAltlar)
            {
                // KanalAlt'ın child'larını da sil (KanalAltIslem, KioskMenuIslem)
                await CascadeDeleteKanalAltChildrenAsync(kanalAlt.KanalAltId);

                kanalAlt.SilindiMi = true;
                kanalAlt.DuzenlenmeTarihi = DateTime.Now;
                kanalAltRepo.Update(kanalAlt);
            }

            // KanalIslem kayıtlarını soft delete
            var kanalIslemRepo = _unitOfWork.Repository<KanalIslem>();
            var kanalIslemler = await kanalIslemRepo.GetAllAsync(x => x.KanalId == kanalId);

            foreach (var kanalIslem in kanalIslemler)
            {
                // KanalIslem'in child'larını da sil (KanalAltIslem)
                await CascadeDeleteKanalIslemChildrenAsync(kanalIslem.KanalIslemId);

                kanalIslem.SilindiMi = true;
                kanalIslem.DuzenlenmeTarihi = DateTime.Now;
                kanalIslemRepo.Update(kanalIslem);
            }

            _logger.LogInformation("Cascade delete: KanalId={KanalId}, KanalAlt={Count1}, KanalIslem={Count2}",
                kanalId, kanalAltlar.Count(), kanalIslemler.Count());
        }

        /// <summary>
        /// Cascade update: Kanal Aktiflik değiştiğinde child kayıtları da günceller
        /// </summary>
        private async Task CascadeAktiflikUpdateAsync(int kanalId, Aktiflik yeniAktiflik)
        {
            // KanalAlt kayıtlarını güncelle
            var kanalAltRepo = _unitOfWork.Repository<KanalAlt>();
            var kanalAltlar = await kanalAltRepo.GetAllAsync(x => x.KanalId == kanalId);

            foreach (var kanalAlt in kanalAltlar)
            {
                // KanalAlt'ın child'larını da güncelle (KanalAltIslem, KioskMenuIslem)
                await CascadeAktiflikUpdateKanalAltChildrenAsync(kanalAlt.KanalAltId, yeniAktiflik);

                kanalAlt.Aktiflik = yeniAktiflik;
                kanalAlt.DuzenlenmeTarihi = DateTime.Now;
                kanalAltRepo.Update(kanalAlt);
            }

            // KanalIslem kayıtlarını güncelle
            var kanalIslemRepo = _unitOfWork.Repository<KanalIslem>();
            var kanalIslemler = await kanalIslemRepo.GetAllAsync(x => x.KanalId == kanalId);

            foreach (var kanalIslem in kanalIslemler)
            {
                // KanalIslem'in child'larını da güncelle (KanalAltIslem)
                await CascadeAktiflikUpdateKanalIslemChildrenAsync(kanalIslem.KanalIslemId, yeniAktiflik);

                kanalIslem.Aktiflik = yeniAktiflik;
                kanalIslem.DuzenlenmeTarihi = DateTime.Now;
                kanalIslemRepo.Update(kanalIslem);
            }

            _logger.LogInformation("Cascade aktiflik update: KanalId={KanalId}, YeniAktiflik={Aktiflik}, KanalAlt={Count1}, KanalIslem={Count2}",
                kanalId, yeniAktiflik, kanalAltlar.Count(), kanalIslemler.Count());
        }

        private async Task CascadeDeleteKanalAltChildrenAsync(int kanalAltId)
        {
            // KanalAltIslem kayıtlarını soft delete
            var kanalAltIslemRepo = _unitOfWork.Repository<KanalAltIslem>();
            var kanalAltIslemler = await kanalAltIslemRepo.GetAllAsync(x => x.KanalAltId == kanalAltId);

            foreach (var islem in kanalAltIslemler)
            {
                islem.SilindiMi = true;
                islem.DuzenlenmeTarihi = DateTime.Now;
                kanalAltIslemRepo.Update(islem);
            }

            // KioskMenuIslem kayıtlarını soft delete
            var kioskMenuIslemRepo = _unitOfWork.Repository<KioskMenuIslem>();
            var kioskMenuIslemler = await kioskMenuIslemRepo.GetAllAsync(x => x.KanalAltId == kanalAltId);

            foreach (var islem in kioskMenuIslemler)
            {
                islem.SilindiMi = true;
                islem.DuzenlenmeTarihi = DateTime.Now;
                kioskMenuIslemRepo.Update(islem);
            }
        }

        private async Task CascadeDeleteKanalIslemChildrenAsync(int kanalIslemId)
        {
            // KanalAltIslem kayıtlarını soft delete
            var kanalAltIslemRepo = _unitOfWork.Repository<KanalAltIslem>();
            var kanalAltIslemler = await kanalAltIslemRepo.GetAllAsync(x => x.KanalIslemId == kanalIslemId);

            foreach (var islem in kanalAltIslemler)
            {
                islem.SilindiMi = true;
                islem.DuzenlenmeTarihi = DateTime.Now;
                kanalAltIslemRepo.Update(islem);
            }
        }

        private async Task CascadeAktiflikUpdateKanalAltChildrenAsync(int kanalAltId, Aktiflik yeniAktiflik)
        {
            // KanalAltIslem kayıtlarını güncelle
            var kanalAltIslemRepo = _unitOfWork.Repository<KanalAltIslem>();
            var kanalAltIslemler = await kanalAltIslemRepo.GetAllAsync(x => x.KanalAltId == kanalAltId);

            foreach (var islem in kanalAltIslemler)
            {
                islem.Aktiflik = yeniAktiflik;
                islem.DuzenlenmeTarihi = DateTime.Now;
                kanalAltIslemRepo.Update(islem);
            }

            // KioskMenuIslem kayıtlarını güncelle
            var kioskMenuIslemRepo = _unitOfWork.Repository<KioskMenuIslem>();
            var kioskMenuIslemler = await kioskMenuIslemRepo.GetAllAsync(x => x.KanalAltId == kanalAltId);

            foreach (var islem in kioskMenuIslemler)
            {
                islem.Aktiflik = yeniAktiflik;
                islem.DuzenlenmeTarihi = DateTime.Now;
                kioskMenuIslemRepo.Update(islem);
            }
        }

        private async Task CascadeAktiflikUpdateKanalIslemChildrenAsync(int kanalIslemId, Aktiflik yeniAktiflik)
        {
            // KanalAltIslem kayıtlarını güncelle
            var kanalAltIslemRepo = _unitOfWork.Repository<KanalAltIslem>();
            var kanalAltIslemler = await kanalAltIslemRepo.GetAllAsync(x => x.KanalIslemId == kanalIslemId);

            foreach (var islem in kanalAltIslemler)
            {
                islem.Aktiflik = yeniAktiflik;
                islem.DuzenlenmeTarihi = DateTime.Now;
                kanalAltIslemRepo.Update(islem);
            }
        }
    }
}
