using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri
{
    public class HubConnection : BaseEntity
    {
        [Key]
        public int HubConnectionId { get; set; }

        public required string TcKimlikNo { get; set; }
        [ForeignKey("TcKimlikNo")]
        public Personel? Personel { get; set; }

        public string? ConnectionId { get; set; }
        public ConnectionStatus ConnectionStatus { get; set; }
        public DateTime IslemZamani { get; set; } = DateTime.Now;
    }
}