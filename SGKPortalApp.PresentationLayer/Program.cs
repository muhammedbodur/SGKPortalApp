using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using SGKPortalApp.Common.Extensions;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.PresentationLayer.Extensions;
using SGKPortalApp.PresentationLayer.Helpers;
using SGKPortalApp.PresentationLayer.Middleware;
using SGKPortalApp.PresentationLayer.Services.Permission;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// ═══════════════════════════════════════════════════════
// 📄 SHARED CONFIGURATION
// ═══════════════════════════════════════════════════════
var sharedConfigPath = Path.Combine(
    Directory.GetParent(Directory.GetCurrentDirectory())!.FullName,
    "appsettings.Shared.json"
);

if (File.Exists(sharedConfigPath))
{
    builder.Configuration.AddJsonFile(
        sharedConfigPath,
        optional: false,
        reloadOnChange: true
    );
    Console.WriteLine($"✅ Shared configuration yüklendi: {sharedConfigPath}");
}
else
{
    Console.WriteLine($"⚠️  Shared configuration bulunamadı: {sharedConfigPath}");
}

// ═══════════════════════════════════════════════════════
// 📌 PORT AYARLARI
// ═══════════════════════════════════════════════════════
var httpsUrl = builder.Configuration["AppSettings:Urls:HttpsUrl"] ?? "https://localhost:8080";
var httpUrl = builder.Configuration["AppSettings:Urls:HttpUrl"] ?? "http://localhost:8081";
var apiUrl = builder.Configuration["AppSettings:ApiUrl"] ?? "https://localhost:9080";

builder.WebHost.UseUrls(httpsUrl, httpUrl);

// ═══════════════════════════════════════════════════════
// 🚀 RESPONSE COMPRESSION (3 Mbit bağlantılar için)
// ═══════════════════════════════════════════════════════
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

// ═══════════════════════════════════════════════════════
// 🔥 BLAZOR SERVER SERVİSLERİ
// ═══════════════════════════════════════════════════════
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor(options =>
{
    options.DetailedErrors = builder.Environment.IsDevelopment();
    options.DisconnectedCircuitMaxRetained = 50;
    options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(3);
    options.JSInteropDefaultCallTimeout = TimeSpan.FromMinutes(2);
    options.MaxBufferedUnacknowledgedRenderBatches = 5;
});

// ═══════════════════════════════════════════════════════
// 🗄️ DATABASE CONNECTION (Shared Configuration'dan)
// ═══════════════════════════════════════════════════════
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("❌ DefaultConnection bağlantı dizesi bulunamadı!");

Console.WriteLine($"📊 Database Connection: {connectionString.Substring(0, Math.Min(50, connectionString.Length))}...");

builder.Services.AddDbContext<SGKDbContext>(options =>
{
    options.UseSqlServer(connectionString);

    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// ═══════════════════════════════════════════════════════
// 🌐 HTTP CLIENT (API çağrıları için)
// ═══════════════════════════════════════════════════════
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(apiUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
})
    .AddHttpMessageHandler<SGKPortalApp.PresentationLayer.Services.ApiServices.Handlers.ApiAuthCookieForwardingHandler>();

Console.WriteLine($"✅ HttpClient configured - BaseAddress: {apiUrl}");

// ═══════════════════════════════════════════════════════
// ⭐ KATMAN SERVİSLERİ ⭐
// ═══════════════════════════════════════════════════════
// 1. Data Access Layer + core servisler (Shared connection string kullanıyor)
builder.Services.AddSGKPortalServices(builder.Configuration);

// 2. Presentation Layer (UI Services)
builder.Services.AddPresentationServices(builder.Configuration);

builder.Services.AddScoped<Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider,
    SGKPortalApp.PresentationLayer.Services.AuthenticationServices.Concrete.ServerAuthenticationStateProvider>();

//Console.WriteLine("✅ ServerAuthenticationStateProvider MANUEL kayıt edildi");

// ═══════════════════════════════════════════════════════
// 🌐 CORS (API kullanımı için)
// ═══════════════════════════════════════════════════════
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(httpsUrl, apiUrl)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ═══════════════════════════════════════════════════════
// 💾 MEMORY CACHE
// ═══════════════════════════════════════════════════════
builder.Services.AddMemoryCache();

// ═══════════════════════════════════════════════════════
// 📡 SIGNALR - API (9080) UZERINDEN KULLANILIYOR
// ═══════════════════════════════════════════════════════
// NOT: SignalR Hublari API katmaninda (9080) tanimli.
// Presentation katmani JavaScript uzerinden APIye baglanir.

// Hub Connection API Service (Layered Architecture)
builder.Services.AddScoped<SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.SignalR.IHubConnectionApiService,
    SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.SignalR.HubConnectionApiService>();

// SignalR Event Log API Service
builder.Services.AddScoped<SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.SignalR.ISignalREventLogApiService,
    SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.SignalR.SignalREventLogApiService>();

// Banko Mode State Service (Singleton - Tum uygulama boyunca tek instance)
builder.Services.AddSingleton<SGKPortalApp.PresentationLayer.Services.State.BankoModeStateService>();

Console.WriteLine("SignalR Hub: API katmaninda (https://localhost:9080/hubs/siramatik)");

// ═══════════════════════════════════════════════════════
// 🔧 AUTOMAPPER
// ═══════════════════════════════════════════════════════
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// ═══════════════════════════════════════════════════════
// 🔐 HTTP CONTEXT ACCESSOR (Blazor'da user bilgileri için)
// ═══════════════════════════════════════════════════════
builder.Services.AddHttpContextAccessor();

var sharedKeysPath = Path.Combine(
    Directory.GetParent(builder.Environment.ContentRootPath)!.FullName,
    "SharedDataProtectionKeys");
Directory.CreateDirectory(sharedKeysPath);

builder.Services
    .AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(sharedKeysPath))
    .SetApplicationName("SGKPortalApp");

// ═══════════════════════════════════════════════════════
// 🔒 AUTHENTICATION & AUTHORIZATION
// ═══════════════════════════════════════════════════════
builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/auth/login";
        options.LogoutPath = "/auth/logout";
        options.AccessDeniedPath = "/auth/access-denied";
        options.ExpireTimeSpan = TimeSpan.FromHours(9); // Personel için 9 saat
        options.SlidingExpiration = true;
        options.Cookie.Name = "SGKPortal.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.SameSite = SameSiteMode.Lax;
        
        // TV kullanıcıları için oturum süresi maksimum yapıldı
        options.Events.OnSigningIn = context =>
        {
            var userTypeClaim = context.Principal?.FindFirst("UserType");
            if (userTypeClaim?.Value == "TvUser")
            {
                // TV için 365 gün (1 yıl) oturum süresi
                context.Properties.ExpiresUtc = DateTimeOffset.UtcNow.AddDays(365);
                context.Properties.IsPersistent = true; // Tarayıcı kapansa bile oturum açık kalsın
            }
            return Task.CompletedTask;
        };
    });

// Authorization Handlers
builder.Services.AddSingleton<Microsoft.AspNetCore.Authorization.IAuthorizationHandler,
    SGKPortalApp.PresentationLayer.Authorization.TvUserAuthorizationHandler>();

builder.Services.AddAuthorization(options =>
{
    // Personel Policy - Sadece Personel tipindeki kullanıcılar
    options.AddPolicy("PersonelOnly", policy =>
        policy.RequireClaim("UserType", "Personel"));

    // TV Policy - Sadece TV tipindeki kullanıcılar
    options.AddPolicy("TvOnly", policy =>
        policy.RequireClaim("UserType", "TvUser"));

    // Banko Modu Policy - Banko modunda olmayan kullanıcılar
    options.AddPolicy("NotInBankoMode", policy =>
        policy.RequireAssertion(context =>
        {
            var bankoModuClaim = context.User.FindFirst("BankoModuAktif");
            return bankoModuClaim == null || bankoModuClaim.Value != "true";
        }));

    // TV User Restriction Policy - TV User'lar sadece kendi Display sayfasına erişebilir
    options.AddPolicy("TvUserRestriction", policy =>
        policy.Requirements.Add(new SGKPortalApp.PresentationLayer.Authorization.TvUserRequirement()));
});

// ═══════════════════════════════════════════════════════
// 🌍 LOCALİZATİON (Yerelleştirme)
// ═══════════════════════════════════════════════════════
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

var supportedCultures = new[]
{
    new CultureInfo("tr-TR"),
    new CultureInfo("en-US")
};

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("tr-TR");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

// Image Helper Service
builder.Services.AddScoped<ImageHelper>();

// Stale Connection Cleanup Background Service
builder.Services.AddHostedService<SGKPortalApp.PresentationLayer.Services.BackgroundServices.StaleConnectionCleanupService>();

// ═══════════════════════════════════════════════════════
// 📝 LOGGING CONFIGURATION
// ═══════════════════════════════════════════════════════
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
if (builder.Environment.IsDevelopment())
{
    builder.Logging.SetMinimumLevel(LogLevel.Debug);
}

// ═══════════════════════════════════════════════════════
// 🏗️ BUILD APPLICATION
// ═══════════════════════════════════════════════════════
var app = builder.Build();

// ═══════════════════════════════════════════════════════
// 🔧 MIDDLEWARE PIPELINE
// ═══════════════════════════════════════════════════════
app.UseResponseCompression();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors();

// Authentication & Authorization
// ÖNEMLİ: UseAuthentication, UseRouting'den SONRA olmalı
app.UseAuthentication();
app.UseAuthorization();

// Session Validation Middleware
// Her request'te session ID kontrolü yapar, farklı cihazdan login varsa logout eder
app.UseMiddleware<SessionValidationMiddleware>();

// TV User Restriction Middleware
// TV User'ların sadece kendi Display sayfalarına erişmesini sağlar
app.UseTvUserRestriction();

// ═══════════════════════════════════════════════════════
// 🔌 BLAZOR HUB & ROUTING
// ═══════════════════════════════════════════════════════
app.MapBlazorHub(options =>
{
    options.ApplicationMaxBufferSize = 32768; // 32KB
    options.TransportMaxBufferSize = 32768;
}).AllowAnonymous();

// ═══════════════════════════════════════════════════════
// 📡 SIGNALR - API (9080) ÜZERİNDEN KULLANILIYOR
// ═══════════════════════════════════════════════════════
// NOT: SignalR Hub'ları API katmanında (9080) tanımlı.
// Presentation katmanı JavaScript üzerinden API'deki hub'a bağlanıyor.
// Eski mapping'ler kaldırıldı - tüm SignalR trafiği API üzerinden.
Console.WriteLine("ℹ️ SignalR Hub: API katmanında (https://localhost:9080/hubs/siramatik)");

app.MapRazorPages();
app.MapFallbackToPage("/_Host");

// ═══════════════════════════════════════════════════════
// 🗄️ DATABASE MIGRATION
// ═══════════════════════════════════════════════════════
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<SGKDbContext>();

        if (context.Database.GetPendingMigrations().Any())
        {
            Console.WriteLine("📊 Bekleyen migration'lar uygulanıyor...");
            context.Database.Migrate();
            Console.WriteLine("✅ Migration'lar başarıyla uygulandı");
        }
        else
        {
            Console.WriteLine("✅ Veritabanı güncel");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Migration hatası: {ex.Message}");
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Migration uygulanırken hata oluştu.");
    }
}

Console.WriteLine("\n╔════════════════════════════════════════════════════════╗");
Console.WriteLine("║      SGK PORTAL PRESENTATION BAŞLATILIYOR...           ║");
Console.WriteLine("╚════════════════════════════════════════════════════════╝");
Console.WriteLine($"🌐 Ortam: {app.Environment.EnvironmentName}");
Console.WriteLine($"🔒 HTTPS URL: {httpsUrl}");
Console.WriteLine($"🌍 HTTP URL: {httpUrl}");
Console.WriteLine($"🔌 API URL: {apiUrl}");
Console.WriteLine($"📊 Database: {connectionString.Split(';')[0]}");
Console.WriteLine("╚════════════════════════════════════════════════════════╝\n");

app.Run();
