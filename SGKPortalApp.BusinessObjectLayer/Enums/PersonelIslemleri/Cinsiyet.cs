using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri
{
    public enum Cinsiyet
    {
        [Display(Name = "Erkek")]
        erkek,

        [Display(Name = "Kadın")]
        kadin
    }
}
