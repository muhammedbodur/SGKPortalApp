using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using SGKPortalApp.Common.Extensions;
using SGKPortalApp.PresentationLayer.Extensions;
using SGKPortalApp.DataAccessLayer.Context;
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
// ⭐ KATMAN SERVİSLERİ ⭐
// ═══════════════════════════════════════════════════════
// 1. Data Access Layer + Business Logic Layer (Shared connection string kullanıyor)
builder.Services.AddSGKPortalServices(builder.Configuration);

// 2. Presentation Layer (UI Services)
builder.Services.AddPresentationServices(builder.Configuration);

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
// 🔧 AUTOMAPPER
// ═══════════════════════════════════════════════════════
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// ═══════════════════════════════════════════════════════
// 🔐 HTTP CONTEXT ACCESSOR (Blazor'da user bilgileri için)
// ═══════════════════════════════════════════════════════
builder.Services.AddHttpContextAccessor();

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

// Authentication & Authorization (Login yapısı kurulduğunda aktif edilecek)
// app.UseAuthentication();
// app.UseAuthorization();

// ═══════════════════════════════════════════════════════
// 🔌 BLAZOR HUB & ROUTING
// ═══════════════════════════════════════════════════════
app.MapBlazorHub(options =>
{
    options.ApplicationMaxBufferSize = 32768; // 32KB
    options.TransportMaxBufferSize = 32768;
});

app.MapFallbackToPage("/_Host");
app.MapRazorPages();

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

        // ═══════════════════════════════════════════════════════
        // 🌱 SEED DATA (İsteğe bağlı)
        // ═══════════════════════════════════════════════════════
        // var seeder = scope.ServiceProvider.GetRequiredService<DataSeed>();
        // await seeder.SeedAsync();
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