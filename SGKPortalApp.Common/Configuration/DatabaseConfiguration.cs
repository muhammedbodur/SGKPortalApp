using Microsoft.Extensions.Configuration;

namespace SGKPortalApp.Common.Configuration
{
    /// <summary>
    /// Veritabanı bağlantı ayarlarını yöneten merkezi sınıf
    /// Tüm connection string bilgileri appsettings.Shared.json'dan okunur
    /// </summary>
    public static class DatabaseConfiguration
    {
        /// <summary>
        /// Connection string'i configuration'dan alır
        /// Öncelik sırası: 
        /// 1. appsettings.Shared.json (Production/Development ortak)
        /// 2. appsettings.json (Layer bazlı - fallback)
        /// </summary>
        /// <param name="configuration">IConfiguration instance</param>
        /// <returns>Connection string</returns>
        /// <exception cref="InvalidOperationException">Connection string bulunamazsa</exception>
        public static string GetConnectionString(IConfiguration configuration)
        {
            // ConnectionStrings:DefaultConnection'ı oku
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "❌ 'DefaultConnection' connection string bulunamadı!\n" +
                    "📋 Kontrol edilecek dosyalar:\n" +
                    "   1. appsettings.Shared.json (Önerilen - Tüm layerlar için ortak)\n" +
                    "   2. appsettings.json (Layer bazlı - fallback)\n" +
                    "   3. appsettings.Development.json (Development ortamı)\n" +
                    "   4. appsettings.Production.json (Production ortamı)\n\n" +
                    "📝 Örnek kullanım:\n" +
                    "{\n" +
                    "  \"ConnectionStrings\": {\n" +
                    "    \"DefaultConnection\": \"Server=...;Database=...;User Id=...;Password=...;\"\n" +
                    "  }\n" +
                    "}");
            }

            // Connection string'i doğrula (temel kontroller)
            ValidateConnectionString(connectionString);

            Console.WriteLine($"✅ Connection String alındı: {MaskConnectionString(connectionString)}");
            return connectionString;
        }

        /// <summary>
        /// Connection string'in temel doğruluğunu kontrol eder
        /// </summary>
        private static void ValidateConnectionString(string connectionString)
        {
            var requiredKeywords = new[] { "Server", "Database" };

            foreach (var keyword in requiredKeywords)
            {
                if (!connectionString.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        $"❌ Connection string eksik veya hatalı! '{keyword}' parametresi bulunamadı.\n" +
                        $"📌 Mevcut connection string: {MaskConnectionString(connectionString)}");
                }
            }
        }

        /// <summary>
        /// Connection string'i güvenli şekilde maskeleyerek gösterir (password ve sensitive data gizler)
        /// </summary>
        private static string MaskConnectionString(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                return "[Boş]";

            // Password ve diğer sensitive bilgileri maskele
            var parts = connectionString.Split(';');
            var maskedParts = parts.Select(part =>
            {
                var trimmedPart = part.Trim();

                // Password maskeleme
                if (trimmedPart.StartsWith("Password", StringComparison.OrdinalIgnoreCase) ||
                    trimmedPart.StartsWith("Pwd", StringComparison.OrdinalIgnoreCase))
                {
                    return "Password=***";
                }

                // User ID maskeleme (opsiyonel - güvenlik için)
                if (trimmedPart.StartsWith("User Id", StringComparison.OrdinalIgnoreCase) ||
                    trimmedPart.StartsWith("UID", StringComparison.OrdinalIgnoreCase))
                {
                    var parts = trimmedPart.Split('=');
                    if (parts.Length == 2)
                    {
                        return $"{parts[0]}={parts[1].Substring(0, Math.Min(2, parts[1].Length))}***";
                    }
                }

                return part;
            });

            var masked = string.Join(";", maskedParts);

            // Çok uzunsa kısalt
            if (masked.Length > 120)
            {
                return masked.Substring(0, 117) + "...";
            }

            return masked;
        }

        /// <summary>
        /// Configuration'dan connection string varlığını kontrol eder (test amaçlı)
        /// </summary>
        public static bool IsConnectionStringConfigured(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            return !string.IsNullOrWhiteSpace(connectionString);
        }

        /// <summary>
        /// Connection string'in hangi kaynaktan geldiğini tespit eder (debug amaçlı)
        /// </summary>
        public static string GetConnectionStringSource(IConfiguration configuration)
        {
            // Configuration provider'ları kontrol et
            if (configuration is IConfigurationRoot configRoot)
            {
                var providers = configRoot.Providers.ToList();

                foreach (var provider in providers)
                {
                    // Provider'ın toString çıktısından dosya adını çıkar
                    var providerInfo = provider.ToString() ?? "";

                    if (providerInfo.Contains("Shared", StringComparison.OrdinalIgnoreCase))
                    {
                        return "appsettings.Shared.json ✅ (Önerilen)";
                    }
                }

                // Shared yoksa, hangi dosyadan geldiğini bul
                var lastProvider = providers.LastOrDefault()?.ToString() ?? "Unknown";

                if (lastProvider.Contains("Development"))
                    return "appsettings.Development.json";
                if (lastProvider.Contains("Production"))
                    return "appsettings.Production.json";
                if (lastProvider.Contains("appsettings.json"))
                    return "appsettings.json";
            }

            return "Unknown Source";
        }
    }
}