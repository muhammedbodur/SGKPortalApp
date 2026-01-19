using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SGKPortalApp.BusinessObjectLayer.Enums.SignalR;

namespace SGKPortalApp.BusinessObjectLayer.Entities.SignalR
{
    /// <summary>
    /// SignalR Event Log Entity
    /// Gönderilen tüm SignalR mesajlarının kaydını tutar
    /// Tablo adı: SIG_EventLogs (Configuration'da tanımlanacak)
    /// 
    /// Amaç:
    /// - Kime mesaj gönderildi
    /// - Ne gönderildi
    /// - Ne amaçla gönderildi
    /// - Ulaştı mı (Faz 2 için ACK desteği)
    /// </summary>
    public class SignalREventLog
    {
        [Key]
        public long EventLogId { get; set; }

        /// <summary>
        /// Benzersiz event kimliği (client ACK için kullanılacak)
        /// </summary>
        [Required]
        public Guid EventId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Event tipi (SiraCalled, SiraRedirected, TvUpdate vb.)
        /// </summary>
        [Required]
        public SignalREventType EventType { get; set; }

        /// <summary>
        /// SignalR event adı (siraListUpdate, TvSiraGuncellendi vb.)
        /// </summary>
        [Required]
        [StringLength(100)]
        public string EventName { get; set; } = string.Empty;

        /// <summary>
        /// Hedef tipi (Connection, Group, Personel, Tv, All)
        /// </summary>
        [Required]
        public SignalRTargetType TargetType { get; set; }

        /// <summary>
        /// Hedef kimliği (ConnectionId, GroupName, PersonelTc, TvId)
        /// Birden fazla hedef varsa virgülle ayrılmış
        /// </summary>
        [StringLength(500)]
        public string? TargetId { get; set; }

        /// <summary>
        /// Hedef sayısı (kaç connection'a gönderildi)
        /// </summary>
        public int TargetCount { get; set; }

        /// <summary>
        /// İlgili Sıra ID (varsa)
        /// </summary>
        public int? SiraId { get; set; }

        /// <summary>
        /// İlgili Sıra No (varsa)
        /// </summary>
        public int? SiraNo { get; set; }

        /// <summary>
        /// İlgili Banko ID (varsa)
        /// </summary>
        public int? BankoId { get; set; }

        /// <summary>
        /// İlgili TV ID (varsa)
        /// </summary>
        public int? TvId { get; set; }

        /// <summary>
        /// İlgili Personel TC (varsa)
        /// </summary>
        [StringLength(11)]
        public string? PersonelTc { get; set; }

        /// <summary>
        /// Payload özeti (JSON - max 2000 karakter)
        /// Tam payload değil, önemli alanların özeti
        /// </summary>
        [StringLength(2000)]
        public string? PayloadSummary { get; set; }

        /// <summary>
        /// Gönderim durumu
        /// </summary>
        [Required]
        public SignalRDeliveryStatus DeliveryStatus { get; set; } = SignalRDeliveryStatus.Pending;

        /// <summary>
        /// Hata mesajı (başarısız ise)
        /// </summary>
        [StringLength(500)]
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Gönderim zamanı
        /// </summary>
        [Required]
        public DateTime SentAt { get; set; } = DateTime.Now;

        /// <summary>
        /// ACK alınma zamanı (Faz 2 için)
        /// </summary>
        public DateTime? AcknowledgedAt { get; set; }

        /// <summary>
        /// İşlem süresi (ms)
        /// </summary>
        public int? DurationMs { get; set; }

        /// <summary>
        /// Hizmet Binası ID (filtreleme için)
        /// </summary>
        public int? HizmetBinasiId { get; set; }

        /// <summary>
        /// Correlation ID (ilişkili eventleri gruplamak için)
        /// Örn: Bir sıra çağrıldığında hem panel hem TV güncellemesi yapılır
        /// </summary>
        public Guid? CorrelationId { get; set; }
    }
}
