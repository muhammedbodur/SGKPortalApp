using System;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.ZKTeco
{
    /// <summary>
    /// API User DTO - Cihazdan gelen/cihaza gönderilen kullanıcı verisi
    /// Bu DTO ZKTeco cihazlarındaki kullanıcı bilgilerini temsil eder
    /// Personel entity'si ile eşleştirilir (PersonelKayitNo == EnrollNumber)
    /// </summary>
    public class ApiUserDto
    {
        /// <summary>
        /// Kayıt numarası (EnrollNumber) - Personel.PersonelKayitNo ile eşleşir
        /// </summary>
        public string EnrollNumber { get; set; } = string.Empty;

        /// <summary>
        /// Kullanıcı adı
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Şifre (opsiyonel)
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Kart numarası (RFID)
        /// </summary>
        public long? CardNumber { get; set; }

        /// <summary>
        /// Yetki seviyesi (0=Normal User, 2=Administrator, 14=Super Admin)
        /// </summary>
        public int Privilege { get; set; } = 0;

        /// <summary>
        /// Kullanıcı aktif mi?
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Parmak izi sayısı
        /// </summary>
        public int FingerCount { get; set; }

        /// <summary>
        /// Yüz tanıma verisi var mı?
        /// </summary>
        public bool HasFace { get; set; }

        /// <summary>
        /// Cihaz IP adresi (hangi cihazdan geldiği)
        /// </summary>
        public string? DeviceIp { get; set; }

        /// <summary>
        /// Son senkronizasyon zamanı
        /// </summary>
        public DateTime? LastSyncTime { get; set; }
    }
}
