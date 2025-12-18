namespace SGKPortalApp.BusinessObjectLayer.DTOs.Common
{
    /// <summary>
    /// Dropdown için kullanılan DTO (Açıklama ile)
    /// ActionType gibi enum'lar için kullanılır
    /// </summary>
    public class DropdownDto
    {
        public int Id { get; set; }
        public string Ad { get; set; } = string.Empty;
        public string? Aciklama { get; set; }
    }
}
