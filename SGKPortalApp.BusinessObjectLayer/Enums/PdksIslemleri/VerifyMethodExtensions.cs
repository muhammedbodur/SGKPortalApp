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
                VerifyMethod.Password => "Şifre",
                VerifyMethod.Fingerprint => "Parmak İzi",
                VerifyMethod.Face => "Yüz Tanıma",
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
                VerifyMethod.Fingerprint => "bg-label-success",
                VerifyMethod.Face => "bg-label-info",
                VerifyMethod.Password => "bg-label-warning",
                _ => "bg-label-secondary"
            };
        }
    }
}
