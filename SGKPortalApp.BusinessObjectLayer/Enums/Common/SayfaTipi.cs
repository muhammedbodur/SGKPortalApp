namespace SGKPortalApp.BusinessObjectLayer.Enums.Common
{
    /// <summary>
    /// Sayfa erişim tipi
    /// </summary>
    public enum SayfaTipi
    {
        /// <summary>
        /// Herkes erişebilir (login gerekmez) - Örn: Login, ForgotPassword
        /// </summary>
        Public = 0,

        /// <summary>
        /// Login yeterli, özel yetki gerekmez - Örn: Dashboard, Profil
        /// </summary>
        Authenticated = 1,

        /// <summary>
        /// Login + Spesifik yetki gerekir - Örn: Personel Listesi, Banko Yönetimi
        /// </summary>
        Protected = 2
    }
}
