using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.Common
{
    /// <summary>
    /// SignalR Hub Bağlantı Bilgileri
    /// Bir kullanıcının birden fazla aktif bağlantısı olabilir (farklı tab'lar)
    /// TcKimlikNo ZORUNLU - Her bağlantı bir User'a ait olmalı (Personel veya TV User)
    /// </summary>
    public class HubConnection : AuditableEntity
    {
        [Key]
        public int HubConnectionId { get; set; }

        // ZORUNLU - Her bağlantı bir User'a ait olmalı
        // NOT: Unique Index YOK - Bir kullanıcı birden fazla bağlantı açabilir
        [Required]
        [StringLength(11)]
        public string TcKimlikNo { get; set; } = string.Empty;

        // User ile Many-to-One ilişki (bir user'ın birden fazla connection'ı olabilir)
        [ForeignKey(nameof(TcKimlikNo))]
        [InverseProperty("HubConnections")]
        public User? User { get; set; }

        // SignalR ConnectionId (Unique - Her bağlantı benzersiz)
        [Required]
        [StringLength(100)]
        public string ConnectionId { get; set; } = string.Empty;

        // Bağlantı Tipi (MainLayout, TvDisplay, BankoMode, Monitoring)
        [Required]
        [StringLength(50)]
        public string ConnectionType { get; set; } = "MainLayout";

        public ConnectionStatus ConnectionStatus { get; set; }
        
        public DateTime ConnectedAt { get; set; } = DateTime.Now;
        public DateTime LastActivityAt { get; set; } = DateTime.Now;
        public DateTime IslemZamani { get; set; } = DateTime.Now;

        // Navigation properties
        [InverseProperty("HubConnection")]
        public HubTvConnection? HubTvConnection { get; set; }

        [InverseProperty("HubConnection")]
        public HubBankoConnection? HubBankoConnection { get; set; }
    }
}