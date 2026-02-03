using System.Collections.Generic;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri
{
    /// <summary>
    /// Kart senkronizasyon sonucu DTO
    /// Özel kartların cihazlara gönderilmesi/silinmesi işlemlerinin sonucunu tutar
    /// </summary>
    public class CardSyncResultDto
    {
        public int TotalDevices { get; set; }
        public int SuccessCount { get; set; }
        public int FailCount { get; set; }
        public List<DeviceSyncDetail> Details { get; set; } = new();
        
        public bool IsSuccess => FailCount == 0 && SuccessCount > 0;
        public string Message => $"{SuccessCount}/{TotalDevices} cihaza başarıyla gönderildi";
    }
}
