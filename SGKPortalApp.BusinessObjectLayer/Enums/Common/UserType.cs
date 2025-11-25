namespace SGKPortalApp.BusinessObjectLayer.Enums.Common
{
    /// <summary>
    /// Kullanıcı tipi
    /// </summary>
    public enum UserType
    {
        /// <summary>
        /// Normal personel kullanıcısı
        /// Personel ilişkisi var, tüm yetkilere sahip
        /// </summary>
        Personel = 1,

        /// <summary>
        /// TV için oluşturulan kullanıcı
        /// Tv ilişkisi var, sadece TV Display sayfasını açabilir
        /// </summary>
        TvUser = 2
    }
}
