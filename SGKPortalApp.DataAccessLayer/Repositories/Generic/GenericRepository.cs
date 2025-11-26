using Microsoft.EntityFrameworkCore;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Generic
{
    /// <summary>
    /// Generic Repository Implementation - IGenericRepository interface'ini implement eder
    /// Entity Framework Core kullanarak temel CRUD operasyonlarını gerçekleştirir
    /// </summary>
    /// <typeparam name="T">Entity tipi</typeparam>
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly SGKDbContext _context;
        protected readonly DbSet<T> _dbSet;

        /// <summary>
        /// Constructor - DbContext'i inject eder ve DbSet'i initialize eder
        /// </summary>
        /// <param name="context">SGK Database Context</param>
        public GenericRepository(SGKDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<T>();
        }

        #region Basic CRUD Operations

        /// <summary>
        /// ID'ye göre entity getirir - tracking ile
        /// </summary>
        public virtual async Task<T?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        /// <summary>
        /// Tüm entity'leri getirir - tracking olmadan (AsNoTracking)
        /// </summary>
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Navigation property'ler ile tüm entity'leri getirir
        /// </summary>
        public virtual async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet.AsNoTracking();

            // Include işlemlerini uygula
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.ToListAsync();
        }

        /// <summary>
        /// Yeni entity ekler ve eklenen entity'yi geri döner
        /// </summary>
        public virtual async Task<T> AddAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var addedEntity = await _dbSet.AddAsync(entity);
            return addedEntity.Entity;
        }

        /// <summary>
        /// Birden fazla entity'yi toplu olarak ekler
        /// </summary>
        public virtual async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
        {
            if (entities == null || !entities.Any())
                throw new ArgumentException("Entities cannot be null or empty", nameof(entities));

            await _dbSet.AddRangeAsync(entities);
            return entities;
        }

        /// <summary>
        /// Entity'yi günceller - change tracking kullanır
        /// </summary>
        public virtual void Update(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            // Entity zaten track ediliyor mu kontrol et
            var entry = _context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }

            entry.State = EntityState.Modified;
        }

        /// <summary>
        /// Birden fazla entity'yi toplu günceller
        /// </summary>
        public virtual void UpdateRange(IEnumerable<T> entities)
        {
            if (entities == null || !entities.Any())
                throw new ArgumentException("Entities cannot be null or empty", nameof(entities));

            _dbSet.UpdateRange(entities);
        }

        /// <summary>
        /// Entity'yi siler (AuditableEntity ise soft delete, değilse hard delete)
        /// </summary>
        public virtual void Delete(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            // Entity detached ise önce attach et
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }

            // ⭐ SOFT DELETE: AuditableEntity ise SilindiMi = true yap
            if (entity is AuditableEntity auditableEntity)
            {
                auditableEntity.SilindiMi = true;
                auditableEntity.SilinmeTarihi = DateTime.Now;
                // TODO: HttpContextAccessor ile kullanıcı bilgisi alınabilir
                // auditableEntity.SilenKullanici = _currentUser?.TcKimlikNo;

                _context.Entry(entity).State = EntityState.Modified;
            }
            else
            {
                // Normal entity ise hard delete
                _dbSet.Remove(entity);
            }
        }

        /// <summary>
        /// Birden fazla entity'yi toplu siler (AuditableEntity ise soft delete, değilse hard delete)
        /// </summary>
        public virtual void DeleteRange(IEnumerable<T> entities)
        {
            if (entities == null || !entities.Any())
                throw new ArgumentException("Entities cannot be null or empty", nameof(entities));

            // ⭐ SOFT DELETE: Her entity için Delete metodunu çağır
            foreach (var entity in entities)
            {
                Delete(entity);
            }
        }

        /// <summary>
        /// ID'ye göre entity bulup siler
        /// </summary>
        public virtual async Task DeleteByIdAsync(object id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                Delete(entity);
            }
        }

        #endregion

        #region Query Operations

        /// <summary>
        /// Expression-based filtreleme
        /// </summary>
        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return await _dbSet.AsNoTracking().Where(predicate).ToListAsync();
        }

        /// <summary>
        /// Navigation property'ler ile filtreleme
        /// </summary>
        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            IQueryable<T> query = _dbSet.AsNoTracking();

            // Include işlemlerini uygula
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.Where(predicate).ToListAsync();
        }

        /// <summary>
        /// İlk eşleşen entity'yi getirir
        /// </summary>
        public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(predicate);
        }

        /// <summary>
        /// Navigation property'ler ile ilk eşleşen entity'yi getirir
        /// </summary>
        public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            IQueryable<T> query = _dbSet.AsNoTracking();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync(predicate);
        }

        #endregion

        #region Pagination & Sorting

        /// <summary>
        /// Sayfalama ile entity'leri getirir - performance optimized
        /// </summary>
        public virtual async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync<TKey>(
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, TKey>>? orderBy = null,
            bool ascending = true,
            params Expression<Func<T, object>>[] includes)
        {
            if (pageNumber < 1)
                throw new ArgumentException("Page number must be greater than 0", nameof(pageNumber));

            if (pageSize < 1)
                throw new ArgumentException("Page size must be greater than 0", nameof(pageSize));

            IQueryable<T> query = _dbSet.AsNoTracking();

            // Include işlemlerini uygula
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            // Filtreleme uygula
            if (filter != null)
            {
                query = query.Where(filter);
            }

            // Toplam kayıt sayısını al (filtrelemeden sonra)
            var totalCount = await query.CountAsync();

            // Sıralama uygula
            if (orderBy != null)
            {
                query = ascending ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);
            }

            // Sayfalama uygula
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        #endregion

        #region Aggregate Operations

        /// <summary>
        /// Toplam kayıt sayısını getirir
        /// </summary>
        public virtual async Task<int> CountAsync()
        {
            return await _dbSet.CountAsync();
        }

        /// <summary>
        /// Koşula uyan kayıt sayısını getirir
        /// </summary>
        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return await _dbSet.CountAsync(predicate);
        }

        /// <summary>
        /// Koşula uyan kayıt var mı kontrol eder
        /// </summary>
        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return await _dbSet.AnyAsync(predicate);
        }

        #endregion

        #region Raw SQL Support

        /// <summary>
        /// Raw SQL sorgusu çalıştırır - güvenlik için parameterized
        /// </summary>
        public virtual async Task<IEnumerable<T>> FromSqlRawAsync(string sql, params object[] parameters)
        {
            if (string.IsNullOrWhiteSpace(sql))
                throw new ArgumentException("SQL cannot be null or empty", nameof(sql));

            return await _dbSet.FromSqlRaw(sql, parameters).AsNoTracking().ToListAsync();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Query'e include işlemlerini uygular - helper method
        /// </summary>
        protected virtual IQueryable<T> ApplyIncludes(IQueryable<T> query, params Expression<Func<T, object>>[] includes)
        {
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            return query;
        }

        #endregion
    }
}