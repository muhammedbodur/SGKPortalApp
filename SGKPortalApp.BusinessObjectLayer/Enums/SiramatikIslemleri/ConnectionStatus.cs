using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri
{
    public enum ConnectionStatus : int
    {
        [Display(Name = "Çevrim İçi")]
        online = 1,

        [Display(Name = "Çevrim Dışı")]
        offline = 0,
    }
}
