using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Data.SqlClient;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using SGKPortalApp.DataAccessLayer.Repositories.Generic;
using SGKPortalApp.BusinessObjectLayer.Exceptions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories
{
    /// <summary>
    /// Smart UnitOfWork Implementation - N-Layer Architecture
    /// Convention-based repository discovery ile otomatik repository management
    /// Thread-safe ve performance optimized
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SGKDbContext _context;
        private readonly IServiceProvider _serviceProvider;

        // Thread-safe repository cache
        private readonly ConcurrentDictionary<Type, object> _repositoryCache = new();
        private readonly ConcurrentDictionary<Type, Type> _interfaceToImplementationMap = new();

        private IDbContextTransaction? _currentTransaction;
        private bool _disposed = false;

        /// <summary>
        /// Constructor - Dependency Injection ile initialize
        /// </summary>
        public UnitOfWork(SGKDbContext context, IServiceProvider serviceProvider)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            // Repository mapping'leri initialize et
            InitializeRepositoryMappings();
        }

        #region Smart Repository Access

        /// <summary>
        /// Smart repository accessor - Convention-based discovery
        /// </summary>
        public IGenericRepository<T> Repository<T>() where T : class
        {
            var entityType = typeof(T);

            // Cache'den kontrol et
            if (_repositoryCache.TryGetValue(entityType, out var cachedRepo))
                return (IGenericRepository<T>)cachedRepo;

            // Specific repository interface'ini bul
            var specificRepoInterface = FindSpecificRepositoryInterface<T>();

            if (specificRepoInterface != null)
            {
                // DI container'dan specific repository'yi al
                var specificRepo = _serviceProvider.GetService(specificRepoInterface) as IGenericRepository<T>;
                if (specificRepo != null)
                {
                    _repositoryCache.TryAdd(entityType, specificRepo);
                    return specificRepo;
                }
            }

            // Specific repository yoksa Generic repository oluştur
            var genericRepo = new GenericRepository<T>(_context);
            _repositoryCache.TryAdd(entityType, genericRepo);
            return genericRepo;
        }

        /// <summary>
        /// Specific repository accessor - Type-safe
        /// </summary>
        public TInterface GetRepository<TInterface>() where TInterface : class
        {
            var interfaceType = typeof(TInterface);

            // Cache'den kontrol et
            if (_repositoryCache.TryGetValue(interfaceType, out var cachedRepo))
                return (TInterface)cachedRepo;

            // DI container'dan al
            var repository = _serviceProvider.GetService<TInterface>();
            if (repository != null)
            {
                _repositoryCache.TryAdd(interfaceType, repository);
                return repository;
            }

            throw new InvalidOperationException(
                $"Repository interface '{typeof(TInterface).Name}' DI container'da kayıtlı değil. " +
                $"Lütfen Program.cs dosyasında kaydedildiğinden emin olun.");
        }

        #endregion

        #region Transaction Management

        public async Task<int> SaveChangesAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new InvalidOperationException(
                    "Eş zamanlılık çakışması oluştu. Kayıt başka bir kullanıcı tarafından değiştirilmiş olabilir.",
                    ex);
            }
            catch (DbUpdateException ex)
            {
                // SQL Exception'ı parse et
                if (ex.InnerException is SqlException sqlException)
                {
                    var constraintName = ExtractConstraintName(ex.Message);
                    
                    switch (sqlException.Number)
                    {
                        case 547: // Foreign Key Violation or Check Constraint
                            // Check constraint mesajında "CHECK" kelimesi var
                            var errorType = ex.Message.Contains("CHECK", StringComparison.OrdinalIgnoreCase)
                                ? DatabaseErrorType.CheckConstraintViolation
                                : DatabaseErrorType.ForeignKeyViolation;
                            
                            throw new DatabaseException(
                                ex.Message, 
                                errorType, 
                                constraintName);
                        
                        case 2627: // Unique Constraint Violation
                        case 2601: // Duplicate Key
                            throw new DatabaseException(
                                ex.Message, 
                                DatabaseErrorType.UniqueConstraintViolation, 
                                constraintName);
                        
                        case 515: // Null Constraint Violation
                            throw new DatabaseException(
                                ex.Message, 
                                DatabaseErrorType.NullConstraintViolation, 
                                constraintName);
                        
                        default:
                            throw new DatabaseException(
                                ex.Message, 
                                DatabaseErrorType.Unknown, 
                                constraintName);
                    }
                }
                
                throw new InvalidOperationException(
                    "Veritabanına kayıt sırasında bir hata oluştu. Detaylar için inner exception'a bakınız.",
                    ex);
            }
        }

        private string? ExtractConstraintName(string errorMessage)
        {
            // "FK_PER_Personeller_CMN_HizmetBinalari" gibi constraint adını çıkar
            var match = System.Text.RegularExpressions.Regex.Match(
                errorMessage, 
                @"constraint\s+""([^""]+)""", 
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            
            return match.Success ? match.Groups[1].Value : null;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_currentTransaction != null)
                throw new InvalidOperationException("Bir transaction zaten başlatılmış durumda.");

            _currentTransaction = await _context.Database.BeginTransactionAsync();
            return _currentTransaction;
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            if (transaction != _currentTransaction)
                throw new InvalidOperationException("Verilen transaction mevcut transaction ile aynı değil.");

            try
            {
                await transaction.CommitAsync();
            }
            catch
            {
                await RollbackTransactionAsync(transaction);
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.DisposeAsync();
                    _currentTransaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync(IDbContextTransaction transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            try
            {
                // Transaction zaten commit/rollback edilmiş veya dispose edilmişse tekrar rollback yapma
                if (_currentTransaction != null && transaction == _currentTransaction)
                {
                    await transaction.RollbackAsync();
                }
            }
            catch (InvalidOperationException)
            {
                // Transaction zaten tamamlanmış (commit/rollback edilmiş), bu durumu yoksay
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.DisposeAsync();
                    _currentTransaction = null;
                }
            }
        }

        public async Task ExecuteInTransactionAsync(Func<Task> operations)
        {
            if (operations == null)
                throw new ArgumentNullException(nameof(operations));

            // Nested transaction kontrolü
            if (_currentTransaction != null)
            {
                await operations();
                return;
            }

            using var transaction = await BeginTransactionAsync();
            try
            {
                await operations();
                await CommitTransactionAsync(transaction);
            }
            catch
            {
                await RollbackTransactionAsync(transaction);
                throw;
            }
        }

        public async Task<TResult> ExecuteInTransactionAsync<TResult>(Func<Task<TResult>> operations)
        {
            if (operations == null)
                throw new ArgumentNullException(nameof(operations));

            // Nested transaction kontrolü
            if (_currentTransaction != null)
                return await operations();

            using var transaction = await BeginTransactionAsync();
            try
            {
                var result = await operations();
                await CommitTransactionAsync(transaction);
                return result;
            }
            catch
            {
                await RollbackTransactionAsync(transaction);
                throw;
            }
        }

        #endregion

        #region Bulk Operations

        public async Task BulkInsertAsync<T>(IEnumerable<T> entities) where T : class
        {
            if (entities == null || !entities.Any())
                return;

            try
            {
                await _context.Set<T>().AddRangeAsync(entities);
                await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"{typeof(T).Name} entity tipi için toplu ekleme işlemi başarısız oldu.", ex);
            }
        }

        public async Task BulkUpdateAsync<T>(IEnumerable<T> entities) where T : class
        {
            if (entities == null || !entities.Any())
                return;

            try
            {
                _context.Set<T>().UpdateRange(entities);
                await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"{typeof(T).Name} entity tipi için toplu güncelleme işlemi başarısız oldu.", ex);
            }
        }

        public async Task BulkDeleteAsync<T>(IEnumerable<T> entities) where T : class
        {
            if (entities == null || !entities.Any())
                return;

            try
            {
                _context.Set<T>().RemoveRange(entities);
                await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"{typeof(T).Name} entity tipi için toplu silme işlemi başarısız oldu.", ex);
            }
        }

        #endregion

        #region Context Management

        public void ResetContext()
        {
            _context.ChangeTracker.Clear();
            // Repository cache'i temizleme - context reset edildiğinde repository'ler de yeniden oluşturulmalı
            _repositoryCache.Clear();
        }

        public bool IsTracked<T>(T entity) where T : class
        {
            return _context.Entry(entity).State != EntityState.Detached;
        }

        public void DetachEntity<T>(T entity) where T : class
        {
            _context.Entry(entity).State = EntityState.Detached;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Repository mapping'lerini initialize eder - Startup'ta çağrılır
        /// </summary>
        private void InitializeRepositoryMappings()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();

                // Tüm repository interface'lerini bul
                var repositoryInterfaces = assembly.GetTypes()
                    .Where(t => t.IsInterface &&
                               t.Name.EndsWith("Repository") &&
                               t != typeof(IGenericRepository<>) &&
                               t != typeof(IUnitOfWork))
                    .ToList();

                // Her interface için implementation'ı bul ve map'le
                foreach (var interfaceType in repositoryInterfaces)
                {
                    var implementationType = assembly.GetTypes()
                        .FirstOrDefault(t => !t.IsInterface &&
                                           !t.IsAbstract &&
                                           interfaceType.IsAssignableFrom(t));

                    if (implementationType != null)
                    {
                        _interfaceToImplementationMap.TryAdd(interfaceType, implementationType);
                    }
                }
            }
            catch (Exception ex)
            {
                // Logging yapılabilir
                throw new InvalidOperationException("Repository mapping'lerini başlatma işlemi başarısız oldu.", ex);
            }
        }

        /// <summary>
        /// Entity için specific repository interface'ini bulur
        /// Convention: Personel -> IPersonelRepository
        /// </summary>
        private Type? FindSpecificRepositoryInterface<T>() where T : class
        {
            var entityName = typeof(T).Name;
            var expectedInterfaceName = $"I{entityName}Repository";

            return _interfaceToImplementationMap.Keys
                .FirstOrDefault(t => t.Name == expectedInterfaceName);
        }

        #endregion

        #region Dispose Pattern

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                // Transaction cleanup
                if (_currentTransaction != null)
                {
                    _currentTransaction.Rollback();
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }

                // Cache cleanup
                _repositoryCache.Clear();
                _interfaceToImplementationMap.Clear();

                // Context cleanup
                _context?.Dispose();

                _disposed = true;
            }
        }

        ~UnitOfWork()
        {
            Dispose(false);
        }

        #endregion
    }
}