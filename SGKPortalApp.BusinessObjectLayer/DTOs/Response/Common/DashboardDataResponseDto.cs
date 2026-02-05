namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common
{
    /// <summary>
    /// Dashboard için tüm verileri içeren birleşik Response DTO
    /// </summary>
    public class DashboardDataResponseDto
    {
        /// <summary>
        /// Slider için ana haberler (görsel olan)
        /// </summary>
        public List<HaberDashboardResponseDto> SliderHaberler { get; set; } = new();

        /// <summary>
        /// Liste halinde haberler
        /// </summary>
        public List<HaberDashboardResponseDto> ListeHaberler { get; set; } = new();

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
}
