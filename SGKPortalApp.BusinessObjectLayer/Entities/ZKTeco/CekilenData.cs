using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco
{
    /// <summary>
    /// ZKTeco cihazından çekilen ham attendance data
    /// PDKS.Net4.8 tbCekilenData ile uyumlu
    /// Tablo adı: ZKTeco_CekilenData (Configuration'da tanımlı)
    /// </summary>
    public class CekilenData : BaseEntity
    {
        [Key]
        public int CekilenDataId { get; set; }

        /// <summary>
        /// Personel kayıt numarası (ZKTeco EnrollNumber)
        /// PDKS.Net4.8: kayitno
        /// </summary>
        [StringLength(50)]
        public string? KayitNo { get; set; }

        /// <summary>
        /// Doğrulama yöntemi (0=Password, 15=Card)
        /// PDKS.Net4.8: dogrulama
        /// </summary>
        [StringLength(50)]
        public string? Dogrulama { get; set; }

        /// <summary>
        /// Giriş/Çıkış modu (0=In, 1=Out, 2=Break-Out, 3=Break-In, 4=OT-In, 5=OT-Out)
        /// PDKS.Net4.8: giriscikismodu
        /// </summary>
        [StringLength(50)]
        public string? GirisCikisModu { get; set; }

        /// <summary>
        /// Attendance tarihi
        /// PDKS.Net4.8: tarih
        /// </summary>
        public DateTime? Tarih { get; set; }

        /// <summary>
        /// İş kodu
        /// PDKS.Net4.8: workCode
        /// </summary>
        [StringLength(50)]
        public string? WorkCode { get; set; }

        /// <summary>
        /// Reserved field (ZKTeco SDK)
        /// PDKS.Net4.8: reserved
        /// </summary>
        [StringLength(50)]
        public string? Reserved { get; set; }

        /// <summary>
        /// Cihaz ID (Device foreign key)
        /// </summary>
        public int DeviceId { get; set; }

        /// <summary>
        /// İlişkili cihaz
        /// </summary>
        [ForeignKey(nameof(DeviceId))]
        [InverseProperty("CekilenData")]
        public Device? Device { get; set; }

        // ========== Ek Modern Field'lar (PDKS.Net4.8'de yok) ==========

        /// <summary>
        /// Cihaz IP adresi (lookup için)
        /// </summary>
        [StringLength(50)]
        public string? CihazIp { get; set; }

        /// <summary>
        /// Cihazdan çekilme zamanı
        /// </summary>
        public DateTime CekilmeTarihi { get; set; } = DateTime.Now;

        /// <summary>
        /// İşlenmiş mi? (Personel ile eşleştirilmiş mi?)
        /// </summary>
        public bool IsProcessed { get; set; } = false;

        /// <summary>
        /// İşlenme zamanı
        /// </summary>
        public DateTime? ProcessedAt { get; set; }

        /// <summary>
        /// Ham data (JSON - debugging için)
        /// </summary>
        public string? RawData { get; set; }
    }
}
