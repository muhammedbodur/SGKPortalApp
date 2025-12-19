using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Complex;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.Complex
{
    /// <summary>
    /// Yetki (Permission) modülü için kompleks sorguları barındıran repository
    /// Recursive CTE ve diğer karmaşık SQL sorgularını içerir
    /// </summary>
    public class YetkiQueryRepository : IYetkiQueryRepository
    {
        private readonly SGKDbContext _context;

        public YetkiQueryRepository(SGKDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Belirtilen controller için tam hiyerarşik yolu (FullPath) döndürür
        /// Recursive CTE kullanarak root'tan leaf'e kadar tüm hiyerarşiyi oluşturur
        /// </summary>
        public async Task<List<(int ModulControllerId, string ModulControllerAdi, int Level, string FullPath)>> GetControllerHierarchyPathAsync(int modulControllerId)
        {
            var sql = @"
            DECLARE @LeafId INT = @controllerId;

            ;WITH RootFinder AS
            (
                SELECT
                    mc.ModulControllerId,
                    mc.ModulControllerAdi,
                    mc.UstModulControllerId
                FROM PER_ModulControllers mc
                WHERE mc.ModulControllerId = @LeafId
                AND mc.SilindiMi = 0

                UNION ALL

                SELECT
                    parent.ModulControllerId,
                    parent.ModulControllerAdi,
                    parent.UstModulControllerId
                FROM PER_ModulControllers parent
                INNER JOIN RootFinder child
                    ON child.UstModulControllerId = parent.ModulControllerId
                WHERE parent.SilindiMi = 0
            ),
            Tree AS
            (
                SELECT
                    rf.ModulControllerId,
                    rf.ModulControllerAdi,
                    rf.UstModulControllerId,
                    CAST('/' + rf.ModulControllerAdi AS NVARCHAR(MAX)) AS FullPath,
                    0 AS Level
                FROM RootFinder rf
                WHERE rf.UstModulControllerId IS NULL

                UNION ALL

                SELECT
                    c.ModulControllerId,
                    c.ModulControllerAdi,
                    c.UstModulControllerId,
                    CAST(p.FullPath + '/' + c.ModulControllerAdi AS NVARCHAR(MAX)) AS FullPath,
                    p.Level + 1
                FROM PER_ModulControllers c
                INNER JOIN Tree p
                    ON c.UstModulControllerId = p.ModulControllerId
                WHERE c.SilindiMi = 0
            )
            SELECT
                ModulControllerId,
                ModulControllerAdi,
                Level,
                FullPath
            FROM Tree
            ORDER BY Level
            OPTION (MAXRECURSION 100);";

            var result = new List<(int, string, int, string)>();

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                command.Parameters.Add(new SqlParameter("@controllerId", modulControllerId));

                await _context.Database.OpenConnectionAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        result.Add((
                            reader.GetInt32(0),  // ModulControllerId
                            reader.GetString(1), // ModulControllerAdi
                            reader.GetInt32(2),  // Level
                            reader.GetString(3)  // FullPath
                        ));
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Belirtilen modüle ait tüm controller'ların hiyerarşik yollarını döndürür
        /// Her controller için recursive CTE çalıştırarak FullPath oluşturur
        /// </summary>
        public async Task<List<(int ModulControllerId, string ModulControllerAdi, int Level, string FullPath)>> GetModulControllersWithHierarchyAsync(int modulId)
        {
            var sql = @"
            DECLARE @ModulId INT = @modulIdParam;

            ;WITH AllControllers AS
            (
                SELECT ModulControllerId
                FROM PER_ModulControllers
                WHERE ModulId = @ModulId
                AND SilindiMi = 0
            ),
            RootFinder AS
            (
                SELECT
                    mc.ModulControllerId,
                    mc.ModulControllerAdi,
                    mc.UstModulControllerId,
                    mc.ModulControllerId AS OriginalId
                FROM PER_ModulControllers mc
                INNER JOIN AllControllers ac ON mc.ModulControllerId = ac.ModulControllerId
                WHERE mc.SilindiMi = 0

                UNION ALL

                SELECT
                    parent.ModulControllerId,
                    parent.ModulControllerAdi,
                    parent.UstModulControllerId,
                    child.OriginalId
                FROM PER_ModulControllers parent
                INNER JOIN RootFinder child
                    ON child.UstModulControllerId = parent.ModulControllerId
                WHERE parent.SilindiMi = 0
            ),
            Tree AS
            (
                -- ROOT (üstü olmayan)
                SELECT
                    rf.ModulControllerId,
                    rf.ModulControllerAdi,
                    rf.UstModulControllerId,
                    rf.OriginalId,
                    CAST('/' + rf.ModulControllerAdi AS NVARCHAR(MAX)) AS FullPath,
                    0 AS Level
                FROM RootFinder rf
                WHERE rf.UstModulControllerId IS NULL

                UNION ALL

                SELECT
                    c.ModulControllerId,
                    c.ModulControllerAdi,
                    c.UstModulControllerId,
                    p.OriginalId,
                    CAST(p.FullPath + '/' + c.ModulControllerAdi AS NVARCHAR(MAX)) AS FullPath,
                    p.Level + 1
                FROM PER_ModulControllers c
                INNER JOIN Tree p
                    ON c.UstModulControllerId = p.ModulControllerId
                WHERE c.SilindiMi = 0
            )
            SELECT DISTINCT
                t.ModulControllerId,
                t.ModulControllerAdi,
                t.Level,
                t.FullPath
            FROM Tree t
            INNER JOIN AllControllers ac ON t.ModulControllerId = ac.ModulControllerId
            ORDER BY t.FullPath
            OPTION (MAXRECURSION 100);";

            var result = new List<(int, string, int, string)>();

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                command.Parameters.Add(new SqlParameter("@modulIdParam", modulId));

                await _context.Database.OpenConnectionAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        result.Add((
                            reader.GetInt32(0),  // ModulControllerId
                            reader.GetString(1), // ModulControllerAdi
                            reader.GetInt32(2),  // Level
                            reader.GetString(3)  // FullPath
                        ));
                    }
                }
            }

            return result;
        }
    }
}
