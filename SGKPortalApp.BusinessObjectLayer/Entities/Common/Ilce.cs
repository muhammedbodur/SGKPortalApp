using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.Common
{
    public class Ilce
    {
        [Key]
        public int IlceId { get; set; }

        public int IlId { get; set; }
        [ForeignKey("IlId")]
        [InverseProperty("Ilceler")]
        public required Il Il { get; set; }

        public required string IlceAdi { get; set; }
    }
}