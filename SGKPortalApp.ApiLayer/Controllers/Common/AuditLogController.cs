using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.AuditLog;

namespace SGKPortalApp.ApiLayer.Controllers.Common
{
    /// <summary>
    /// Audit log görüntüleme API controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Sadece login olmuş kullanıcılar erişebilir
    public class AuditLogController : ControllerBase
    {
        private readonly IAuditLogService _auditLogService;
        private readonly ILogger<AuditLogController> _logger;

        public AuditLogController(
            IAuditLogService auditLogService,
            ILogger<AuditLogController> logger)
        {
            _auditLogService = auditLogService;
            _logger = logger;
        }

        /// <summary>
        /// Audit log'ları filtreler ve sayfalanmış döner
        /// </summary>
        [HttpPost("search")]
        public async Task<IActionResult> GetLogs([FromBody] AuditLogFilterDto filter)
        {
            try
            {
                var result = await _auditLogService.GetLogsAsync(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching audit logs");
                return StatusCode(500, new { message = "Audit log'lar alınırken hata oluştu", error = ex.Message });
            }
        }

        /// <summary>
        /// Belirli bir audit log'un detayını döner
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetLogDetail(int id)
        {
            try
            {
                var result = await _auditLogService.GetLogDetailAsync(id);

                if (result == null)
                    return NotFound(new { message = $"Audit log bulunamadı: {id}" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching audit log detail: {LogId}", id);
                return StatusCode(500, new { message = "Audit log detayı alınırken hata oluştu", error = ex.Message });
            }
        }

        /// <summary>
        /// Transaction ID'ye göre gruplu log'ları döner
        /// </summary>
        [HttpGet("transaction/{transactionId:guid}")]
        public async Task<IActionResult> GetTransactionLogs(Guid transactionId)
        {
            try
            {
                var result = await _auditLogService.GetTransactionLogsAsync(transactionId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching transaction logs: {TransactionId}", transactionId);
                return StatusCode(500, new { message = "Transaction log'ları alınırken hata oluştu", error = ex.Message });
            }
        }

        /// <summary>
        /// Kullanıcının son N işlemini döner
        /// </summary>
        [HttpGet("user/{tcKimlikNo}")]
        public async Task<IActionResult> GetUserRecentLogs(string tcKimlikNo, [FromQuery] int count = 50)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tcKimlikNo) || tcKimlikNo.Length != 11)
                    return BadRequest(new { message = "Geçersiz TC Kimlik No" });

                var result = await _auditLogService.GetUserRecentLogsAsync(tcKimlikNo, count);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching user logs: {TcKimlikNo}", tcKimlikNo);
                return StatusCode(500, new { message = "Kullanıcı log'ları alınırken hata oluştu", error = ex.Message });
            }
        }

        /// <summary>
        /// Entity'nin değişiklik geçmişini döner
        /// </summary>
        [HttpGet("entity/{tableName}/{entityId}")]
        public async Task<IActionResult> GetEntityHistory(string tableName, string entityId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tableName) || string.IsNullOrWhiteSpace(entityId))
                    return BadRequest(new { message = "Tablo adı ve entity ID gerekli" });

                var result = await _auditLogService.GetEntityHistoryAsync(tableName, entityId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching entity history: {TableName}/{EntityId}", tableName, entityId);
                return StatusCode(500, new { message = "Entity geçmişi alınırken hata oluştu", error = ex.Message });
            }
        }
    }
}
