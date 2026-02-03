namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.Dashboard
{
    /// <summary>
    /// Dashboard için tüm verileri içeren birleşik Response DTO
    /// </summary>
    public class DashboardDataResponseDto
    {
        /// <summary>
        /// Slider için ana duyurular (görsel olan)
        /// </summary>
        public List<DuyuruResponseDto> SliderDuyurular { get; set; } = new();

        /// <summary>
        /// Liste halinde duyurular
        /// </summary>
        public List<DuyuruResponseDto> ListeDuyurular { get; set; } = new();

        /// <summary>
        /// Sık kullanılan programlar
        /// </summary>
        public List<SikKullanilanProgramResponseDto> SikKullanilanProgramlar { get; set; } = new();

        /// <summary>
        /// Önemli linkler
        /// </summary>
        public List<OnemliLinkResponseDto> OnemliLinkler { get; set; } = new();

        /// <summary>
        /// Bugünün menüsü
        /// </summary>
        public GununMenusuResponseDto? GununMenusu { get; set; }

        /// <summary>
        /// Bugün doğanlar
        /// </summary>
        public List<BugunDoganResponseDto> BugunDoganlar { get; set; } = new();
    }

    /// <summary>
    /// Bugün doğan personel bilgisi
    /// </summary>
    public class BugunDoganResponseDto
    {
        public string TcKimlikNo { get; set; } = string.Empty;
        public string AdSoyad { get; set; } = string.Empty;
        public string? Resim { get; set; }
        public string? DepartmanAdi { get; set; }
        public string? ServisAdi { get; set; }
        public DateTime? DogumTarihi { get; set; }
        public int Yas => DogumTarihi.HasValue ? DateTime.Today.Year - DogumTarihi.Value.Year : 0;
    }
}
