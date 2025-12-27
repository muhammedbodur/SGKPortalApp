using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System;

namespace SGKPortalApp.BusinessObjectLayer.Attributes
{
    /// <summary>
    /// Entity seviyesinde audit logging davranışını kontrol eder
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class AuditLogAttribute : Attribute
    {
        /// <summary>
        /// INSERT işlemleri loglanacak mı?
        /// </summary>
        public bool Insert { get; set; } = true;

        /// <summary>
        /// UPDATE işlemleri loglanacak mı?
        /// </summary>
        public bool Update { get; set; } = true;

        /// <summary>
        /// DELETE işlemleri loglanacak mı?
        /// </summary>
        public bool Delete { get; set; } = true;

        /// <summary>
        /// Storage stratejisi (DB, File, veya SmartHybrid)
        /// </summary>
        public StorageStrategy StorageStrategy { get; set; } = StorageStrategy.SmartHybrid;

        /// <summary>
        /// SmartHybrid stratejisinde threshold (byte cinsinden)
        /// </summary>
        public int HybridThresholdBytes { get; set; } = 1024; // 1 KB

        /// <summary>
        /// Bulk işlemlerde kaç kayıttan fazlası summary log olsun?
        /// </summary>
        public int BulkThreshold { get; set; } = 50;

        /// <summary>
        /// İlişkili değişiklikleri grupla (Transaction-based grouping)
        /// </summary>
        public bool GroupRelatedChanges { get; set; } = true;
    }
}
