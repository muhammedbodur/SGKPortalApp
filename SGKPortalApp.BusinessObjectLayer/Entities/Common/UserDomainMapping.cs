using System;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.Entities.Common
{
    /// <summary>
    /// Uygulama kullanıcısı (TcKimlikNo) ile Domain kullanıcısı (Windows Auth) eşleştirmesi
    /// Güvenlik analizi için kullanılır - şifre çalınması tespiti
    /// </summary>
    public class UserDomainMapping : BaseEntity
    {
        [Key]
        public int UserDomainMappingId { get; set; }

        [Required]
        [StringLength(11)]
        public required string TcKimlikNo { get; set; }

        [Required]
        [StringLength(200)]
        public required string DomainUser { get; set; }  // "DOMAIN\username"

        [StringLength(100)]
        public string? MachineName { get; set; }  // Son bilinen bilgisayar

        public DateTime LastVerified { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;

        [StringLength(500)]
        public string? Notes { get; set; }  // Admin notları
    }
}
