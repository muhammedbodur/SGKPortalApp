using System;

namespace SGKPortalApp.BusinessObjectLayer.Attributes
{
    /// <summary>
    /// Bu entity için audit logging tamamen devre dışı bırakılır
    /// Örnek: LoginLogoutLog, SignalREventLog, DatabaseLog gibi log tabloları
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class NoAuditLogAttribute : Attribute
    {
        /// <summary>
        /// Loglama devre dışı bırakılma sebebi (dokümantasyon için)
        /// </summary>
        public string Reason { get; set; } = "This entity should not be audited";
    }
}
