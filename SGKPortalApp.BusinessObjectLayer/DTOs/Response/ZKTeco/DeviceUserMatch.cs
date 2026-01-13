using System.Collections.Generic;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Shared.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.Enums.ZKTeco;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.ZKTeco
{
    /// <summary>
    /// Cihazdaki kullanıcı eşleşme sonucu
    /// </summary>
    public class DeviceUserMatch
    {
        /// <summary>
        /// Cihaz bilgisi
        /// </summary>
        public DeviceResponseDto Device { get; set; } = new DeviceResponseDto();

        /// <summary>
        /// Cihazdaki kullanıcı bilgisi
        /// </summary>
        public ApiUserDto? DeviceUser { get; set; }

        /// <summary>
        /// Personel bilgisi (DB'den)
        /// </summary>
        public PersonelResponseDto? PersonelInfo { get; set; }

        /// <summary>
        /// Eşleşme durumu
        /// </summary>
        public MatchStatus Status { get; set; }

        /// <summary>
        /// Uyuşmazlık detayları
        /// </summary>
        public List<MismatchDetail> Mismatches { get; set; } = new List<MismatchDetail>();

        /// <summary>
        /// Cihazda bulundu mu?
        /// </summary>
        public bool FoundOnDevice => DeviceUser != null;

        /// <summary>
        /// Personel DB'de var mı?
        /// </summary>
        public bool PersonelExists => PersonelInfo != null;
    }
}
