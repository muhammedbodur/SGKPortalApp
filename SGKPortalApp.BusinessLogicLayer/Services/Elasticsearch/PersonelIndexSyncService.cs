using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Elasticsearch;
using SGKPortalApp.BusinessObjectLayer.DTOs.Elasticsearch;
using SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri;
using SGKPortalApp.Common.Helpers;
using SGKPortalApp.DataAccessLayer.Context;

namespace SGKPortalApp.BusinessLogicLayer.Services.Elasticsearch
{
    /// <summary>
    /// SQL -> Elasticsearch senkronizasyon servisi implementasyonu
    /// </summary>
    public class PersonelIndexSyncService : IPersonelIndexSyncService
    {
        private readonly SGKDbContext _dbContext;
        private readonly IPersonelSearchService _searchService;
        private readonly ILogger<PersonelIndexSyncService> _logger;
        private const int BatchSize = 500;

        public PersonelIndexSyncService(
            SGKDbContext dbContext,
            IPersonelSearchService searchService,
            ILogger<PersonelIndexSyncService> logger)
        {
            _dbContext = dbContext;
            _searchService = searchService;
            _logger = logger;
        }

        public async Task<int> FullReindexAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Full reindex başlatılıyor...");
            var totalIndexed = 0;

            try
            {
                // Index'i yeniden oluştur
                var indexCreated = await _searchService.RecreateIndexAsync();
                if (!indexCreated)
                {
                    _logger.LogError("Index oluşturulamadı, reindex iptal edildi");
                    return 0;
                }

                // Toplam personel sayısını al
                var totalCount = await _dbContext.Personeller
                    .Where(p => !p.SilindiMi)
                    .CountAsync(cancellationToken);

                _logger.LogInformation("Toplam {Count} personel indexlenecek", totalCount);

                // Batch halinde işle
                var skip = 0;
                while (skip < totalCount)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        _logger.LogWarning("Reindex iptal edildi");
                        break;
                    }

                    var batch = await GetPersonelBatchAsync(skip, BatchSize, cancellationToken);
                    var elasticDtos = batch.Select(MapToElasticDto).ToList();

                    var indexed = await _searchService.BulkIndexAsync(elasticDtos);
                    totalIndexed += indexed;

                    skip += BatchSize;
                    _logger.LogInformation("İlerleme: {Current}/{Total} ({Percent}%)",
                        Math.Min(skip, totalCount), totalCount, (skip * 100 / totalCount));
                }

                _logger.LogInformation("Full reindex tamamlandı: {Count} personel indexlendi", totalIndexed);
                return totalIndexed;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Full reindex hatası");
                return totalIndexed;
            }
        }

        public async Task<int> IncrementalSyncAsync(DateTime sinceDate, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Incremental sync başlatılıyor: {SinceDate}", sinceDate);
            var totalIndexed = 0;

            try
            {
                // Index yoksa oluştur
                if (!await _searchService.IndexExistsAsync())
                {
                    await _searchService.CreateIndexAsync();
                }

                var updatedPersoneller = await _dbContext.Personeller
                    .Include(p => p.Departman)
                    .Include(p => p.Servis)
                    .Include(p => p.Unvan)
                    .Where(p => !p.SilindiMi && p.DuzenlenmeTarihi >= sinceDate)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                if (!updatedPersoneller.Any())
                {
                    _logger.LogInformation("Güncellenecek personel bulunamadı");
                    return 0;
                }

                var elasticDtos = updatedPersoneller.Select(MapToElasticDto).ToList();
                totalIndexed = await _searchService.BulkIndexAsync(elasticDtos);

                _logger.LogInformation("Incremental sync tamamlandı: {Count} personel güncellendi", totalIndexed);
                return totalIndexed;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Incremental sync hatası");
                return totalIndexed;
            }
        }

        public async Task<bool> SyncPersonelAsync(string tcKimlikNo)
        {
            try
            {
                var personel = await _dbContext.Personeller
                    .Include(p => p.Departman)
                    .Include(p => p.Servis)
                    .Include(p => p.Unvan)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.TcKimlikNo == tcKimlikNo && !p.SilindiMi);

                if (personel == null)
                {
                    // Personel silinmiş olabilir, index'ten kaldır
                    return await _searchService.DeletePersonelAsync(tcKimlikNo);
                }

                var elasticDto = MapToElasticDto(personel);
                return await _searchService.IndexPersonelAsync(elasticDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel sync hatası: {TcKimlikNo}", tcKimlikNo);
                return false;
            }
        }

        public async Task<bool> RemovePersonelAsync(string tcKimlikNo)
        {
            try
            {
                return await _searchService.DeletePersonelAsync(tcKimlikNo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel silme hatası: {TcKimlikNo}", tcKimlikNo);
                return false;
            }
        }

        public async Task<IndexStatusInfo> GetIndexStatusAsync()
        {
            var status = new IndexStatusInfo();

            try
            {
                status.IsAvailable = await _searchService.PingAsync();
                status.IndexExists = await _searchService.IndexExistsAsync();
                status.DocumentCount = await _searchService.GetDocumentCountAsync();
                status.SqlRecordCount = await _dbContext.Personeller
                    .Where(p => !p.SilindiMi)
                    .CountAsync();
            }
            catch (Exception ex)
            {
                status.ErrorMessage = ex.Message;
                _logger.LogError(ex, "Index status hatası");
            }

            return status;
        }

        private async Task<List<BusinessObjectLayer.Entities.PersonelIslemleri.Personel>> GetPersonelBatchAsync(
            int skip, int take, CancellationToken cancellationToken)
        {
            return await _dbContext.Personeller
                .Include(p => p.Departman)
                .Include(p => p.Servis)
                .Include(p => p.Unvan)
                .Where(p => !p.SilindiMi)
                .OrderBy(p => p.TcKimlikNo)
                .Skip(skip)
                .Take(take)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        private static PersonelElasticDto MapToElasticDto(BusinessObjectLayer.Entities.PersonelIslemleri.Personel p)
        {
            var sicilNoStr = p.SicilNo?.ToString();
            var fullTextParts = new List<string>
            {
                p.AdSoyad,
                sicilNoStr ?? "",
                p.Departman?.DepartmanAdi ?? "",
                p.Servis?.ServisAdi ?? "",
                p.Unvan?.UnvanAdi ?? ""
            };

            return new PersonelElasticDto
            {
                TcKimlikNo = p.TcKimlikNo,
                AdSoyad = p.AdSoyad,
                SicilNo = sicilNoStr,
                DepartmanId = p.DepartmanId,
                DepartmanAdi = p.Departman?.DepartmanAdi,
                ServisId = p.ServisId,
                ServisAdi = p.Servis?.ServisAdi,
                UnvanId = p.UnvanId,
                UnvanAdi = p.Unvan?.UnvanAdi,
                Resim = p.Resim,
                PersonelAktiflikDurum = p.PersonelAktiflikDurum,
                Aktif = p.PersonelAktiflikDurum.IsKurumPersoneli(),
                FullText = string.Join(" ", fullTextParts.Where(x => !string.IsNullOrWhiteSpace(x))),
                GuncellemeTarihi = p.DuzenlenmeTarihi
            };
        }
    }
}
