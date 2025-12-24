namespace SGKPortalApp.Common.Interfaces.Permission
{
    /// <summary>
    /// HTTP isteğinin route bilgisinden permission key'i otomatik çözümleyen servis.
    /// Frontend'deki PermissionStateService.GetPermissionKeyByRoute() ile aynı mantık.
    ///
    /// Kullanım:
    /// - Service'lerde field validation yaparken otomatik permission key çözümlemesi
    /// - Frontend ile backend tutarlılığı sağlar (tek mapping source)
    ///
    /// NOT: Cache PermissionStateService tarafından yüklenir. Bu servis sadece lookup yapar.
    /// </summary>
    public interface IPermissionKeyResolverService
    {
        /// <summary>
        /// Mevcut HTTP request'in route'undan permission key'i çözümler (SYNC - cache lookup).
        /// Backend service'lerde kullanım için - HttpContext'ten route'u otomatik alır.
        ///
        /// Örnek (Backend):
        /// - HttpContext.Request.Path: /api/personel/unvan → Permission Key: "PERSONEL.UNVAN.INDEX"
        /// </summary>
        /// <returns>Permission key veya null</returns>
        string? ResolveFromCurrentRequest();

        /// <summary>
        /// Belirtilen route'tan permission key'i çözümler (SYNC - cache lookup).
        /// Frontend'deki PermissionStateService.GetPermissionKeyByRoute() ile aynı mantık.
        ///
        /// Örnek:
        /// - Route: /personel/unvan → Permission Key: "PERSONEL.UNVAN.INDEX"
        /// - Route: /personel/unvan/manage → Permission Key: "PERSONEL.UNVAN.MANAGE"
        /// - Route: /siramatik/banko → Permission Key: "SIRAMATIK.BANKO.INDEX"
        ///
        /// NOT: ModulControllerIslem tablosundaki Route → PermissionKey mapping kullanılır.
        /// Cache yoksa null döner (PermissionStateService.EnsureLoadedAsync() cache'i yükler).
        /// </summary>
        /// <param name="route">Route path (örn: "/personel/unvan")</param>
        /// <returns>Permission key (örn: "PERSONEL.UNVAN.MANAGE") veya null</returns>
        string? ResolveFromRoute(string route);
    }
}
