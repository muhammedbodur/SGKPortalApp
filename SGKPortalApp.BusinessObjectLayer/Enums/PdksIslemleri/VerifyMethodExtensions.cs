namespace SGKPortalApp.BusinessObjectLayer.Enums.PdksIslemleri
{
    /// <summary>
    /// VerifyMethod enum extension metodları
    /// </summary>
    public static class VerifyMethodExtensions
    {
        /// <summary>
        /// Doğrulama yönteminin Türkçe açıklamasını döner
        /// </summary>
        public static string ToDisplayText(this VerifyMethod method)
        {
            return method switch
            {
                VerifyMethod.Card => "Kart",
                _ => "Bilinmiyor"
            };
        }

        /// <summary>
        /// Badge CSS class döner
        /// </summary>
        public static string ToBadgeClass(this VerifyMethod method)
        {
            return method switch
            {
                VerifyMethod.Card => "bg-label-primary",
                _ => "bg-label-secondary"
            };
        }
    }
}
