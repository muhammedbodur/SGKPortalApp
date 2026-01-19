namespace SGKPortalApp.BusinessObjectLayer.Enums.Common
{
    /// <summary>
    /// Resmi tatil tipi
    /// </summary>
    public enum TatilTipi
    {
        /// <summary>
        /// Sabit tatil (her yıl aynı tarih)
        /// Örnek: 23 Nisan, 29 Ekim, 1 Mayıs
        /// </summary>
        SabitTatil = 0,

        /// <summary>
        /// Dini tatil (hicri takvime göre değişken)
        /// Örnek: Ramazan Bayramı, Kurban Bayramı
        /// </summary>
        DiniTatil = 1,

        /// <summary>
        /// Özel tatil (kurum içi özel günler)
        /// Örnek: Kuruluş yıldönümü
        /// </summary>
        OzelTatil = 2
    }
}
