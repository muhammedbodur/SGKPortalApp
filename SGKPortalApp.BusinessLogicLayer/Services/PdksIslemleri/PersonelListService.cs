using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.BusinessLogicLayer.Services.PdksIslemleri
{
    public class PersonelListService : IPersonelListService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PersonelListService> _logger;

        public PersonelListService(
            IUnitOfWork unitOfWork,
            ILogger<PersonelListService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponseDto<List<PersonelListResponseDto>>> GetPersonelListAsync(PersonelListFilterRequestDto request, string currentUserTcKimlikNo)
        {
            try
            {
                // 1. Tüm personelleri çek (detaylarla birlikte)
                var personelRepo = _unitOfWork.GetRepository<IPersonelRepository>();
                var personeller = await personelRepo.GetAllWithDetailsAsync();

                // 2. Filtreleme uygula
                var query = personeller.AsQueryable();

                // Aktif durum filtresi
                if (request.SadeceAktifler.HasValue && request.SadeceAktifler.Value)
                {
                    query = query.Where(p => p.PersonelAktiflikDurum == PersonelAktiflikDurum.Aktif);
                }


                // Departman filtresi
                if (request.DepartmanId.HasValue)
                {
                    query = query.Where(p => p.DepartmanId == request.DepartmanId.Value);
                }

                // Servis filtresi
                if (request.ServisId.HasValue)
                {
                    query = query.Where(p => p.ServisId == request.ServisId.Value);
                }

                // Arama metni filtresi (Ad Soyad veya Sicil No)
                if (!string.IsNullOrWhiteSpace(request.AramaMetni))
                {
                    var aramaMetni = request.AramaMetni.ToLower();
                    query = query.Where(p =>
                        p.AdSoyad.ToLower().Contains(aramaMetni) ||
                        p.SicilNo.ToString().Contains(aramaMetni));
                }

                // 3. DTO'ya dönüştür
                var result = query.Select(p => new PersonelListResponseDto
                {
                    TcKimlikNo = p.TcKimlikNo,
                    AdSoyad = p.AdSoyad,
                    SicilNo = p.SicilNo,
                    PersonelKayitNo = p.PersonelKayitNo,
                    DepartmanAdi = p.Departman != null ? p.Departman.DepartmanAdi : "",
                    ServisAdi = p.Servis != null ? p.Servis.ServisAdi : null,
                    Aktif = p.PersonelAktiflikDurum == PersonelAktiflikDurum.Aktif,
                    Email = p.Email,
                    CepTelefonu = p.CepTelefonu
                })
                .OrderBy(p => p.AdSoyad)
                .ToList();

                _logger.LogInformation("Personel listesi alındı: {Count} kayıt", result.Count);

                return ApiResponseDto<List<PersonelListResponseDto>>.SuccessResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel listesi alınırken hata oluştu");
                return ApiResponseDto<List<PersonelListResponseDto>>.ErrorResult($"Hata: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<bool>> UpdatePersonelAktifDurumAsync(PersonelAktifDurumUpdateDto request, string currentUserTcKimlikNo)
        {
            try
            {
                // 1. Personeli bul
                var personelRepo = _unitOfWork.GetRepository<IPersonelRepository>();
                var personel = await personelRepo.GetByTcKimlikNoAsync(request.TcKimlikNo);

                if (personel == null)
                {
                    return ApiResponseDto<bool>.ErrorResult($"Personel bulunamadı: {request.TcKimlikNo}");
                }

                // 2. Aktif durumu güncelle
                personel.PersonelAktiflikDurum = request.Aktif ? PersonelAktiflikDurum.Aktif : PersonelAktiflikDurum.Pasif;
                personel.DuzenlenmeTarihi = DateTime.Now;
                personel.DuzenleyenKullanici = currentUserTcKimlikNo;

                // 3. Repository'ye güncelle
                var genericRepo = _unitOfWork.Repository<Personel>();
                genericRepo.Update(personel);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation(
                    "Personel aktif durum güncellendi: {TcKimlikNo} -> {Aktif}",
                    request.TcKimlikNo,
                    request.Aktif);

                return ApiResponseDto<bool>.SuccessResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel aktif durum güncellenirken hata oluştu: {TcKimlikNo}", request.TcKimlikNo);
                return ApiResponseDto<bool>.ErrorResult($"Hata: {ex.Message}");
            }
        }
    }
}
