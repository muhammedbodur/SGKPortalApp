namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri
{
    /// <summary>
    /// Filtrelenmiş izin/mazeret talepleri için response DTO
    /// Tuple yerine kullanılır (JSON serialization için)
    /// </summary>
    public class IzinMazeretTalepFilterResponseDto
    {
        public List<IzinMazeretTalepListResponseDto> Items { get; set; } = new();
        public int TotalCount { get; set; }
    }
}
