using Microsoft.EntityFrameworkCore.Storage;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using System;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces
{
    /// <summary>
    /// Unit of Work Pattern Interface - N-Layer Architecture için optimize edilmiş
    /// Convention-based repository discovery ile otomatik repository management
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        #region Smart Repository Access

        /// <summary>
        /// Smart repository accessor - Otomatik olarak specific veya generic repository döner
        /// Convention: Entity -> IEntityRepository varsa onu kullanır, yoksa IGenericRepository<Entity>
        /// </summary>
        /// <typeparam name="T">Entity tipi</typeparam>
        /// <returns>Specific repository (varsa) veya Generic repository</returns>
        IGenericRepository<T> Repository<T>() where T : class;

        /// <summary>
        /// Specific repository accessor - Type-safe specific repository access
        /// </summary>
        /// <typeparam name="TInterface">Repository interface tipi</typeparam>
        /// <returns>Specific repository instance</returns>
        TInterface GetRepository<TInterface>() where TInterface : class;

        #endregion

        #region Transaction Management

        /// <summary>
        /// Tüm değişiklikleri veritabanına kaydeder
        /// </summary>
        /// <returns>Etkilenen kayıt sayısı</returns>
        Task<int> SaveChangesAsync();

        /// <summary>
        /// Transaction başlatır
        /// </summary>
        /// <returns>Database transaction</returns>
        Task<IDbContextTransaction> BeginTransactionAsync();

        /// <summary>
        /// Transaction'ı commit eder
        /// </summary>
        Task CommitTransactionAsync(IDbContextTransaction transaction);

        /// <summary>
        /// Transaction'ı rollback eder
        /// </summary>
        Task RollbackTransactionAsync(IDbContextTransaction transaction);

        /// <summary>
        /// Otomatik transaction yönetimi ile işlem gerçekleştirir
        /// </summary>
        /// <param name="operations">Transaction içinde çalıştırılacak işlemler</param>
        Task ExecuteInTransactionAsync(Func<Task> operations);

        /// <summary>
        /// Otomatik transaction yönetimi ile işlem gerçekleştirir (return value ile)
        /// </summary>
        /// <typeparam name="TResult">Return tipi</typeparam>
        /// <param name="operations">Transaction içinde çalıştırılacak işlemler</param>
        /// <returns>İşlem sonucu</returns>
        Task<TResult> ExecuteInTransactionAsync<TResult>(Func<Task<TResult>> operations);

        #endregion

        #region Bulk Operations

        /// <summary>
        /// Toplu kayıt ekleme - Performance optimized
        /// </summary>
        Task BulkInsertAsync<T>(IEnumerable<T> entities) where T : class;

        /// <summary>
        /// Toplu kayıt güncelleme
        /// </summary>
        Task BulkUpdateAsync<T>(IEnumerable<T> entities) where T : class;

        /// <summary>
        /// Toplu kayıt silme
        /// </summary>
        Task BulkDeleteAsync<T>(IEnumerable<T> entities) where T : class;

        #endregion

        #region Context Management

        /// <summary>
        /// DbContext'i reset eder - Change tracking temizler
        /// </summary>
        void ResetContext();

        /// <summary>
        /// Entity'nin tracking durumunu kontrol eder
        /// </summary>
        bool IsTracked<T>(T entity) where T : class;

        /// <summary>
        /// Entity'yi detach eder
        /// </summary>
        void DetachEntity<T>(T entity) where T : class;

        #endregion

        #region Raw SQL Operations

        /// <summary>
        /// Raw SQL komutu çalıştırır (IDENTITY_INSERT gibi özel durumlar için)
        /// </summary>
        Task<int> ExecuteSqlRawAsync(string sql, CancellationToken ct = default);

        #endregion
    }
}