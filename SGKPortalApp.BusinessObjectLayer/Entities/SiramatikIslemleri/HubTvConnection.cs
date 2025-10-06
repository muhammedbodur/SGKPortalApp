using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri
{
    public class HubTvConnection : AuditableEntity
    {
        [Key]
        public int HubTvConnectionId { get; set; }

        public int TvId { get; set; }
        [ForeignKey("TvId")]
        [InverseProperty("HubTvConnection")]
        public required Tv Tv { get; set; }

        public string? ConnectionId { get; set; }
        public ConnectionStatus ConnectionStatus { get; set; }
        public DateTime IslemZamani { get; set; } = DateTime.Now;
    }
}