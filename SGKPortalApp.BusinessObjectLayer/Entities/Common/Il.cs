using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.Common
{
    public class Il
    {
        [Key]
        public int IlId { get; set; }

        public required string IlAdi { get; set; }

        [InverseProperty("Il")]
        public ICollection<Ilce>? Ilceler { get; set; } = new List<Ilce>();
    }
}