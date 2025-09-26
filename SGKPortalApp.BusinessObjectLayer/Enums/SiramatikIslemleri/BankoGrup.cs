using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri
{
    public enum BankoGrup
    {
        [Display(Name = "Ana Grup")]
        AnaGrup,

        [Display(Name = "Orta Grup")]
        OrtaGrup,

        [Display(Name = "Alt Grup")]
        AltGrup
    }
}
