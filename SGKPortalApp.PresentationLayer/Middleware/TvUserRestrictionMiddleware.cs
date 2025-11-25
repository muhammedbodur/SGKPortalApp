namespace SGKPortalApp.PresentationLayer.Middleware
{
    /// <summary>
    /// TV User'ların sadece kendi Display sayfalarına erişmesini sağlar
    /// Diğer tüm sayfalara erişimi engeller
    /// </summary>
    public class TvUserRestrictionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TvUserRestrictionMiddleware> _logger;

        public TvUserRestrictionMiddleware(
            RequestDelegate next,
            ILogger<TvUserRestrictionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var user = context.User;
            var userType = user?.FindFirst("UserType")?.Value;

            // TV User kontrolü
            if (userType == "TvUser")
            {
                var path = context.Request.Path.Value?.ToLower() ?? "";
                var tcKimlikNo = user?.FindFirst("TcKimlikNo")?.Value;

                // TcKimlikNo formatı: TV0000001
                if (tcKimlikNo?.StartsWith("TV") == true)
                {
                    var tvIdFromUser = int.Parse(tcKimlikNo.Substring(2)); // "TV0000001" -> 1
                    var allowedPath = $"/siramatik/tv/display/{tvIdFromUser}";

                    // İzin verilen yollar
                    var isAllowed = path == allowedPath.ToLower() ||
                                    path.StartsWith("/auth/") ||
                                    path.StartsWith("/_blazor") ||
                                    path.StartsWith("/_framework") ||
                                    path.StartsWith("/css/") ||
                                    path.StartsWith("/js/") ||
                                    path.StartsWith("/lib/") ||
                                    path.StartsWith("/logo/") ||
                                    path.StartsWith("/sounds/") ||
                                    path.StartsWith("/video/") ||
                                    path.StartsWith("/hubs/");

                    if (!isAllowed)
                    {
                        _logger.LogWarning($"❌ TV User yetkisiz erişim engellendi: {tcKimlikNo} -> {path}");
                        
                        // TV Display sayfasına yönlendir
                        context.Response.Redirect(allowedPath);
                        return;
                    }
                    
                    // Root path (/) için otomatik yönlendirme
                    if (path == "/")
                    {
                        _logger.LogInformation($"🔄 TV User root path'den Display'e yönlendiriliyor: {tcKimlikNo}");
                        context.Response.Redirect(allowedPath);
                        return;
                    }
                }
            }

            await _next(context);
        }
    }

    /// <summary>
    /// Middleware extension methods
    /// </summary>
    public static class TvUserRestrictionMiddlewareExtensions
    {
        public static IApplicationBuilder UseTvUserRestriction(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TvUserRestrictionMiddleware>();
        }
    }
}
