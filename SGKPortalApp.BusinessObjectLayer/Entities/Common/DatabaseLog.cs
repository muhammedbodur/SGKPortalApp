using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGKPortalApp.BusinessObjectLayer.Entities.Common
{
    public class DatabaseLog : BaseEntity
    {
        public int DatabaseLogId { get; set; }
        public required string TcKimlikNo { get; set; }
        public DatabaseAction DatabaseAction { get; set; } // INSERT, UPDATE, DELETE
        public required string TableName { get; set; }
        public DateTime ActionTime { get; set; }
        public string BeforeData { get; set; } = string.Empty; // Değişiklik öncesi veri JSON formatında
        public string AfterData { get; set; } = string.Empty;  // Değişiklik sonrası veri JSON formatında
        public DateTime IslemZamani { get; set; } = DateTime.Now; // Varsayılan olarak işlem zamanı
    }
}
