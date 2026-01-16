using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.OpenApi.Models;
using SGKPortalApp.ApiLayer.Services.Hubs;
using SGKPortalApp.ApiLayer.Services.Hubs.Concrete;
using SGKPortalApp.ApiLayer.Services.Hubs.Filters;
using SGKPortalApp.ApiLayer.Services.Hubs.Interfaces;
using SGKPortalApp.ApiLayer.Services.State;
using SGKPortalApp.BusinessLogicLayer.Interfaces.SignalR;
using SGKPortalApp.DataAccessLayer.Services.Interfaces;
using SGKPortalApp.Common.Extensions;
using SGKPortalApp.BusinessLogicLayer.Extensions;
using SGKPortalApp.DataAccessLayer.Extensions;
using System.Text.Json.Serialization;

namespace SGKPortalApp.ApiLayer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
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

            // Audit Logging Configuration
            var auditConfigPath = Path.Combine(
                Directory.GetParent(Directory.GetCurrentDirectory())!.FullName,
                "appsettings.AuditLogging.json"
            );

            if (File.Exists(auditConfigPath))
            {
                builder.Configuration.AddJsonFile(
                    auditConfigPath,
                    optional: false,
                    reloadOnChange: true
                );
                Console.WriteLine($"✅ Audit logging configuration yüklendi: {auditConfigPath}");
            }
            else
            {
                Console.WriteLine($"⚠️  Audit logging configuration bulunamadı: {auditConfigPath}");
            }

            // ═══════════════════════════════════════════════════════
            // 📌 PORT AYARLARI
            // ═══════════════════════════════════════════════════════
            var httpsUrl = builder.Configuration["AppSettings:Urls:HttpsUrl"] ?? "https://localhost:9080";
            var httpUrl = builder.Configuration["AppSettings:Urls:HttpUrl"] ?? "http://localhost:9081";

            builder.WebHost.UseUrls(httpsUrl, httpUrl);

            // ═══════════════════════════════════════════════════════
            // 📦 CONTROLLERS & JSON SERİALİZATİON
            // ═══════════════════════════════════════════════════════
            builder.Services.AddControllers(options =>
                {
                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();

                    options.Filters.Add(new AuthorizeFilter(policy));
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.WriteIndented = true;
                });

            builder.Services.AddHttpContextAccessor();

            var sharedKeysPath = Path.Combine(
                Directory.GetParent(builder.Environment.ContentRootPath)!.FullName,
                "SharedDataProtectionKeys");
            Directory.CreateDirectory(sharedKeysPath);

            builder.Services
                .AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(sharedKeysPath))
                .SetApplicationName("SGKPortalApp");

            builder.Services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = "SGKPortal.Auth";
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                    options.Cookie.SameSite = SameSiteMode.Lax;

                    options.Events.OnRedirectToLogin = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return Task.CompletedTask;
                    };

                    options.Events.OnRedirectToAccessDenied = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        return Task.CompletedTask;
                    };
                });

            builder.Services.AddAuthorization();

            // ═══════════════════════════════════════════════════════
            // ⭐ KATMAN SERVİSLERİ ⭐
            // ═══════════════════════════════════════════════════════
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("❌ DefaultConnection bağlantı dizesi bulunamadı!");

            Console.WriteLine($"📊 Database Connection: {connectionString.Substring(0, Math.Min(50, connectionString.Length))}...");

            // 1. Common Layer (Shared services)
            builder.Services.AddCommonServices();

            // 2. Data Access Layer (DbContext + Repository + UnitOfWork)
            builder.Services.AddDataAccessLayer(connectionString);
            builder.Services.AddAuditLogging(builder.Configuration);

            // 3. Business Logic Layer (Business Services)
            builder.Services.AddBusinessLogicLayer();

            // ═══════════════════════════════════════════════════════
            // 🔌 ZKTeco API CLIENT
            // ═══════════════════════════════════════════════════════
            var zkTecoApiBaseUrl = builder.Configuration["ZKTecoApi:BaseUrl"]
                ?? throw new InvalidOperationException("❌ ZKTecoApi:BaseUrl yapılandırması bulunamadı!");

            // Named HttpClient yapılandırması
            builder.Services.AddHttpClient("ZKTecoApiClient", client =>
            {
                client.BaseAddress = new Uri(zkTecoApiBaseUrl);
                client.Timeout = TimeSpan.FromSeconds(builder.Configuration.GetValue<int>("ZKTecoApi:Timeout", 30));
            });

            // ZKTecoApiClient servis kaydı
            builder.Services.AddTransient<SGKPortalApp.BusinessLogicLayer.Interfaces.PdksIslemleri.IZKTecoApiClient>(serviceProvider =>
            {
                var httpClientFactory = serviceProvider.GetRequiredService<System.Net.Http.IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient("ZKTecoApiClient");
                var logger = serviceProvider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<SGKPortalApp.BusinessLogicLayer.Services.PdksIslemleri.ZKTecoApiClient>>();
                return new SGKPortalApp.BusinessLogicLayer.Services.PdksIslemleri.ZKTecoApiClient(httpClient, logger, zkTecoApiBaseUrl);
            });

            Console.WriteLine($"🔌 ZKTeco API Client yapılandırıldı: {zkTecoApiBaseUrl}");

            // ═══════════════════════════════════════════════════════
            // 🔄 ZKTeco REALTIME SERVICE (SignalR Client)
            // ═══════════════════════════════════════════════════════
            var zkTecoSignalRUrl = $"{zkTecoApiBaseUrl}/signalr";
            builder.Services.AddSingleton<SGKPortalApp.BusinessLogicLayer.Interfaces.PdksIslemleri.IZKTecoRealtimeService>(serviceProvider =>
            {
                var logger = serviceProvider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<SGKPortalApp.BusinessLogicLayer.Services.PdksIslemleri.ZKTecoRealtimeService>>();
                return new SGKPortalApp.BusinessLogicLayer.Services.PdksIslemleri.ZKTecoRealtimeService(logger, zkTecoSignalRUrl);
            });

            Console.WriteLine($"🔄 ZKTeco Realtime Service yapılandırıldı: {zkTecoSignalRUrl}");

            // ═══════════════════════════════════════════════════════
            // 🎯 ZKTeco BUSINESS SERVICES
            // ═══════════════════════════════════════════════════════
            builder.Services.AddScoped<SGKPortalApp.BusinessLogicLayer.Interfaces.PdksIslemleri.IDeviceBusinessService, SGKPortalApp.BusinessLogicLayer.Services.PdksIslemleri.DeviceBusinessService>();
            builder.Services.AddScoped<SGKPortalApp.BusinessLogicLayer.Interfaces.PdksIslemleri.IDeviceService, SGKPortalApp.BusinessLogicLayer.Services.PdksIslemleri.DeviceService>();
            builder.Services.AddScoped<SGKPortalApp.BusinessLogicLayer.Interfaces.PdksIslemleri.IZKTecoAttendanceService, SGKPortalApp.BusinessLogicLayer.Services.PdksIslemleri.ZKTecoAttendanceService>();
            builder.Services.AddScoped<SGKPortalApp.BusinessLogicLayer.Services.PdksIslemleri.Interfaces.ICardSyncComparisonService, SGKPortalApp.BusinessLogicLayer.Services.PdksIslemleri.CardSyncComparisonService>();
            builder.Services.AddScoped<SGKPortalApp.BusinessLogicLayer.Services.PdksIslemleri.IPersonelMesaiService, SGKPortalApp.BusinessLogicLayer.Services.PdksIslemleri.PersonelMesaiService>();
            builder.Services.AddScoped<SGKPortalApp.BusinessLogicLayer.Services.PdksIslemleri.IPersonelListService, SGKPortalApp.BusinessLogicLayer.Services.PdksIslemleri.PersonelListService>();
            builder.Services.AddScoped<SGKPortalApp.BusinessLogicLayer.Services.PdksIslemleri.IDepartmanMesaiService, SGKPortalApp.BusinessLogicLayer.Services.PdksIslemleri.DepartmanMesaiService>();

            Console.WriteLine("🎯 ZKTeco Business Services yapılandırıldı");

            // ═══════════════════════════════════════════════════════
            // 📡 SIGNALR SERVİSLERİ
            // ═══════════════════════════════════════════════════════
            builder.Services.AddSignalR(options =>
            {
                // Activity tracking filter ekle (Blazor Server için SonAktiviteZamani güncelleme)
                options.AddFilter<ActivityTrackingHubFilter>();
            });
            builder.Services.AddSingleton<BankoModeStateService>();
            builder.Services.AddSingleton<DeviceMonitoringStateService>();
            builder.Services.AddScoped<IHubConnectionService, HubConnectionService>();
            builder.Services.AddScoped<IBankoModeService, BankoModeService>();
            // ⭐ Audit destekli SignalR Broadcaster (Decorator Pattern)
            builder.Services.AddScoped<ISignalRBroadcaster, AuditingSignalRBroadcaster>();
            builder.Services.AddScoped<ISignalRAuditService, SGKPortalApp.BusinessLogicLayer.Services.SignalR.SignalRAuditService>();
            builder.Services.AddSingleton<IUserIdProvider, TcKimlikNoUserIdProvider>();

            // ═══════════════════════════════════════════════════════
            // 🧹 BACKGROUND SERVICES
            // ═══════════════════════════════════════════════════════
            builder.Services.AddHostedService<SGKPortalApp.ApiLayer.Services.BackgroundServices.SessionCleanupService>();
            builder.Services.AddHostedService<SGKPortalApp.ApiLayer.Services.BackgroundServices.IdleSessionCleanupService>(); // 30 dakika idle timeout

            // ZKTeco background services
            builder.Services.AddHostedService<SGKPortalApp.ApiLayer.Services.BackgroundServices.ZKTecoRealtimeListenerService>(); // ZKTeco realtime event listener
            builder.Services.AddHostedService<SGKPortalApp.ApiLayer.Services.BackgroundServices.AttendanceSyncBackgroundService>(); // ZKTeco attendance periodic sync (00:00 ve 12:00)

            // ═══════════════════════════════════════════════════════
            // 🔧 AUTOMAPPER
            // ═══════════════════════════════════════════════════════
            // Sadece proje assembly'lerini scan et - üçüncü parti kütüphaneleri değil
            // Microsoft.AspNet.SignalR.Client artık BusinessLogicLayer'da (sadece orada kullanılıyor)
            // BusinessObjectLayer temiz (sadece DTOs/Entities) - güvenle scan edilebilir
            builder.Services.AddAutoMapper(cfg =>
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => a.FullName != null && a.FullName.StartsWith("SGKPortalApp"))
                    .ToArray();
                cfg.AddMaps(assemblies);
            });

            // ═══════════════════════════════════════════════════════
            // ❤️ HEALTH CHECKS
            // ═══════════════════════════════════════════════════════
            builder.Services.AddHealthChecks();

            // ═══════════════════════════════════════════════════════
            // 📖 SWAGGER/OpenAPI
            // ═══════════════════════════════════════════════════════
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "SGK Portal API",
                    Version = "v1",
                    Description = "SGK Portal API servisleri - Personel İşlemleri, Sıramatik İşlemleri, PDKS İşlemleri ve Eshot İşlemleri",
                    Contact = new OpenApiContact
                    {
                        Name = "SGK Portal Geliştirme Ekibi",
                        Email = "muhammedbodur@gmail.com"
                    }
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
            });

            // ═══════════════════════════════════════════════════════
            // 🌐 CORS CONFIGURATION
            // ═══════════════════════════════════════════════════════
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("SGKPortalPolicy", policy =>
                {
                    var presentationUrl = builder.Configuration["AppSettings:PresentationUrl"] ?? "https://localhost:8080";

                    policy.WithOrigins(
                            presentationUrl,
                            "http://localhost:3000",
                            "https://localhost:3001",
                            "http://localhost:5243",
                            "https://localhost:8080"
                          )
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
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
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SGK Portal API v1");
                    c.RoutePrefix = string.Empty;
                    c.DocumentTitle = "SGK Portal API Documentation";
                    c.DefaultModelsExpandDepth(-1);
                });

                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseCors("SGKPortalPolicy");
            app.UseAuthentication();
            app.UseAuthorization();

            // ═══════════════════════════════════════════════════════
            // ❤️ HEALTH CHECK ENDPOINTS
            // ═══════════════════════════════════════════════════════
            app.MapHealthChecks("/health");
            app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("ready")
            });
            app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
            {
                Predicate = _ => false
            });

            app.MapControllers();

            // ═══════════════════════════════════════════════════════
            // 📡 SIGNALR HUB ENDPOINTS
            // ═══════════════════════════════════════════════════════
            app.MapHub<SiramatikHub>("/hubs/siramatik");
            app.MapHub<PdksHub>("/hubs/pdks"); // ZKTeco realtime events

            // ═══════════════════════════════════════════════════════
            // 🏠 ROOT ENDPOINT (API Info)
            // ═══════════════════════════════════════════════════════
            app.MapGet("/", () => new
            {
                Service = "SGK Portal API",
                Version = "v1.0.0",
                Environment = app.Environment.EnvironmentName,
                Timestamp = DateTime.UtcNow,
                Endpoints = new
                {
                    Swagger = "/swagger",
                    Health = "/health",
                    HealthReady = "/health/ready",
                    HealthLive = "/health/live",
                    Controllers = new[]
                    {
                        "/api/personel",
                        "/api/departman",
                        "/api/servis",
                        "/api/unvan",
                        "/api/banko",
                        "/api/kanal",
                        "/api/kiosk",
                        "/api/tv",
                        "/api/sira",
                        "/api/pdks",
                        "/api/izin",
                        "/api/eshot",
                        "/api/auth"
                    }
                }
            }).WithTags("Info");

            // ═══════════════════════════════════════════════════════
            // 🗄️ DATABASE MIGRATION (Sadece Production)
            // ═══════════════════════════════════════════════════════
            // Development'ta manuel migration kullanılır: Add-Migration, Update-Database
            // Production'da otomatik migration uygulanır
            if (!app.Environment.IsDevelopment())
            {
                using (var scope = app.Services.CreateScope())
                {
                    try
                    {
                        var migrationService = scope.ServiceProvider.GetRequiredService<IDatabaseMigrationService>();
                        await migrationService.ApplyMigrationsAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ Migration hatası: {ex.Message}");
                        throw; // Production'da migration hatası kritik
                    }
                }
            }
            else
            {
                Console.WriteLine("ℹ️  Development ortamı - Migration'lar manuel uygulanmalı (Add-Migration, Update-Database)");
            }

            Console.WriteLine("\n╔════════════════════════════════════════════════════════╗");
            Console.WriteLine("║         SGK PORTAL API BAŞLATILIYOR...                 ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════╝");
            Console.WriteLine($"🌐 Ortam: {app.Environment.EnvironmentName}");
            Console.WriteLine($"🔒 HTTPS URL: {httpsUrl}");
            Console.WriteLine($"🌍 HTTP URL: {httpUrl}");
            Console.WriteLine($"📖 Swagger: {httpsUrl}");
            Console.WriteLine($"❤️  Health: {httpsUrl}/health");
            Console.WriteLine($"❤️  Health Ready: {httpsUrl}/health/ready");
            Console.WriteLine($"❤️  Health Live: {httpsUrl}/health/live");
            Console.WriteLine("╚════════════════════════════════════════════════════════╝\n");

            app.Run();
        }
    }
}