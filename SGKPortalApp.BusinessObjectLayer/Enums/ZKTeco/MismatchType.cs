namespace SGKPortalApp.BusinessObjectLayer.Enums.ZKTeco
{
    /// <summary>
    /// Uyuşmazlık tipi
    /// </summary>
    public enum MismatchType
    {
        /// <summary>
        /// EnrollNumber uyuşmazlığı
        /// </summary>
        EnrollNumberMismatch = 0,

        /// <summary>
        /// İsim uyuşmazlığı
        /// </summary>
        NameMismatch = 1,

        /// <summary>
        /// Kart numarası uyuşmazlığı
        /// </summary>
        CardNumberMismatch = 2,

        /// <summary>
        /// Personel kayıt no uyuşmazlığı
        /// </summary>
        PersonelKayitNoMismatch = 3,

        /// <summary>
        /// Cihazda kullanıcı yok
        /// </summary>
        UserNotOnDevice = 4,

        /// <summary>
        /// DB'de personel yok
        /// </summary>
        PersonelNotInDb = 5
    }
}
