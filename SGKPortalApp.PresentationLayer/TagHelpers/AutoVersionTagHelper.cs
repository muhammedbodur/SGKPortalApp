using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace SGKPortalApp.PresentationLayer.TagHelpers
{
    /// <summary>
    /// Tüm custom JS/CSS dosyalarına otomatik version (cache busting) ekler
    /// ~/js/, ~/css/ veya /js/, /css/ ile başlayan dosyalara otomatik asp-append-version="true" uygular
    /// </summary>
    [HtmlTargetElement("script", Attributes = "src")]
    [HtmlTargetElement("link", Attributes = "href")]
    public class AutoVersionTagHelper : TagHelper
    {
        private const string AUTO_VERSION_ATTRIBUTE = "auto-version";
        private const string ASP_APPEND_VERSION_ATTRIBUTE = "asp-append-version";

        /// <summary>
        /// Otomatik versioning devre dışı bırakmak için: auto-version="false"
        /// </summary>
        [HtmlAttributeName(AUTO_VERSION_ATTRIBUTE)]
        public bool? AutoVersion { get; set; }

        // Order: -1000 -> asp-append-version tag helper'dan ÖNCE çalış
        public override int Order => -1000;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // 1. Eğer auto-version="false" ise, atla
            if (AutoVersion.HasValue && !AutoVersion.Value)
            {
                return;
            }

            // 2. Zaten asp-append-version varsa, atla (manuel override)
            if (context.AllAttributes.ContainsName(ASP_APPEND_VERSION_ATTRIBUTE))
            {
                return;
            }

            // 3. src veya href attribute'ünü al
            string? path = null;
            if (context.AllAttributes.ContainsName("src"))
            {
                path = context.AllAttributes["src"]?.Value?.ToString();
            }
            else if (context.AllAttributes.ContainsName("href"))
            {
                path = context.AllAttributes["href"]?.Value?.ToString();
            }

            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            // 4. Sadece custom dosyalara version ekle (vendor'lara değil)
            if (IsCustomFile(path))
            {
                // asp-append-version="true" ekle
                output.Attributes.SetAttribute(ASP_APPEND_VERSION_ATTRIBUTE, "true");
            }
        }

        /// <summary>
        /// Custom (bizim yazdığımız) dosya mı kontrol et
        /// </summary>
        private bool IsCustomFile(string path)
        {
            // External URL'leri atla (CDN, http://, https://)
            if (path.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("//", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            // Blazor framework dosyalarını atla
            if (path.Contains("_framework/", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            // Vendor/library dosyalarını atla
            if (path.Contains("/vendor/", StringComparison.OrdinalIgnoreCase) ||
                path.Contains("/lib/", StringComparison.OrdinalIgnoreCase) ||
                path.Contains("/portal/assets/", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            // Custom klasörler: ~/js/, ~/css/, /js/, /css/
            if (path.Contains("/js/", StringComparison.OrdinalIgnoreCase) ||
                path.Contains("/css/", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }
    }
}
