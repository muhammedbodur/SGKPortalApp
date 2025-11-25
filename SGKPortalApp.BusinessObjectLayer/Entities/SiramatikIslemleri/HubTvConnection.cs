using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri
{
    /// <summary>
    /// TV SignalR Hub bağlantı bilgileri
    /// TV'lerin online/offline durumunu takip eder
    /// HubConnection ile 1-1 ilişkili (zorunlu - TV User veya Personel)
    /// Birden fazla kullanıcı aynı TV'yi izleyebilir (farklı tab'lar)
    /// </summary>
    public class HubTvConnection : AuditableEntity
    {
        [Key]
        public int HubTvConnectionId { get; set; }

        // HubConnection ile 1-1 ilişki (Zorunlu - TV User veya Personel)
        [Required]
        public int HubConnectionId { get; set; }
        [ForeignKey("HubConnectionId")]
        [InverseProperty("HubTvConnection")]
        public HubConnection? HubConnection { get; set; }

        // TV ile Many-to-One ilişki (Zorunlu - Birden fazla bağlantı aynı TV'yi izleyebilir)
        [Required]
        public int TvId { get; set; }
        [ForeignKey("TvId")]
        [InverseProperty("HubTvConnections")]
        public Tv? Tv { get; set; }

        public DateTime IslemZamani { get; set; } = DateTime.Now;
    }
}