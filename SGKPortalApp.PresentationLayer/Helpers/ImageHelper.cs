using Microsoft.Extensions.Logging;
using SkiaSharp;

namespace SGKPortalApp.PresentationLayer.Helpers
{
    /// <summary>
    /// Resim işleme, doğrulama ve kaydetme işlemlerini SkiaSharp kütüphanesi kullanarak yönetir.
    /// Bu versiyon, tüm derleyici hataları için düzeltilmiştir.
    /// </summary>
    public class ImageHelper
    {
        private readonly ILogger<ImageHelper> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ImageHelper(ILogger<ImageHelper> logger, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _logger.LogInformation("✅ ImageHelper (SkiaSharp - Final Version) başlatıldı.");
        }

        /// <summary>
        /// Gelen stream'i okur, resim olup olmadığını doğrular, yeniden boyutlandırır ve optimize eder.
        /// </summary>
        public Task<byte[]?> LoadResizeAndOptimizeAsync(
            Stream imageStream,
            int maxWidth = 400,
            int maxHeight = 400,
            int quality = 85)
        {
            if (imageStream == null || imageStream.Length == 0)
            {
                _logger.LogWarning("Stream boş veya null olduğu için resim işlenemedi.");
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
                        _logger.LogWarning("❌ SkiaSharp stream'i çözemedi. Geçersiz veya desteklenmeyen resim formatı.");
                        return null;
                    }

                    _logger.LogInformation("✅ Resim (SkiaSharp) başarıyla doğrulandı. Orijinal boyut: {Width}x{Height}", originalBitmap.Width, originalBitmap.Height);

                    var ratioX = (double)maxWidth / originalBitmap.Width;
                    var ratioY = (double)maxHeight / originalBitmap.Height;
                    var ratio = Math.Min(ratioX, ratioY);

                    if (ratio > 1) ratio = 1;

                    var newWidth = (int)(originalBitmap.Width * ratio);
                    var newHeight = (int)(originalBitmap.Height * ratio);

                    var resizeInfo = new SKImageInfo(newWidth, newHeight);

                    // HATA DÜZELTİLDİ: SKSamplingOptions için doğru constructor'ı kullanıyoruz.
                    // Bu, yüksek kaliteli (Linear) filtreleme ve mipmapping kullanarak pürüzsüz bir sonuç sağlar.
                    using var resizedBitmap = originalBitmap.Resize(resizeInfo, new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear));

                    _logger.LogInformation("📐 Resim (SkiaSharp) yeniden boyutlandırıldı: {Width}x{Height}", resizedBitmap.Width, resizedBitmap.Height);

                    using var image = SKImage.FromBitmap(resizedBitmap);
                    using var data = image.Encode(SKEncodedImageFormat.Jpeg, quality);

                    using var ms = new MemoryStream();
                    data.SaveTo(ms);

                    var resultBytes = ms.ToArray();
                    _logger.LogInformation("💾 Resim (SkiaSharp) optimize edildi. Boyut: {Size}KB", resultBytes.Length / 1024);

                    return resultBytes;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Resim (SkiaSharp) işleme sırasında beklenmedik bir hata oluştu.");
                    return null;
                }
            });
        }

        // Bu metodun geri kalanı aynı kalabilir.
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
                }

                var filePath = Path.Combine(uploadFolder, fileName);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                await File.WriteAllBytesAsync(filePath, imageBytes);
                _logger.LogInformation("✅ Dosya kaydedildi: {FilePath}", filePath);

                return $"/images/{subfolder}/{fileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Dosya kaydedilirken hata oluştu: {FileName}", fileName);
                throw;
            }
        }

        public bool DeleteImage(string imagePath)
        {
            try
            {
                if (string.IsNullOrEmpty(imagePath) || !imagePath.StartsWith("/images/"))
                {
                    return false;
                }
                var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, imagePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Dosya silinirken hata oluştu: {ImagePath}", imagePath);
                return false;
            }
        }

        public string GenerateSafeFileName(string tcKimlikNo, string extension = ".jpg")
        {
            var safeTcKimlikNo = new string(tcKimlikNo.Where(char.IsDigit).ToArray());
            if (!extension.StartsWith("."))
            {
                extension = "." + extension;
            }
            return $"{safeTcKimlikNo}{extension}";
        }
    }
}