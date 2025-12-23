namespace SGKPortalApp.BusinessLogicLayer.Interfaces.Common
{
    /// <summary>
    /// HTTP isteğinin route bilgisinden permission key'i otomatik çözümleyen servis.
    /// Frontend'deki PermissionStateService.GetPermissionKeyByRoute() ile aynı mantık.
    ///
    /// Kullanım:
    /// - Service'lerde field validation yaparken otomatik permission key çözümlemesi
    /// - Frontend ile backend tutarlılığı sağlar (tek mapping source)
    /// </summary>
    public interface IPermissionKeyResolverService
    {
        /// <summary>
        /// Mevcut HTTP request'in route'undan permission key'i çözümler.
        ///
        /// Örnek:
        /// - Route: /personel/unvan → Permission Key: "PERSONEL.UNVAN.INDEX"
        /// - Route: /personel/unvan/manage → Permission Key: "PERSONEL.UNVAN.MANAGE"
        /// - Route: /siramatik/banko → Permission Key: "SIRAMATIK.BANKO.INDEX"
        ///
        /// NOT: ModulControllerIslem tablosundaki Route → PermissionKey mapping kullanılır.
        /// </summary>
        /// <returns>
        /// Permission key (örn: "PERSONEL.UNVAN.MANAGE") veya null (route bulunamazsa)
        /// </returns>
        Task<string?> ResolveFromCurrentRequestAsync();

        /// <summary>
        /// Belirtilen route'tan permission key'i çözümler.
        /// Test ve özel durumlar için kullanılabilir.
        /// </summary>
        /// <param name="route">Route path (örn: "/personel/unvan")</param>
        /// <returns>Permission key veya null</returns>
        Task<string?> ResolveFromRouteAsync(string route);
    }
}
