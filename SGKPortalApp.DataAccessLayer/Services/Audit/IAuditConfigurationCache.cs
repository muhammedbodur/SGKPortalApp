using System;

namespace SGKPortalApp.DataAccessLayer.Services.Audit
{
    /// <summary>
    /// Entity audit konfigürasyonlarını cache'ler (reflection overhead'i önlemek için)
    /// </summary>
    public interface IAuditConfigurationCache
    {
        /// <summary>
        /// Entity tipi için audit konfigürasyonunu döndürür (cache'den)
        /// </summary>
        AuditConfig GetConfig(Type entityType);

        /// <summary>
        /// Cache'i temizler
        /// </summary>
        void ClearCache();
    }
}
