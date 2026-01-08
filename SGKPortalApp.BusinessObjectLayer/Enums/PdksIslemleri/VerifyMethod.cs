namespace SGKPortalApp.BusinessObjectLayer.Enums.PdksIslemleri
{
    /// <summary>
    /// ZKTeco cihaz doğrulama yöntemleri
    /// </summary>
    public enum VerifyMethod
    {
        /// <summary>
        /// Şifre ile doğrulama
        /// </summary>
        Password = 0,

        /// <summary>
        /// Parmak izi ile doğrulama
        /// </summary>
        Fingerprint = 1,

        /// <summary>
        /// Yüz tanıma ile doğrulama
        /// </summary>
        Face = 3,

        /// <summary>
        /// Kart ile doğrulama
        /// </summary>
        Card = 15
    }
}
