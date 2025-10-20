using Microsoft.Extensions.Logging;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace SGKPortalApp.PresentationLayer.Services.UIServices.Concrete
{
    /// <summary>
    /// ⭐ RESIM CACHE BUSTING SERVİSİ
    /// 
    /// SORUN:
    /// Aynı dosya adıyla yüklenen yeni resim, browser cache'ten eski resim gösteriyor.
    /// 
    /// ÇÖZÜM:
    /// Dosya son değiştirilme zamanından ETag oluşturup, URL'ye parametre olarak ekle.
    /// Her resim güncellemede ETag değişir → Browser yeni resmi indirir.
    /// 
    /// NASIL ÇALIŞIR:
    /// 1. Resim yüklendi: /images/avatars/28165202398.jpg?v=A1B2C3D4
    /// 2. Yeni resim aynı adla yüklendi
    /// 3. ImageHelper cache invalidate ediyor
    /// 4. Sayfa yüklendiğinde yeni ETag: /images/avatars/28165202398.jpg?v=X9Y8Z7W6
    /// 5. Browser URL değiştiğini görüp yeni resmi indiriyor ✅
    /// </summary>
    

    public class ImageCacheService : IImageCacheService
    {
        private readonly ILogger<ImageCacheService> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ImageCacheService(
            ILogger<ImageCacheService> logger,
            IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _logger.LogDebug("✅ ImageCacheService başlatıldı");
        }

        /// <summary>
        /// Resim dosyası için ETag (cache-busting parametresi) döndürür.
        /// Her seferinde dosyadan GÜNCEL ETag oluşturur (cache'lenmez).
        /// </summary>
        public string GetCacheBustParameter(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return "none";

            try
            {
                var fullPath = GetFullPath(imagePath);

                // Dosya var mı kontrol et
                if (!File.Exists(fullPath))
                {
                    _logger.LogWarning("⚠️  Resim bulunamadı: {FilePath}", fullPath);
                    return "notfound";
                }

                var fileInfo = new FileInfo(fullPath);

                // ⭐ HER SEFERINDE DOSYADAN GÜNCEL ETag OLUŞTUR
                // Cache'leme yapılmıyor, böylece resim güncellendiğinde hemen yeni ETag alınır
                var etag = GenerateETag(fileInfo);

                _logger.LogDebug("✅ ETag oluşturuldu: {ETag} (Dosya: {Size} bytes, Değiştirilme: {LastWrite})", 
                    etag, fileInfo.Length, fileInfo.LastWriteTimeUtc);
                
                return etag;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ ETag alınamadı: {ImagePath}", imagePath);
                return "error";
            }
        }

        /// <summary>
        /// Cache'i invalidate et (resim silindiğinde veya güncellendiğinde)
        /// NOT: Artık cache kullanılmıyor, bu metod boş bırakıldı (interface uyumluluğu için)
        /// </summary>
        public void InvalidateCache(string imagePath)
        {
            // Cache kullanılmadığı için hiçbir şey yapmaya gerek yok
            // Her GetCacheBustParameter çağrısında dosyadan güncel ETag alınıyor
            _logger.LogDebug("🧹 InvalidateCache çağrıldı (cache yok): {ImagePath}", imagePath);
        }

        /// <summary>
        /// FileInfo'dan ETag oluştur
        /// Format: MD5(FileSize + LastWriteTime) → 8 karakter HEX
        /// Örnek: "A1B2C3D4"
        /// </summary>
        private string GenerateETag(FileInfo fileInfo)
        {
            try
            {
                // Dosya boyutu + son değiştirilme zamanı birleştir
                var combined = $"{fileInfo.Length}_{fileInfo.LastWriteTimeUtc.Ticks}";

                // MD5 hash'i oluştur (hızlı ve yeterli)
                using var md5 = MD5.Create();
                var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(combined));

                // İlk 4 byte'ı HEX'e çevir (8 karakter)
                var etag = BitConverter.ToString(hash, 0, 4).Replace("-", "");
                return etag;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ ETag oluşturulamadı");
                return DateTime.Now.Ticks.ToString("X");
            }
        }

        /// <summary>
        /// Göreli yolu tam dosya yoluna çevirir
        /// /images/avatars/28165202398.jpg → C:\...\wwwroot\images\avatars\28165202398.jpg
        /// </summary>
        private string GetFullPath(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return string.Empty;

            var cleanPath = imagePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            return Path.Combine(_webHostEnvironment.WebRootPath, cleanPath);
        }
    }
}
