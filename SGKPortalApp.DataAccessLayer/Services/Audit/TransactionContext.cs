using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SGKPortalApp.DataAccessLayer.Services.Audit
{
    /// <summary>
    /// Transaction içindeki birden fazla SaveChanges çağrısını gruplamak için kullanılır.
    /// CreateCompleteAsync ve UpdateCompleteAsync gibi methodlar için.
    /// </summary>
    public class TransactionContext
    {
        private static readonly AsyncLocal<TransactionContextData> _current = new();

        /// <summary>
        /// Aktif transaction context var mı?
        /// </summary>
        public static bool IsActive => _current.Value != null;

        /// <summary>
        /// Transaction ID'yi döner
        /// </summary>
        public static Guid? TransactionId => _current.Value?.TransactionId;

        /// <summary>
        /// Transaction başlat
        /// </summary>
        public static IDisposable BeginTransaction()
        {
            if (_current.Value != null)
            {
                // Nested transaction - aynı context'i kullan
                _current.Value.RefCount++;
                return new TransactionScope(_current.Value);
            }

            var context = new TransactionContextData
            {
                TransactionId = Guid.NewGuid(),
                RefCount = 1
            };

            _current.Value = context;
            return new TransactionScope(context);
        }

        /// <summary>
        /// Transaction içinde bufferlanan log'ları ekler
        /// </summary>
        public static void AddLog(DatabaseLog log)
        {
            _current.Value?.Logs.Add(log);
        }

        /// <summary>
        /// Bufferlanan tüm log'ları döner ve buffer'ı temizler
        /// </summary>
        public static List<DatabaseLog> GetAndClearLogs()
        {
            if (_current.Value == null)
                return new List<DatabaseLog>();

            var logs = _current.Value.Logs;
            _current.Value.Logs = new List<DatabaseLog>();
            return logs;
        }

        /// <summary>
        /// SaveChanges sayacını artırır
        /// </summary>
        public static void IncrementSaveChangesCount()
        {
            if (_current.Value != null)
                _current.Value.SaveChangesCount++;
        }

        /// <summary>
        /// SaveChanges sayısını döner
        /// </summary>
        public static int GetSaveChangesCount()
        {
            return _current.Value?.SaveChangesCount ?? 0;
        }

        private class TransactionContextData
        {
            public Guid TransactionId { get; set; }
            public List<DatabaseLog> Logs { get; set; } = new();
            public int SaveChangesCount { get; set; }
            public int RefCount { get; set; }
        }

        private class TransactionScope : IDisposable
        {
            private readonly TransactionContextData _context;
            private bool _disposed;

            public TransactionScope(TransactionContextData context)
            {
                _context = context;
            }

            public void Dispose()
            {
                if (_disposed)
                    return;

                _disposed = true;
                _context.RefCount--;

                // Son scope bittiğinde context'i temizle
                if (_context.RefCount <= 0)
                {
                    _current.Value = null;
                }
            }
        }
    }
}
