using Microsoft.Extensions.Logging;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;
using SkiaSharp;

namespace SGKPortalApp.PresentationLayer.Helpers
{
    /// <summary>
    /// ⭐ GÜNCELLENMIŞ ImageHelper
    /// 
    /// Değişiklikler:
    /// ✅ ImageCacheService dependency eklendi
    /// ✅ SaveImageAsync() içinde cache invalidate yapılıyor
    /// ✅ DeleteImage() içinde cache invalidate yapılıyor
    /// 
    /// Kullanım:
    /// var imagePath = await _imageHelper.SaveImageAsync(optimizedImage, fileName);
    /// // Otomatik olarak eski resim silinir ve cache invalidate yapılır
    /// </summary>
    public class ImageHelper
    {
        private readonly ILogger<ImageHelper> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IImageCacheService _imageCacheService;

        public ImageHelper(
            ILogger<ImageHelper> logger,
            IWebHostEnvironment webHostEnvironment,
            IImageCacheService imageCacheService)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _imageCacheService = imageCacheService;
            _logger.LogInformation("✅ ImageHelper (Cache Service entegre) başlatıldı");
        }

        /// <summary>
        /// Stream'den resim okur, validate eder, resize ve optimize eder
        /// SkiaSharp ile JPEG formatında çıkış verir
        /// </summary>
        public Task<byte[]?> LoadResizeAndOptimizeAsync(
            Stream imageStream,
            int maxWidth = 400,
            int maxHeight = 400,
            int quality = 85)
        {
            if (imageStream == null || imageStream.Length == 0)
            {
                _logger.LogWarning("⚠️  Stream boş veya null");
                return Task.FromResult<byte[]?>(null);
            }

            return Task.Run(() =>
            {
                try
                {
                    imageStream.Position = 0;

                    using var originalBitmap = SKBitmap.Decode(imageStream);

                    if (originalBitmap == null)
                    {
                        _logger.LogWarning("❌ Geçersiz resim formatı");
                        return null;
                    }

                    _logger.LogInformation("✅ Resim doğrulandı: {Width}x{Height}",
                        originalBitmap.Width, originalBitmap.Height);

                    // Aspect ratio'yu koru
                    var ratioX = (double)maxWidth / originalBitmap.Width;
                    var ratioY = (double)maxHeight / originalBitmap.Height;
                    var ratio = Math.Min(ratioX, ratioY);
                    if (ratio > 1) ratio = 1;

                    var newWidth = (int)(originalBitmap.Width * ratio);
                    var newHeight = (int)(originalBitmap.Height * ratio);

                    var resizeInfo = new SKImageInfo(newWidth, newHeight);
                    using var resizedBitmap = originalBitmap.Resize(resizeInfo,
                        new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear));

                    _logger.LogInformation("📐 Yeniden boyutlandırıldı: {Width}x{Height}", newWidth, newHeight);

                    using var image = SKImage.FromBitmap(resizedBitmap);
                    using var data = image.Encode(SKEncodedImageFormat.Jpeg, quality);

                    using var ms = new MemoryStream();
                    data.SaveTo(ms);

                    var resultBytes = ms.ToArray();
                    _logger.LogInformation("💾 Optimize edildi: {Size}KB", resultBytes.Length / 1024);

                    return resultBytes;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Resim işlemede hata");
                    return null;
                }
            });
        }

        /// <summary>
        /// ⭐ ÖNEMLİ: Optimize edilmiş resim byte array'ini dosya olarak kaydeder
        /// - Eski dosyayı siler
        /// - Yeni dosyayı yazıyor
        /// - Cache'i invalidate ediyor (Resim tarayıcıda yenilenir!)
        /// </summary>
        public async Task<string> SaveImageAsync(
            byte[] imageBytes,
            string fileName,
            string subfolder = "avatars")
        {
            try
            {
                var uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", subfolder);

                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                    _logger.LogInformation("📁 Klasör oluşturuldu: {Folder}", uploadFolder);
                }

                var filePath = Path.Combine(uploadFolder, fileName);
                var relativeImagePath = $"/images/{subfolder}/{fileName}";

                // STEP 1: Eski dosyayı sil (varsa)
                if (File.Exists(filePath))
                {
                    try
                    {
                        File.Delete(filePath);
                        _logger.LogInformation("🗑️  Eski dosya silindi: {FileName}", fileName);

                        // Windows file lock sorunu için kısacık bir delay
                        await Task.Delay(50);
                    }
                    catch (IOException ex)
                    {
                        _logger.LogWarning(ex, "⚠️  Eski dosya silinemedi, yenisi ile değiştirilecek");
                    }
                }

                // STEP 2: Yeni dosyayı kaydet
                await File.WriteAllBytesAsync(filePath, imageBytes);
                _logger.LogInformation("✅ Dosya kaydedildi: {FilePath}", filePath);

                // STEP 3: ⭐ CACHE'İ INVALIDATE ET
                // Bu KRITIK! Aynı adda yüklenen resim için cache'i temizle.
                // Sonraki sayfa yüklemesinde, CachedImage component'i
                // ImageCacheService'ten yeni ETag alacak ve browser yeni resmi indirecek.
                _imageCacheService.InvalidateCache(relativeImagePath);
                _logger.LogInformation("🧹 Cache invalidate edildi: {RelativePath}", relativeImagePath);

                return relativeImagePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Dosya kaydedilirken hata: {FileName}", fileName);
                throw;
            }
        }

        /// <summary>
        /// Dosyayı siler ve cache'i invalidate eder
        /// </summary>
        public bool DeleteImage(string imagePath)
        {
            try
            {
                if (string.IsNullOrEmpty(imagePath) || !imagePath.StartsWith("/images/"))
                {
                    return false;
                }

                var fullPath = GetFullPath(imagePath);

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    _logger.LogInformation("✅ Dosya silindi: {FilePath}", fullPath);

                    // Cache'i invalidate et
                    _imageCacheService.InvalidateCache(imagePath);
                    _logger.LogInformation("🧹 Silinme sonrası cache invalidate edildi");

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Dosya silinirken hata: {ImagePath}", imagePath);
                return false;
            }
        }

        /// <summary>
        /// TC Kimlik No'dan güvenli dosya adı oluştur
        /// Format: "28165202398.jpg"
        /// </summary>
        public string GenerateSafeFileName(string tcKimlikNo, string extension = ".jpg")
        {
            var safeTcKimlikNo = new string(tcKimlikNo.Where(char.IsDigit).ToArray());
            if (!extension.StartsWith("."))
            {
                extension = "." + extension;
            }
            return $"{safeTcKimlikNo}{extension}";
        }

        /// <summary>
        /// Göreli yolu tam dosya yoluna çevirir
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
