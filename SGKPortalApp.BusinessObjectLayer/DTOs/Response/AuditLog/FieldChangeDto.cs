namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.AuditLog
{
    /// <summary>
    /// Field değişikliği detayı
    /// </summary>
    public class FieldChangeDto
    {
        public string FieldName { get; set; } = string.Empty;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }

        /// <summary>
        /// Eski değerin kullanıcı dostu gösterimi (FK lookup, enum açıklama, vb)
        /// </summary>
        public string? OldValueDisplay { get; set; }

        /// <summary>
        /// Yeni değerin kullanıcı dostu gösterimi (FK lookup, enum açıklama, vb)
        /// </summary>
        public string? NewValueDisplay { get; set; }

        public bool IsChanged => OldValue != NewValue;
    }
}
