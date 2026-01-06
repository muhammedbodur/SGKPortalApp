using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco
{
    /// <summary>
    /// ZKTeco cihazına kayıtlı kullanıcı/personel bilgileri
    /// ZKTecoApi /api/users endpoint'i ile senkronize
    /// Tablo adı: ZKTeco_User
    /// </summary>
    [Table("ZKTeco_User")]
    public class ZKTecoUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Personel Sicil No (FK to Personel)
        /// Personel tablosuyla ilişki
        /// </summary>
        public int? PersonelSicilNo { get; set; }

        /// <summary>
        /// Navigation property - İlişkili personel
        /// </summary>
        [ForeignKey("PersonelSicilNo")]
        public virtual PersonelIslemleri.Personel? Personel { get; set; }

        /// <summary>
        /// Kayıt/Sicil numarası (EnrollNumber)
        /// ZKTeco cihazındaki unique user ID
        /// Genellikle PersonelSicilNo ile aynı olur ama manuel kart için farklı olabilir
        /// </summary>
        [Required]
        [StringLength(50)]
        public string EnrollNumber { get; set; } = string.Empty;

        /// <summary>
        /// Kullanıcı adı (Ad Soyad)
        /// Personel'den otomatik doldurulur ama manuel kart için manuel girilebilir
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Cihaz şifresi (opsiyonel)
        /// </summary>
        [StringLength(50)]
        public string? Password { get; set; }

        /// <summary>
        /// Kart numarası (RFID card)
        /// </summary>
        public long? CardNumber { get; set; }

        /// <summary>
        /// Yetki seviyesi
        /// 0=User, 1=Enroller, 2=Manager, 3=SuperAdmin
        /// </summary>
        public int Privilege { get; set; } = 0;

        /// <summary>
        /// Kullanıcı aktif mi?
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Hangi cihaza kayıtlı (opsiyonel FK)
        /// Null ise tüm cihazlarda kayıtlı
        /// </summary>
        public int? DeviceId { get; set; }

        /// <summary>
        /// Navigation property - İlişkili cihaz
        /// </summary>
        [ForeignKey("DeviceId")]
        public virtual ZKTecoDevice? Device { get; set; }

        /// <summary>
        /// Son senkronizasyon zamanı
        /// Cihazdan en son ne zaman çekildi/gönderildi
        /// </summary>
        public DateTime? LastSyncTime { get; set; }

        /// <summary>
        /// Cihaz IP adresi (eğer tek cihaza özgüyse)
        /// </summary>
        [StringLength(50)]
        public string? DeviceIp { get; set; }

        // ========== Audit ==========

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Yetki seviyesi açıklaması
        /// </summary>
        [NotMapped]
        public string PrivilegeText => Privilege switch
        {
            0 => "Kullanıcı",
            1 => "Kayıt Yetkilisi",
            2 => "Yönetici",
            3 => "Süper Admin",
            _ => "Bilinmiyor"
        };
    }
}
