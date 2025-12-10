using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SGKPortalApp.ApiLayer.Services.Hubs;
using SGKPortalApp.ApiLayer.Services.Hubs.Concrete;
using SGKPortalApp.ApiLayer.Services.Hubs.Interfaces;
using SGKPortalApp.ApiLayer.Services.State;
using SGKPortalApp.BusinessObjectLayer.Interfaces.SignalR;
using SGKPortalApp.Common.Extensions;
using SGKPortalApp.BusinessLogicLayer.Extensions;
using SGKPortalApp.DataAccessLayer.Context;
using System.Text.Json.Serialization;

namespace SGKPortalApp.ApiLayer
{
    public class Program
    {
        public static void Main(string[] args)
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

            // ═══════════════════════════════════════════════════════
            // 📌 PORT AYARLARI
            // ═══════════════════════════════════════════════════════
            var httpsUrl = builder.Configuration["AppSettings:Urls:HttpsUrl"] ?? "https://localhost:9080";
            var httpUrl = builder.Configuration["AppSettings:Urls:HttpUrl"] ?? "http://localhost:9081";

            builder.WebHost.UseUrls(httpsUrl, httpUrl);

            // ═══════════════════════════════════════════════════════
            // 📦 CONTROLLERS & JSON SERİALİZATİON
            // ═══════════════════════════════════════════════════════
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.WriteIndented = true;
                });

            // ═══════════════════════════════════════════════════════
            // 🎯 SGK PORTAL SERVİSLERİ
            // ═══════════════════════════════════════════════════════
            builder.Services.AddSGKPortalServices(builder.Configuration);

            // Business Logic Layer
            builder.Services.AddBusinessLogicLayer();

            // ═══════════════════════════════════════════════════════
            // 📡 SIGNALR SERVİSLERİ
            // ═══════════════════════════════════════════════════════
            builder.Services.AddSignalR();
            builder.Services.AddSingleton<BankoModeStateService>();
            builder.Services.AddScoped<IHubConnectionService, HubConnectionService>();
            builder.Services.AddScoped<IBankoModeService, BankoModeService>();
            // ⭐ Audit destekli SignalR Broadcaster (Decorator Pattern)
            builder.Services.AddScoped<ISignalRBroadcaster, AuditingSignalRBroadcaster>();
            builder.Services.AddScoped<ISignalRAuditService, SGKPortalApp.BusinessLogicLayer.Services.SignalR.SignalRAuditService>();
            builder.Services.AddSingleton<IUserIdProvider, TcKimlikNoUserIdProvider>();

            // ═══════════════════════════════════════════════════════
            // 🔧 AUTOMAPPER
            // ═══════════════════════════════════════════════════════
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // ═══════════════════════════════════════════════════════
            // ❤️ HEALTH CHECKS
            // ═══════════════════════════════════════════════════════
            builder.Services.AddHealthChecks()
                .AddDbContextCheck<SGKDbContext>("database", tags: new[] { "db", "sql" });

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
            // 🗄️ DATABASE MIGRATION & SCRIPTS
            // ═══════════════════════════════════════════════════════
            using (var scope = app.Services.CreateScope())
            {
                try
                {
                    var context = scope.ServiceProvider.GetRequiredService<SGKDbContext>();
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

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

                    // SQL Script'leri çalıştır (View, SP, Function)
                    var scriptsPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", 
                        "SGKPortalApp.DataAccessLayer", "Scripts");
                    
                    if (Directory.Exists(scriptsPath))
                    {
                        Console.WriteLine("📜 SQL Script'leri çalıştırılıyor...");
                        SGKPortalApp.DataAccessLayer.Scripts.DatabaseScriptRunner
                            .RunScriptsFromFolderAsync(context, scriptsPath, logger).Wait();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Migration/Script hatası: {ex.Message}");
                }
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