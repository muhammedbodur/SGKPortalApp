namespace SGKPortalApp.BusinessObjectLayer.Configuration
{
    /// <summary>
    /// Elasticsearch yapılandırma ayarları
    /// </summary>
    public class ElasticsearchSettings
    {
        public const string SectionName = "Elasticsearch";

        /// <summary>
        /// Elasticsearch sunucu adresi
        /// </summary>
        public string Uri { get; set; } = "http://localhost:9200";

        /// <summary>
        /// Personel index adı
        /// </summary>
        public string IndexName { get; set; } = "sgk_personel";

        /// <summary>
        /// Kullanıcı adı (opsiyonel)
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// Şifre (opsiyonel)
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Autocomplete özelliği açık mı?
        /// </summary>
        public bool EnableAutocomplete { get; set; } = true;

        /// <summary>
        /// Synonym tanımları (yanlış yazım düzeltme)
        /// Örnek: "pirim => prim"
        /// </summary>
        public List<string> Synonyms { get; set; } = new();
    }
}
