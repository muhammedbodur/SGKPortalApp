using Microsoft.AspNetCore.Http;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Handlers
{
    public class ApiAuthCookieForwardingHandler : DelegatingHandler
    {
        private const string AuthCookieName = "SGKPortal.Auth";
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiAuthCookieForwardingHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext?.Request?.Cookies != null
                && httpContext.Request.Cookies.TryGetValue(AuthCookieName, out var authCookie)
                && !string.IsNullOrWhiteSpace(authCookie))
            {
                request.Headers.Remove("Cookie");
                request.Headers.TryAddWithoutValidation("Cookie", $"{AuthCookieName}={authCookie}");
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
