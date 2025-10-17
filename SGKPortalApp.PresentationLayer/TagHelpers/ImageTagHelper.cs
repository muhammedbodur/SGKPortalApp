using Microsoft.AspNetCore.Razor.TagHelpers;

namespace SGKPortalApp.PresentationLayer.TagHelpers
{
    /// <summary>
    /// Image tag'lerine cache-busting özelliği ekleyen TagHelper
    /// Kullanım: <img src="@Personel.Resim" asp-cache-bust="true" />
    /// Otomatik: /images/ veya /uploads/ içeren tüm resimlere otomatik uygulanır
    /// </summary>
    [HtmlTargetElement("img")]
    public class ImageTagHelper : TagHelper
    {
        private const string CacheBustAttributeName = "asp-cache-bust";

        // Otomatik cache-busting uygulanacak klasörler
        private static readonly string[] AutoCacheBustPaths = new[]
        {
            "/images/",
            "/uploads/"
        };

        /// <summary>
        /// Cache-busting aktif mi?
        /// true: Zorla aktif
        /// false: Zorla pasif
        /// null: Otomatik (path kontrolü)
        /// </summary>
        [HtmlAttributeName(CacheBustAttributeName)]
        public bool? CacheBust { get; set; }

        /// <summary>
        /// Cache-busting stratejisi (timestamp, version, hash)
        /// Varsayılan: timestamp
        /// </summary>
        [HtmlAttributeName("asp-cache-strategy")]
        public string CacheStrategy { get; set; } = "timestamp";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // Mevcut src attribute'unu al
            var src = output.Attributes["src"]?.Value?.ToString();

            if (string.IsNullOrEmpty(src))
                return;

            // Cache-busting gerekli mi kontrol et
            bool shouldCacheBust = ShouldApplyCacheBusting(src);

            if (!shouldCacheBust)
                return;

            // Cache-busting parametresini oluştur
            string cacheBustParam = CacheStrategy.ToLower() switch
            {
                "timestamp" => $"v={DateTime.Now.Ticks}",
                "version" => $"v={GetApplicationVersion()}",
                "hash" => $"v={GetFileHash(src)}",
                _ => $"v={DateTime.Now.Ticks}"
            };

            // URL'ye parametre ekle
            var separator = src.Contains('?') ? "&" : "?";
            var newSrc = $"{src}{separator}{cacheBustParam}";

            // src attribute'unu güncelle
            output.Attributes.SetAttribute("src", newSrc);

            // asp-cache-bust attribute'unu kaldır (HTML'de görünmesin)
            output.Attributes.RemoveAll(CacheBustAttributeName);
            output.Attributes.RemoveAll("asp-cache-strategy");
        }

        /// <summary>
        /// Uygulama versiyonunu döndürür
        /// </summary>
        private string GetApplicationVersion()
        {
            // Assembly versiyonunu veya appsettings'den versiyonu kullanabilirsin
            return "1.0.0";
        }

        /// <summary>
        /// Dosya hash'ini döndürür (ileride implement edilebilir)
        /// </summary>
        private string GetFileHash(string filePath)
        {
            // Dosya hash'i hesaplama (şimdilik timestamp kullan)
            return DateTime.Now.Ticks.ToString();
        }

        /// <summary>
        /// Cache-busting uygulanmalı mı kontrol eder
        /// </summary>
        private bool ShouldApplyCacheBusting(string src)
        {
            // Manuel olarak belirtilmişse ona göre hareket et
            if (CacheBust.HasValue)
                return CacheBust.Value;

            // Otomatik kontrol: Belirtilen path'lerden birini içeriyor mu?
            foreach (var path in AutoCacheBustPaths)
            {
                if (src.Contains(path, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }
    }
}
