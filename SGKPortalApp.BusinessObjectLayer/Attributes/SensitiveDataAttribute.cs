using System;

namespace SGKPortalApp.BusinessObjectLayer.Attributes
{
    /// <summary>
    /// Property seviyesinde hassas veri işaretlemesi
    /// Bu attribute'a sahip property'ler audit log'da "***" ile maskeli gösterilir
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SensitiveDataAttribute : Attribute
    {
        /// <summary>
        /// Hiç loglanmasın mı? (true ise log'a hiç yazılmaz)
        /// </summary>
        public bool ExcludeFromLog { get; set; } = false;
    }
}
