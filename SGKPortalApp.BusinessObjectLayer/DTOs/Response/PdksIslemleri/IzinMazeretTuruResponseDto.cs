namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri
{
    /// <summary>
    /// İzin/Mazeret türü Response DTO
    /// </summary>
    public class IzinMazeretTuruResponseDto
    {
        public int IzinMazeretTuruId { get; set; }
        public string TuruAdi { get; set; } = string.Empty;
        public string? KisaKod { get; set; }
        public string? Aciklama { get; set; }
        public bool BirinciOnayciGerekli { get; set; }
        public bool IkinciOnayciGerekli { get; set; }
        public bool PlanliIzinMi { get; set; }
        public int Sira { get; set; }
        public bool IsActive { get; set; }
        public string? RenkKodu { get; set; }
    }
}
