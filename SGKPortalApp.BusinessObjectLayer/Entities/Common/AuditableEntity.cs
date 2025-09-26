using System;

namespace SGKPortalApp.BusinessObjectLayer.Entities.Common
{
    public abstract class AuditableEntity : BaseEntity
    {
        public string? EkleyenKullanici { get; set; }
        public string? DuzenleyenKullanici { get; set; }
        public bool SilindiMi { get; set; } = false;
        public DateTime? SilinmeTarihi { get; set; }
        public string? SilenKullanici { get; set; }
    }
}