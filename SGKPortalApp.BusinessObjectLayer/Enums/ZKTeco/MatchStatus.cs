namespace SGKPortalApp.BusinessObjectLayer.Enums.ZKTeco
{
    /// <summary>
    /// Eşleşme durumu
    /// </summary>
    public enum MatchStatus
    {
        /// <summary>
        /// Tam eşleşme
        /// </summary>
        PerfectMatch = 0,

        /// <summary>
        /// Kısmi eşleşme (bazı alanlar uyuşmuyor)
        /// </summary>
        PartialMatch = 1,

        /// <summary>
        /// Sadece cihazda var, DB'de yok
        /// </summary>
        DeviceOnly = 2,

        /// <summary>
        /// Sadece DB'de var, cihazda yok
        /// </summary>
        PersonelOnly = 3,

        /// <summary>
        /// Hiçbir yerde bulunamadı
        /// </summary>
        NotFound = 4
    }
}
