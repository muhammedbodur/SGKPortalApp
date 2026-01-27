using Microsoft.Extensions.Configuration;

namespace SGKPortalApp.Common.Helpers
{
    /// <summary>
    /// Personel resim yolu yönetimi için merkezi helper.
    /// DB'de sadece filename (örn: "1234.jpg") saklanır.
    /// Bu helper, filename'i web path'e çevirir (örn: "/images/avatars/1234.jpg").
    /// Base path appsettings.Shared.json'dan gelir.
    /// </summary>
    public class PersonelImagePathHelper
    {
        private readonly string _basePath;

        public PersonelImagePathHelper(IConfiguration configuration)
        {
            _basePath = configuration["PersonelImageSettings:BasePath"] ?? "/images/avatars";
        }

        /// <summary>
        /// Filename'i web path'e çevirir.
        /// </summary>
        /// <param name="filename">Sadece dosya adı (örn: "1234.jpg")</param>
        /// <returns>Web path (örn: "/images/avatars/1234.jpg") veya null/empty ise boş string</returns>
        public string GetWebPath(string? filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
                return string.Empty;

            // Eğer zaten tam path ise (eski kayıtlar için backward compatibility)
            if (filename.StartsWith("/") || filename.StartsWith("http://") || filename.StartsWith("https://"))
                return filename;

            // Eğer "images/..." formatında ise başına / ekle
            if (filename.StartsWith("images/"))
                return "/" + filename;

            // Normal durum: sadece filename → base path + filename
            return $"{_basePath.TrimEnd('/')}/{filename.TrimStart('/')}";
        }

        /// <summary>
        /// Web path'den sadece filename'i çıkarır (DB'ye kaydetmek için).
        /// </summary>
        /// <param name="webPath">Tam web path (örn: "/images/avatars/1234.jpg")</param>
        /// <returns>Sadece filename (örn: "1234.jpg")</returns>
        public string ExtractFilename(string? webPath)
        {
            if (string.IsNullOrWhiteSpace(webPath))
                return string.Empty;

            // Eğer zaten sadece filename ise aynen dön
            if (!webPath.Contains('/') && !webPath.Contains('\\'))
                return webPath;

            // Path'den son segment'i al (filename)
            return Path.GetFileName(webPath);
        }

        /// <summary>
        /// TC Kimlik No'dan güvenli dosya adı oluşturur.
        /// </summary>
        /// <param name="tcKimlikNo">TC Kimlik No</param>
        /// <param name="extension">Dosya uzantısı (varsayılan: .jpg)</param>
        /// <returns>Güvenli filename (örn: "28165202398.jpg")</returns>
        public static string GenerateSafeFileName(string tcKimlikNo, string extension = ".jpg")
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
