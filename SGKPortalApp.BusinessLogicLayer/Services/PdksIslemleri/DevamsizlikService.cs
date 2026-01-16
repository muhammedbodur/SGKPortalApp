using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Services.Base;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.PdksIslemleri;
using SGKPortalApp.Common.Results;
using SGKPortalApp.DataAccessLayer.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.BusinessLogicLayer.Services.PdksIslemleri
{
    public class DevamsizlikService : BaseService, IDevamsizlikService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DevamsizlikService> _logger;

        public DevamsizlikService(
            ApplicationDbContext context,
            ILogger<DevamsizlikService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IResult<List<DevamsizlikListDto>>> GetDevamsizlikListAsync(DevamsizlikFilterDto filter)
        {
            try
            {
                var query = _context.IzinMazeretTalepleri
                    .Include(i => i.Personel)
                    .AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(filter.TcKimlikNo))
                {
                    query = query.Where(i => i.TcKimlikNo == filter.TcKimlikNo);
                }

                if (filter.BaslangicTarihi.HasValue)
                {
                    query = query.Where(i =>
                        (i.BaslangicTarihi.HasValue && i.BaslangicTarihi >= filter.BaslangicTarihi) ||
                        (i.MazeretTarihi.HasValue && i.MazeretTarihi >= filter.BaslangicTarihi));
                }

                if (filter.BitisTarihi.HasValue)
                {
                    query = query.Where(i =>
                        (i.BitisTarihi.HasValue && i.BitisTarihi <= filter.BitisTarihi) ||
                        (i.MazeretTarihi.HasValue && i.MazeretTarihi <= filter.BitisTarihi));
                }

                if (filter.SadeceOnayBekleyenler == true)
                {
                    query = query.Where(i => !i.OnayDurumu);
                }

                var list = await query
                    .OrderByDescending(i => i.CreatedAt)
                    .Select(i => new DevamsizlikListDto
                    {
                        Id = i.IzinMazeretTalepId,
                        TcKimlikNo = i.TcKimlikNo,
                        AdSoyad = i.Personel != null ? i.Personel.AdSoyad : "",
                        SicilNo = i.Personel != null ? i.Personel.SicilNo : 0,
                        Turu = i.Turu,
                        TuruAdi = i.Turu.ToString(),
                        BaslangicTarihi = i.BaslangicTarihi,
                        BitisTarihi = i.BitisTarihi,
                        MazeretTarihi = i.MazeretTarihi,
                        SaatDilimi = i.SaatDilimi,
                        SaatDilimiAdi = i.SaatDilimi.HasValue ? i.SaatDilimi.Value.ToString() : null,
                        Aciklama = i.Aciklama,
                        OnayDurumu = i.OnayDurumu,
                        OnaylayanSicilNo = i.OnaylayanSicilNo,
                        OnaylayanAdSoyad = null, // TODO: Join with Personel for approver name
                        OnayTarihi = i.OnayTarihi,
                        OlusturmaTarihi = i.CreatedAt
                    })
                    .ToListAsync();

                return Result<List<DevamsizlikListDto>>.Success(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Devamsızlık listesi alınırken hata");
                return Result<List<DevamsizlikListDto>>.Failure($"Bir hata oluştu: {ex.Message}");
            }
        }

        public async Task<IResult<int>> CreateDevamsizlikAsync(DevamsizlikCreateDto request)
        {
            try
            {
                // Validate personel exists
                var personelExists = await _context.Personeller
                    .AnyAsync(p => p.TcKimlikNo == request.TcKimlikNo);

                if (!personelExists)
                    return Result<int>.Failure("Personel bulunamadı");

                // Validate date logic based on type
                if (request.Turu == BusinessObjectLayer.Enums.PdksIslemleri.IzinMazeretTuru.Mazeret)
                {
                    if (!request.MazeretTarihi.HasValue)
                        return Result<int>.Failure("Mazeret için tarih girilmelidir");
                }
                else
                {
                    if (!request.BaslangicTarihi.HasValue || !request.BitisTarihi.HasValue)
                        return Result<int>.Failure("İzin için başlangıç ve bitiş tarihleri girilmelidir");

                    if (request.BaslangicTarihi > request.BitisTarihi)
                        return Result<int>.Failure("Başlangıç tarihi bitiş tarihinden büyük olamaz");
                }

                var entity = new IzinMazeretTalep
                {
                    TcKimlikNo = request.TcKimlikNo,
                    Turu = request.Turu,
                    BaslangicTarihi = request.BaslangicTarihi,
                    BitisTarihi = request.BitisTarihi,
                    MazeretTarihi = request.MazeretTarihi,
                    SaatDilimi = request.SaatDilimi,
                    Aciklama = request.Aciklama,
                    OnayDurumu = false,
                    OnaylayanSicilNo = request.OnaylayanSicilNo
                };

                _context.IzinMazeretTalepleri.Add(entity);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Devamsızlık kaydı oluşturuldu: {TcKimlikNo} - {Turu}",
                    request.TcKimlikNo, request.Turu);

                return Result<int>.Success(entity.IzinMazeretTalepId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Devamsızlık oluştururken hata: {TcKimlikNo}", request.TcKimlikNo);
                return Result<int>.Failure($"Bir hata oluştu: {ex.Message}");
            }
        }

        public async Task<IResult<bool>> OnaylaDevamsizlikAsync(int id, int onaylayanSicilNo)
        {
            try
            {
                var entity = await _context.IzinMazeretTalepleri
                    .FirstOrDefaultAsync(i => i.IzinMazeretTalepId == id);

                if (entity == null)
                    return Result<bool>.Failure("Kayıt bulunamadı");

                entity.OnayDurumu = true;
                entity.OnaylayanSicilNo = onaylayanSicilNo;
                entity.OnayTarihi = DateTime.Now;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Devamsızlık onaylandı: {Id} - Onaylayan: {SicilNo}",
                    id, onaylayanSicilNo);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Devamsızlık onaylanırken hata: {Id}", id);
                return Result<bool>.Failure($"Bir hata oluştu: {ex.Message}");
            }
        }

        public async Task<IResult<bool>> DeleteDevamsizlikAsync(int id)
        {
            try
            {
                var entity = await _context.IzinMazeretTalepleri
                    .FirstOrDefaultAsync(i => i.IzinMazeretTalepId == id);

                if (entity == null)
                    return Result<bool>.Failure("Kayıt bulunamadı");

                _context.IzinMazeretTalepleri.Remove(entity);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Devamsızlık silindi: {Id}", id);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Devamsızlık silinirken hata: {Id}", id);
                return Result<bool>.Failure($"Bir hata oluştu: {ex.Message}");
            }
        }
    }
}
