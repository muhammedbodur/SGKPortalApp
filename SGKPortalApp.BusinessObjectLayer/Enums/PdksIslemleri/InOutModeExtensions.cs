namespace SGKPortalApp.BusinessObjectLayer.Enums.PdksIslemleri
{
    /// <summary>
    /// InOutMode enum extension metodları
    /// </summary>
    public static class InOutModeExtensions
    {
        /// <summary>
        /// Giriş/Çıkış durumunun Türkçe açıklamasını döner
        /// </summary>
        public static string ToDisplayText(this InOutMode mode)
        {
            return mode switch
            {
                InOutMode.CheckIn => "Giriş",
                InOutMode.CheckOut => "Çıkış",
                InOutMode.BreakOut => "Mola Çıkış",
                InOutMode.BreakIn => "Mola Giriş",
                InOutMode.OvertimeIn => "Mesai Başlangıç",
                InOutMode.OvertimeOut => "Mesai Bitiş",
                _ => "Bilinmiyor"
            };
        }

        /// <summary>
        /// Badge CSS class döner
        /// </summary>
        public static string ToBadgeClass(this InOutMode mode)
        {
            return mode switch
            {
                InOutMode.CheckIn => "bg-success",
                InOutMode.CheckOut => "bg-danger",
                InOutMode.BreakOut => "bg-warning",
                InOutMode.BreakIn => "bg-info",
                InOutMode.OvertimeIn => "bg-primary",
                InOutMode.OvertimeOut => "bg-secondary",
                _ => "bg-secondary"
            };
        }
    }
}
