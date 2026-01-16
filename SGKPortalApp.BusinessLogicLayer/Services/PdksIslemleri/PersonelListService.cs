using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.DataAccessLayer.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.BusinessLogicLayer.Services.PdksIslemleri
{
    public class PersonelListService : IPersonelListService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PersonelListService> _logger;

        // Upper management titles that have full access
        private readonly string[] _upperManagementTitles = new[]
        {
            "SUPER USER",
            "SG İL MÜDÜRÜ",
            "İL MÜDÜRÜ",
            "İL MÜDÜR YARDIMCISI",
            "İL MÜDÜR BAŞYARDIMCISI",
            "SG İL MÜDÜR YARDIMCISI"
        };

        // Special sicil numbers that have full access
        private readonly int[] _specialSicilNumbers = new[] { 418434, 412613, 208032 };

        public PersonelListService(
            ApplicationDbContext context,
            ILogger<PersonelListService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ApiResponseDto<List<PersonelListResponseDto>>> GetPersonelListAsync(
            PersonelListFilterRequestDto request,
            string currentUserTcKimlikNo)
        {
            try
            {
                // Get current user with department and title info
                var currentUser = await _context.Personeller
                    .Include(p => p.Departman)
                        .ThenInclude(d => d!.Sgm)
                    .Include(p => p.Servis)
                        .ThenInclude(s => s!.Sgm)
                    .Include(p => p.Unvan)
                    .FirstOrDefaultAsync(p => p.TcKimlikNo == currentUserTcKimlikNo);

                if (currentUser == null)
                    return ApiResponseDto<List<PersonelListResponseDto>>.ErrorResult("Kullanıcı bulunamadı");

                // Check authorization level
                bool hasFullAccess = await CheckFullAccessAsync(currentUserTcKimlikNo, currentUser);

                // Build query
                var query = _context.Personeller
                    .Include(p => p.Departman)
                        .ThenInclude(d => d!.Sgm)
                    .Include(p => p.Servis)
                        .ThenInclude(s => s!.Sgm)
                    .AsQueryable();

                // Apply authorization filter
                if (!hasFullAccess)
                {
                    // Normal users can only see their department
                    query = query.Where(p => p.DepartmanId == currentUser.DepartmanId);
                }

                // Apply filters
                if (request.SgmId.HasValue)
                {
                    query = query.Where(p =>
                        p.Departman!.SgmId == request.SgmId.Value ||
                        p.Servis!.SgmId == request.SgmId.Value);
                }

                if (request.DepartmanId.HasValue)
                {
                    query = query.Where(p => p.DepartmanId == request.DepartmanId.Value);
                }

                if (request.ServisId.HasValue)
                {
                    query = query.Where(p => p.ServisId == request.ServisId.Value);
                }

                if (request.SadeceAktifler == true)
                {
                    query = query.Where(p => p.PersonelAktiflikDurum == PersonelAktiflikDurum.Aktif);
                }

                if (!string.IsNullOrWhiteSpace(request.AramaMetni))
                {
                    var searchText = request.AramaMetni.ToLower();
                    query = query.Where(p =>
                        p.AdSoyad.ToLower().Contains(searchText) ||
                        p.SicilNo.ToString().Contains(searchText));
                }

                // Execute query and map to DTO
                var personeller = await query
                    .OrderBy(p => p.AdSoyad)
                    .Select(p => new PersonelListResponseDto
                    {
                        TcKimlikNo = p.TcKimlikNo,
                        AdSoyad = p.AdSoyad,
                        SicilNo = p.SicilNo,
                        PersonelKayitNo = p.PersonelKayitNo,
                        DepartmanAdi = p.Departman!.DepartmanAdi,
                        ServisAdi = p.Servis!.ServisAdi,
                        SgmAdi = p.Departman!.Sgm!.SgmAdi ?? p.Servis!.Sgm!.SgmAdi,
                        Aktif = p.PersonelAktiflikDurum == PersonelAktiflikDurum.Aktif,
                        Email = p.Email,
                        CepTelefonu = p.CepTelefonu
                    })
                    .ToListAsync();

                return ApiResponseDto<List<PersonelListResponseDto>>.SuccessResult(personeller);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel listesi alınırken hata: {TcKimlikNo}", currentUserTcKimlikNo);
                return ApiResponseDto<List<PersonelListResponseDto>>.ErrorResult($"Bir hata oluştu: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<bool>> UpdatePersonelAktifDurumAsync(
            PersonelAktifDurumUpdateDto request,
            string currentUserTcKimlikNo)
        {
            try
            {
                // Get current user
                var currentUser = await _context.Personeller
                    .Include(p => p.Unvan)
                    .FirstOrDefaultAsync(p => p.TcKimlikNo == currentUserTcKimlikNo);

                if (currentUser == null)
                    return ApiResponseDto<bool>.ErrorResult("Kullanıcı bulunamadı");

                // Check authorization
                bool hasFullAccess = await CheckFullAccessAsync(currentUserTcKimlikNo, currentUser);
                if (!hasFullAccess)
                    return ApiResponseDto<bool>.ErrorResult("Bu işlem için yetkiniz yok");

                // Get target personel
                var personel = await _context.Personeller
                    .FirstOrDefaultAsync(p => p.TcKimlikNo == request.TcKimlikNo);

                if (personel == null)
                    return ApiResponseDto<bool>.ErrorResult("Personel bulunamadı");

                // Update aktif durum
                personel.PersonelAktiflikDurum = request.Aktif
                    ? PersonelAktiflikDurum.Aktif
                    : PersonelAktiflikDurum.Pasif;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Personel aktiflik durumu güncellendi: {TcKimlikNo} -> {Durum}",
                    request.TcKimlikNo, personel.PersonelAktiflikDurum);

                return ApiResponseDto<bool>.SuccessResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel aktiflik durumu güncellenirken hata: {TcKimlikNo}", request.TcKimlikNo);
                return ApiResponseDto<bool>.ErrorResult($"Bir hata oluştu: {ex.Message}");
            }
        }

        private async Task<bool> CheckFullAccessAsync(string tcKimlikNo,
            SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri.Personel personel)
        {
            // Layer 1: Check if user is social facilities authority (st)
            var hasStAuthority = await _context.EpostaYetkilisi
                .AnyAsync(e => e.SicilNo == personel.SicilNo &&
                              e.Tip == "st" &&
                              e.Aktif);

            if (hasStAuthority)
                return true;

            // Layer 2: Check if user has upper management title
            if (personel.Unvan != null &&
                _upperManagementTitles.Contains(personel.Unvan.UnvanAdi.ToUpper()))
                return true;

            // Layer 3: Check if user has special sicil number
            if (_specialSicilNumbers.Contains(personel.SicilNo))
                return true;

            // Layer 4: Normal user - no full access
            return false;
        }
    }
}
