namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common
{
    public class DropdownItemDto
    {
        public int Id { get; set; }
        public string Ad { get; set; } = string.Empty;
        public string? Metadata { get; set; } // Ek bilgi tutmak için (örn: parent controller ID)
    }
}
