using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri
{
    /// <summary>
    /// SignalR Hub Bağlantı Bilgileri
    /// User tablosu ile 1-1 ilişkili (anlık bağlantı durumu)
    /// </summary>
    public class HubConnection : AuditableEntity
    {
        [Key]
        public int HubConnectionId { get; set; }

        [Required]
        [StringLength(11)]
        public string TcKimlikNo { get; set; } = string.Empty;

        // User ile One-to-One ilişki
        [ForeignKey(nameof(TcKimlikNo))]
        public User? User { get; set; }

        [StringLength(100)]
        public string? ConnectionId { get; set; }

        public ConnectionStatus ConnectionStatus { get; set; }
        public DateTime IslemZamani { get; set; } = DateTime.Now;
    }
}