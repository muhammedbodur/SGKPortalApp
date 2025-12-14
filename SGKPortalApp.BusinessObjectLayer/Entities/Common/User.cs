using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.BusinessObjectLayer.Entities.Common
{
    /// <summary>
    /// User Tablosu - Personellerin ve TV'lerin dinamik (sık değişen) verilerini tutar
    /// - Şifre, oturum, aktiflik, giriş başarısızlık sayısı gibi işlemsel veriler
    /// - Personel tablosu ile 1-1 ilişkili (TcKimlikNo üzerinden) - Opsiyonel
    /// - Tv tablosu ile 1-1 ilişkili (TvId üzerinden) - Opsiyonel
    /// 
    /// Kullanıcı Tipleri:
    /// - Personel: Normal personel kullanıcısı (Personel ilişkisi var, tüm yetkilere sahip)
    /// - TvUser: TV için oluşturulan kullanıcı (Tv ilişkisi var, sadece TV Display açabilir)
    /// </summary>
    public class User : AuditableEntity
    {
        [Key]
        [StringLength(11)]
        public string TcKimlikNo { get; set; } = string.Empty;

        // Kullanıcı tipi
        [Required]
        public UserType UserType { get; set; } = UserType.Personel;

        // Personel ile One-to-One ilişki (Opsiyonel - Sadece Personel tipinde)
        [ForeignKey(nameof(TcKimlikNo))]
        [InverseProperty("User")]
        public Personel? Personel { get; set; }

        // HubConnection ile One-to-Many ilişki (Bir user'ın birden fazla connection'ı olabilir)
        [InverseProperty("User")]
        public ICollection<HubConnection> HubConnections { get; set; } = new List<HubConnection>();

        // TV ile One-to-One ilişki (Opsiyonel - Sadece TvUser tipinde)
        [InverseProperty("User")]
        public Tv? Tv { get; set; }

        // Güvenlik Bilgileri (Dinamik)
        [Required]
        [StringLength(255)]
        public string PassWord { get; set; } = string.Empty;

        [StringLength(100)]
        public string? SessionID { get; set; }

        // Durum Bilgileri (Dinamik)
        [Required]
        public bool AktifMi { get; set; } = false;

        public DateTime? SonGirisTarihi { get; set; }
        public int BasarisizGirisSayisi { get; set; } = 0;
        public DateTime? HesapKilitTarihi { get; set; }

        // Banko Modu Bilgileri (Dinamik)
        /// <summary>
        /// Kullanıcı şu anda banko modunda mı?
        /// true ise sadece sıra çağırma sayfasını kullanabilir
        /// </summary>
        public bool BankoModuAktif { get; set; } = false;

        /// <summary>
        /// Banko modunda hangi bankoda oturuyor?
        /// null ise banko modunda değil
        /// </summary>
        public int? AktifBankoId { get; set; }

        /// <summary>
        /// Banko moduna geçiş zamanı
        /// </summary>
        public DateTime? BankoModuBaslangic { get; set; }

        public Guid PermissionStamp { get; set; } = Guid.NewGuid();

        // SignalR Bağlantısı (One-to-One)
        public HubConnection? HubConnection { get; set; }

        public User()
        {
            // Varsayılan şifre TC Kimlik No olacak
            PassWord = TcKimlikNo;
        }
    }
}