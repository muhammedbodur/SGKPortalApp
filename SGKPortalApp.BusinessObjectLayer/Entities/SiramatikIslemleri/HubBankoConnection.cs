using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri
{
    /// <summary>
    /// Banko SignalR Hub bağlantı bilgileri
    /// Bankoların online/offline durumunu takip eder
    /// HubConnection ile 1-1 ilişkili (zorunlu - personel olmak zorunda)
    /// BankoId ve TcKimlikNo unique - Bir bankoya sadece 1 personel, bir personel sadece 1 bankoda
    /// </summary>
    public class HubBankoConnection : AuditableEntity
    {
        [Key]
        public int HubBankoConnectionId { get; set; }

        // HubConnection ile 1-1 ilişki (Zorunlu - Personel olmak zorunda)
        [Required]
        public int HubConnectionId { get; set; }
        [ForeignKey(nameof(HubConnectionId))]
        [InverseProperty(nameof(HubConnection.HubBankoConnection))]
        public HubConnection? HubConnection { get; set; }

        // Banko ile 1-1 ilişki (Zorunlu, Unique - Bir bankoya sadece 1 personel)
        [Required]
        public int BankoId { get; set; }
        [ForeignKey("BankoId")]
        [InverseProperty("HubBankoConnection")]
        public Banko? Banko { get; set; }

        // Personel TcKimlikNo (Unique - Bir personel aynı anda sadece 1 bankoda)
        [Required]
        [StringLength(11)]
        public string TcKimlikNo { get; set; } = string.Empty;

        // Banko Modu Bilgileri
        public bool BankoModuAktif { get; set; } = true;
        public DateTime BankoModuBaslangic { get; set; } = DateTime.Now;
        public DateTime? BankoModuBitis { get; set; }

        public DateTime IslemZamani { get; set; } = DateTime.Now;
    }
}
