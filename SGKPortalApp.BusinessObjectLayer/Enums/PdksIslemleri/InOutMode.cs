namespace SGKPortalApp.BusinessObjectLayer.Enums.PdksIslemleri
{
    /// <summary>
    /// ZKTeco cihaz giriş/çıkış durumu
    /// </summary>
    public enum InOutMode
    {
        /// <summary>
        /// Giriş
        /// </summary>
        CheckIn = 0,

        /// <summary>
        /// Çıkış
        /// </summary>
        CheckOut = 1,

        /// <summary>
        /// Mola çıkış
        /// </summary>
        BreakOut = 2,

        /// <summary>
        /// Mola giriş
        /// </summary>
        BreakIn = 3,

        /// <summary>
        /// Mesai başlangıç
        /// </summary>
        OvertimeIn = 4,

        /// <summary>
        /// Mesai bitiş
        /// </summary>
        OvertimeOut = 5
    }
}
