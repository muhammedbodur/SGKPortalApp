using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.BusinessLogicLayer.Services.PdksIslemleri
{
    public class IzinSorumluService : IIzinSorumluService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<IzinSorumluService> _logger;

        public IzinSorumluService(
            IUnitOfWork unitOfWork,
            ILogger<IzinSorumluService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponseDto<List<IzinSorumluResponseDto>>> GetAllAsync()
        {
            try
            {
                var repo = _unitOfWork.Repository<IzinSorumlu>();
                var entities = await repo.GetAllAsync(
                    x => x.Departman!,
                    x => x.Servis!,
                    x => x.SorumluPersonel!);

                var dtos = entities.Select(MapToDto).ToList();
                return ApiResponseDto<List<IzinSorumluResponseDto>>.SuccessResult(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İzin sorumlu listesi alınırken hata");
                return ApiResponseDto<List<IzinSorumluResponseDto>>.ErrorResult("Veriler yüklenirken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<List<IzinSorumluResponseDto>>> GetActiveAsync()
        {
            try
            {
                var repo = _unitOfWork.Repository<IzinSorumlu>();
                var entities = await repo.FindAsync(
                    x => x.Aktif,
                    x => x.Departman!,
                    x => x.Servis!,
                    x => x.SorumluPersonel!);

                var dtos = entities.Select(MapToDto).ToList();
                return ApiResponseDto<List<IzinSorumluResponseDto>>.SuccessResult(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Aktif izin sorumlu listesi alınırken hata");
                return ApiResponseDto<List<IzinSorumluResponseDto>>.ErrorResult("Veriler yüklenirken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<IzinSorumluResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var repo = _unitOfWork.Repository<IzinSorumlu>();
                var entity = await repo.FirstOrDefaultAsync(
                    x => x.IzinSorumluId == id,
                    x => x.Departman!,
                    x => x.Servis!,
                    x => x.SorumluPersonel!);

                if (entity == null)
                    return ApiResponseDto<IzinSorumluResponseDto>.ErrorResult("İzin sorumlusu bulunamadı");

                var dto = MapToDto(entity);
                return ApiResponseDto<IzinSorumluResponseDto>.SuccessResult(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İzin sorumlusu alınırken hata: {Id}", id);
                return ApiResponseDto<IzinSorumluResponseDto>.ErrorResult("Veri yüklenirken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<List<IzinSorumluResponseDto>>> GetByDepartmanServisAsync(int? departmanId, int? servisId)
        {
            try
            {
                var repo = _unitOfWork.Repository<IzinSorumlu>();
                var entities = await repo.FindAsync(
                    x => x.Aktif &&
                        (departmanId == null || x.DepartmanId == departmanId) &&
                        (servisId == null || x.ServisId == servisId),
                    x => x.Departman!,
                    x => x.Servis!,
                    x => x.SorumluPersonel!);

                var dtos = entities.Select(MapToDto).ToList();
                return ApiResponseDto<List<IzinSorumluResponseDto>>.SuccessResult(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Departman/Servis bazlı sorumlu listesi alınırken hata");
                return ApiResponseDto<List<IzinSorumluResponseDto>>.ErrorResult("Veriler yüklenirken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<List<IzinSorumluResponseDto>>> GetSorumluForPersonelAsync(string personelTcKimlikNo)
        {
            try
            {
                // Personel bilgilerini al
                var personelRepo = _unitOfWork.Repository<Personel>();
                var personel = await personelRepo.GetByIdAsync(personelTcKimlikNo);

                if (personel == null)
                    return ApiResponseDto<List<IzinSorumluResponseDto>>.ErrorResult("Personel bulunamadı");

                // Personelin departman ve servisine göre sorumluları getir
                var repo = _unitOfWork.Repository<IzinSorumlu>();
                var entities = await repo.FindAsync(
                    x => x.Aktif &&
                        (x.DepartmanId == null || x.DepartmanId == personel.DepartmanId) &&
                        (x.ServisId == null || x.ServisId == personel.ServisId),
                    x => x.Departman!,
                    x => x.Servis!,
                    x => x.SorumluPersonel!);

                var dtos = entities.OrderBy(x => x.OnaySeviyesi).Select(MapToDto).ToList();
                return ApiResponseDto<List<IzinSorumluResponseDto>>.SuccessResult(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel için sorumlu listesi alınırken hata: {TcKimlikNo}", personelTcKimlikNo);
                return ApiResponseDto<List<IzinSorumluResponseDto>>.ErrorResult("Veriler yüklenirken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<IzinSorumluResponseDto>> CreateAsync(IzinSorumluCreateDto request)
        {
            try
            {
                // Sorumlu personelin varlığını kontrol et
                var personelRepo = _unitOfWork.Repository<Personel>();
                var personel = await personelRepo.GetByIdAsync(request.SorumluPersonelTcKimlikNo);

                if (personel == null)
                    return ApiResponseDto<IzinSorumluResponseDto>.ErrorResult("Sorumlu personel bulunamadı");

                // Aynı atama var mı kontrol et
                var repo = _unitOfWork.Repository<IzinSorumlu>();
                var existing = await repo.FindAsync(
                    x => x.Aktif &&
                        x.DepartmanId == request.DepartmanId &&
                        x.ServisId == request.ServisId &&
                        x.OnaySeviyesi == request.OnaySeviyesi);

                if (existing.Any())
                    return ApiResponseDto<IzinSorumluResponseDto>.ErrorResult("Bu departman/servis ve onay seviyesi için zaten sorumlu atanmış");

                // Yeni atama oluştur
                var entity = new IzinSorumlu
                {
                    DepartmanId = request.DepartmanId,
                    ServisId = request.ServisId,
                    SorumluPersonelTcKimlikNo = request.SorumluPersonelTcKimlikNo,
                    OnaySeviyesi = request.OnaySeviyesi,
                    Aktif = true,
                    Aciklama = request.Aciklama,
                    EklenmeTarihi = DateTime.Now
                };

                await repo.AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();

                // Detaylı entity'yi çek
                var created = await repo.FirstOrDefaultAsync(
                    x => x.IzinSorumluId == entity.IzinSorumluId,
                    x => x.Departman!,
                    x => x.Servis!,
                    x => x.SorumluPersonel!);

                var dto = MapToDto(created!);
                return ApiResponseDto<IzinSorumluResponseDto>.SuccessResult(dto, "İzin sorumlusu başarıyla atandı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İzin sorumlusu oluşturulurken hata");
                return ApiResponseDto<IzinSorumluResponseDto>.ErrorResult("Kayıt oluşturulurken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<IzinSorumluResponseDto>> UpdateAsync(IzinSorumluUpdateDto request)
        {
            try
            {
                var repo = _unitOfWork.Repository<IzinSorumlu>();
                var entity = await repo.GetByIdAsync(request.IzinSorumluId);

                if (entity == null)
                    return ApiResponseDto<IzinSorumluResponseDto>.ErrorResult("İzin sorumlusu bulunamadı");

                // Sorumlu personelin varlığını kontrol et
                var personelRepo = _unitOfWork.Repository<Personel>();
                var personel = await personelRepo.GetByIdAsync(request.SorumluPersonelTcKimlikNo);

                if (personel == null)
                    return ApiResponseDto<IzinSorumluResponseDto>.ErrorResult("Sorumlu personel bulunamadı");

                // Güncelle
                entity.DepartmanId = request.DepartmanId;
                entity.ServisId = request.ServisId;
                entity.SorumluPersonelTcKimlikNo = request.SorumluPersonelTcKimlikNo;
                entity.OnaySeviyesi = request.OnaySeviyesi;
                entity.Aktif = request.Aktif;
                entity.Aciklama = request.Aciklama;
                entity.DuzenlenmeTarihi = DateTime.Now;

                repo.Update(entity);
                await _unitOfWork.SaveChangesAsync();

                // Detaylı entity'yi çek
                var updated = await repo.FirstOrDefaultAsync(
                    x => x.IzinSorumluId == entity.IzinSorumluId,
                    x => x.Departman!,
                    x => x.Servis!,
                    x => x.SorumluPersonel!);

                var dto = MapToDto(updated!);
                return ApiResponseDto<IzinSorumluResponseDto>.SuccessResult(dto, "İzin sorumlusu başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İzin sorumlusu güncellenirken hata: {Id}", request.IzinSorumluId);
                return ApiResponseDto<IzinSorumluResponseDto>.ErrorResult("Güncelleme sırasında hata oluştu");
            }
        }

        public async Task<ApiResponseDto<bool>> DeactivateAsync(int id)
        {
            try
            {
                var repo = _unitOfWork.Repository<IzinSorumlu>();
                var entity = await repo.GetByIdAsync(id);

                if (entity == null)
                    return ApiResponseDto<bool>.ErrorResult("İzin sorumlusu bulunamadı");

                entity.Aktif = false;
                entity.DuzenlenmeTarihi = DateTime.Now;

                repo.Update(entity);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponseDto<bool>.SuccessResult(true, "İzin sorumlusu pasif yapıldı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İzin sorumlusu pasif yapılırken hata: {Id}", id);
                return ApiResponseDto<bool>.ErrorResult("İşlem sırasında hata oluştu");
            }
        }

        public async Task<ApiResponseDto<bool>> ActivateAsync(int id)
        {
            try
            {
                var repo = _unitOfWork.Repository<IzinSorumlu>();
                var entity = await repo.GetByIdAsync(id);

                if (entity == null)
                    return ApiResponseDto<bool>.ErrorResult("İzin sorumlusu bulunamadı");

                entity.Aktif = true;
                entity.DuzenlenmeTarihi = DateTime.Now;

                repo.Update(entity);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponseDto<bool>.SuccessResult(true, "İzin sorumlusu aktif yapıldı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İzin sorumlusu aktif yapılırken hata: {Id}", id);
                return ApiResponseDto<bool>.ErrorResult("İşlem sırasında hata oluştu");
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteAsync(int id)
        {
            try
            {
                var repo = _unitOfWork.Repository<IzinSorumlu>();
                var entity = await repo.GetByIdAsync(id);

                if (entity == null)
                    return ApiResponseDto<bool>.ErrorResult("İzin sorumlusu bulunamadı");

                repo.Delete(entity);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponseDto<bool>.SuccessResult(true, "İzin sorumlusu silindi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İzin sorumlusu silinirken hata: {Id}", id);
                return ApiResponseDto<bool>.ErrorResult("Silme işlemi sırasında hata oluştu");
            }
        }

        private IzinSorumluResponseDto MapToDto(IzinSorumlu entity)
        {
            return new IzinSorumluResponseDto
            {
                IzinSorumluId = entity.IzinSorumluId,
                DepartmanId = entity.DepartmanId,
                DepartmanAdi = entity.Departman?.DepartmanAdi ?? "Tüm Departmanlar",
                ServisId = entity.ServisId,
                ServisAdi = entity.Servis?.ServisAdi ?? (entity.DepartmanId.HasValue ? "Tüm Servisler" : null),
                SorumluPersonelTcKimlikNo = entity.SorumluPersonelTcKimlikNo,
                SorumluPersonelAdSoyad = entity.SorumluPersonel?.AdSoyad ?? "",
                SorumluPersonelSicilNo = entity.SorumluPersonel?.SicilNo ?? 0,
                OnaySeviyesi = entity.OnaySeviyesi,
                Aktif = entity.Aktif,
                Aciklama = entity.Aciklama,
                EklenmeTarihi = entity.EklenmeTarihi,
                DuzenlenmeTarihi = entity.DuzenlenmeTarihi
            };
        }
    }
}
