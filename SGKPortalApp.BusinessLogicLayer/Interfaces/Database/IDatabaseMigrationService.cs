namespace SGKPortalApp.BusinessLogicLayer.Interfaces.Database
{
    /// <summary>
    /// Database Migration yönetimi için servis interface
    /// Production deployment sırasında migration'ları otomatik uygular
    /// </summary>
    public interface IDatabaseMigrationService
    {
        /// <summary>
        /// Bekleyen migration'ları uygular
        /// </summary>
        Task ApplyMigrationsAsync();
    }
}
