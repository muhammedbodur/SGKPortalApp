using System;

namespace SGKPortalApp.BusinessObjectLayer.Attributes
{
    /// <summary>
    /// Property seviyesinde hassas veri işaretlemesi
    /// Bu attribute'a sahip property'ler audit log'da maskeli gösterilir
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SensitiveDataAttribute : Attribute
    {
        /// <summary>
        /// Maskeleme formatı (default: "***")
        /// </summary>
        public string MaskFormat { get; set; } = "***";

        /// <summary>
        /// Hiç loglanmasın mı? (true ise log'a hiç yazılmaz)
        /// </summary>
        public bool ExcludeFromLog { get; set; } = false;

        /// <summary>
        /// Sadece ilk N karakteri göster (örnek: kartNo için ilk 4 hane)
        /// </summary>
        public int? ShowFirstChars { get; set; }

        /// <summary>
        /// Sadece son N karakteri göster (örnek: kartNo için son 4 hane)
        /// </summary>
        public int? ShowLastChars { get; set; }
    }
}
