using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base
{
    /// <summary>
    /// Generic Repository Interface - Tüm entity repository'lerin base interface'i
    /// CRUD operasyonları ve ortak sorgu metodlarını içerir
    /// </summary>
    /// <typeparam name="T">Entity tipi - class constraint ile sınırlandırılmış</typeparam>
    public interface IGenericRepository<T> where T : class
    {
        #region Basic CRUD Operations

        /// <summary>
        /// ID'ye göre entity getirir
        /// </summary>
        /// <param name="id">Primary key değeri</param>
        /// <returns>Entity veya null</returns>
        Task<T?> GetByIdAsync(object id);

        /// <summary>
        /// Tüm entity'leri getirir
        /// </summary>
        /// <returns>Entity koleksiyonu</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Tüm entity'leri navigation property'ler ile birlikte getirir
        /// </summary>
        /// <param name="includes">Include edilecek navigation property'ler</param>
        /// <returns>Entity koleksiyonu</returns>
        Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);

        /// <summary>
        /// Yeni entity ekler
        /// </summary>
        /// <param name="entity">Eklenecek entity</param>
        /// <returns>Eklenen entity</returns>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// Birden fazla entity'yi toplu olarak ekler
        /// </summary>
        /// <param name="entities">Eklenecek entity koleksiyonu</param>
        /// <returns>Eklenen entity koleksiyonu</returns>
        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// Entity'yi günceller
        /// </summary>
        /// <param name="entity">Güncellenecek entity</param>
        void Update(T entity);

        /// <summary>
        /// Birden fazla entity'yi toplu olarak günceller
        /// </summary>
        /// <param name="entities">Güncellenecek entity koleksiyonu</param>
        void UpdateRange(IEnumerable<T> entities);

        /// <summary>
        /// Entity'yi siler (hard delete)
        /// </summary>
        /// <param name="entity">Silinecek entity</param>
        void Delete(T entity);

        /// <summary>
        /// Birden fazla entity'yi toplu olarak siler
        /// </summary>
        /// <param name="entities">Silinecek entity koleksiyonu</param>
        void DeleteRange(IEnumerable<T> entities);

        /// <summary>
        /// ID'ye göre entity'yi siler
        /// </summary>
        /// <param name="id">Silinecek entity'nin ID'si</param>
        Task DeleteByIdAsync(object id);

        #endregion

        #region Query Operations

        /// <summary>
        /// Koşula göre entity'leri filtreler
        /// </summary>
        /// <param name="predicate">Filtreleme koşulu</param>
        /// <returns>Filtrelenmiş entity koleksiyonu</returns>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Koşula göre entity'leri navigation property'ler ile birlikte filtreler
        /// </summary>
        /// <param name="predicate">Filtreleme koşulu</param>
        /// <param name="includes">Include edilecek navigation property'ler</param>
        /// <returns>Filtrelenmiş entity koleksiyonu</returns>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);

        /// <summary>
        /// Koşula uyan ilk entity'yi getirir
        /// </summary>
        /// <param name="predicate">Arama koşulu</param>
        /// <returns>İlk eşleşen entity veya null</returns>
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Koşula uyan ilk entity'yi navigation property'ler ile birlikte getirir
        /// </summary>
        /// <param name="predicate">Arama koşulu</param>
        /// <param name="includes">Include edilecek navigation property'ler</param>
        /// <returns>İlk eşleşen entity veya null</returns>
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);

        #endregion

        #region Pagination & Sorting

        /// <summary>
        /// Sayfalama ile entity'leri getirir
        /// </summary>
        /// <typeparam name="TKey">Sıralama key tipi</typeparam>
        /// <param name="pageNumber">Sayfa numarası (1'den başlar)</param>
        /// <param name="pageSize">Sayfa boyutu</param>
        /// <param name="filter">Filtreleme koşulu (opsiyonel)</param>
        /// <param name="orderBy">Sıralama expression'ı (opsiyonel)</param>
        /// <param name="ascending">Artan sıralama (true) veya azalan (false)</param>
        /// <param name="includes">Include edilecek navigation property'ler</param>
        /// <returns>Sayfalanmış data ve toplam kayıt sayısı</returns>
        Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync<TKey>(
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, TKey>>? orderBy = null,
            bool ascending = true,
            params Expression<Func<T, object>>[] includes);

        #endregion

        #region Aggregate Operations

        /// <summary>
        /// Toplam kayıt sayısını getirir
        /// </summary>
        /// <returns>Kayıt sayısı</returns>
        Task<int> CountAsync();

        /// <summary>
        /// Koşula uyan kayıt sayısını getirir
        /// </summary>
        /// <param name="predicate">Sayma koşulu</param>
        /// <returns>Koşula uyan kayıt sayısı</returns>
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Koşula uyan kayıt var mı kontrol eder
        /// </summary>
        /// <param name="predicate">Kontrol koşulu</param>
        /// <returns>Kayıt varsa true, yoksa false</returns>
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);

        #endregion

        #region Raw SQL Support

        /// <summary>
        /// Raw SQL sorgusu çalıştırır
        /// </summary>
        /// <param name="sql">SQL sorgusu</param>
        /// <param name="parameters">SQL parametreleri</param>
        /// <returns>Sorgu sonucu</returns>
        Task<IEnumerable<T>> FromSqlRawAsync(string sql, params object[] parameters);

        #endregion
    }
}