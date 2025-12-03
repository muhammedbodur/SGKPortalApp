namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;

/// <summary>
/// Hedef banko seçeneği DTO
/// </summary>
public class BankoOptionDto
{
    public int BankoId { get; set; }
    public int BankoNo { get; set; }
    public string PersonelAdi { get; set; } = string.Empty;
    public string PersonelTc { get; set; } = string.Empty;
    public string KatAdi { get; set; } = string.Empty;

    /// <summary>
    /// Display için formatlanmış text
    /// Örnek: "Banko 1 (Zemin Kat) - Ahmet Yılmaz"
    /// </summary>
    public string DisplayText => $"Banko {BankoNo} ({KatAdi}) - {(!string.IsNullOrWhiteSpace(PersonelAdi) ? PersonelAdi : PersonelTc)}";
}
