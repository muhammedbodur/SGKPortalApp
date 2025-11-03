using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common
{
    /// <summary>
    /// User Update DTO - Sadece dinamik alanlar
    /// Statik alanlar (Email, TelefonNo) Personel'den güncellenir
    /// </summary>
    public class UserUpdateRequestDto
    {
        public bool AktifMi { get; set; }
    }
}
