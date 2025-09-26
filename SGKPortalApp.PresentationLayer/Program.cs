using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using SGKPortalApp.Common.Extensions;
using SGKPortalApp.DataAccessLayer.Context;

var builder = WebApplication.CreateBuilder(args);

// Port ayarlarını appsettings'den al
var httpsUrl = builder.Configuration["AppSettings:Urls:HttpsUrl"] ?? "https://localhost:7037";
var httpUrl = builder.Configuration["AppSettings:Urls:HttpUrl"] ?? "http://localhost:5243";
var apiUrl = builder.Configuration["AppSettings:ApiUrl"] ?? "https://localhost:7021";

// URL konfigürasyonu
builder.WebHost.UseUrls(httpsUrl, httpUrl);

// Blazor Server servisleri
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Veritabanı bağlantısı
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("DefaultConnection bağlantı dizesi bulunamadı.");

builder.Services.AddDbContext<SGKDbContext>(options =>
{
    options.UseSqlServer(connectionString);

    // Development ortamında detaylı logging
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// SGK Portal servislerini kaydet (Repository Pattern)
builder.Services.AddSGKPortalServices(connectionString);

// CORS (API kullanımı için) - appsettings'den al
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(httpsUrl, apiUrl)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Memory Cache
builder.Services.AddMemoryCache();

// HTTP Context Accessor (Blazor'da user bilgileri için)
builder.Services.AddHttpContextAccessor();

// Logging konfigürasyonu
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
if (builder.Environment.IsDevelopment())
{
    builder.Logging.SetMinimumLevel(LogLevel.Debug);
}

var app = builder.Build();

// Development/Production ayarları
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

// Middleware pipeline
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// CORS middleware
app.UseCors();

// Authentication & Authorization (ileride eklenecek)
// app.UseAuthentication();
// app.UseAuthorization();

// Blazor Hub ve routing
app.MapBlazorHub(options =>
{
    // SignalR için timeout ayarları
    options.ApplicationMaxBufferSize = 65536;
    options.TransportMaxBufferSize = 65536;
});

app.MapFallbackToPage("/_Host");

// Razor Pages (API endpoint'leri için)
app.MapRazorPages();

// Veritabanı migration'larını otomatik uygula (opsiyonel)
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<SGKDbContext>();

        // Veritabanı var mı kontrol et
        if (context.Database.GetPendingMigrations().Any())
        {
            Console.WriteLine("Bekleyen migration'lar uygulanıyor...");
            context.Database.Migrate();
            Console.WriteLine("Migration'lar başarıyla uygulandı.");
        }
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Migration uygulanırken hata oluştu.");
    }
}

Console.WriteLine("SGK Portal uygulaması başlatılıyor...");
Console.WriteLine($"Ortam: {app.Environment.EnvironmentName}");
Console.WriteLine($"HTTPS URL: {httpsUrl}");
Console.WriteLine($"HTTP URL: {httpUrl}");
Console.WriteLine($"API URL: {apiUrl}");

app.Run();