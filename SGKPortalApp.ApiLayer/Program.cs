using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SGKPortalApp.Common.Extensions;
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
            // 📌 PORT AYARLARI
            // ═══════════════════════════════════════════════════════
            var httpsUrl = builder.Configuration["AppSettings:Urls:HttpsUrl"] ?? "https://localhost:7021";
            var httpUrl = builder.Configuration["AppSettings:Urls:HttpUrl"] ?? "http://localhost:5272";

            // URL konfigürasyonu
            builder.WebHost.UseUrls(httpsUrl, httpUrl);

            // ═══════════════════════════════════════════════════════
            // 📦 CONTROLLERS & JSON SERİALİZATİON
            // ═══════════════════════════════════════════════════════
            builder.Services.AddControllers()
                .AddJsonOptions(options =>  // ← GELİŞTİRİLDİ
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.WriteIndented = true;
                });

            // ═══════════════════════════════════════════════════════
            // 🗄️ DATABASE CONNECTION - YENİ EKLENEN
            // ═══════════════════════════════════════════════════════
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("❌ DefaultConnection bağlantı dizesi bulunamadı!");

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
            // 🎯 SGK PORTAL SERVİSLERİ - YENİ EKLENEN
            // Data Access Layer + Business Logic Layer
            // ═══════════════════════════════════════════════════════
            builder.Services.AddSGKPortalServices(connectionString);

            // ═══════════════════════════════════════════════════════
            // 🔧 AUTOMAPPER - YENİ EKLENEN
            // ═══════════════════════════════════════════════════════
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

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
                        Email = "sgk@portal.gov.tr"
                    }
                });

                // JWT Bearer token configuration for Swagger
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
                    // Presentation layer'dan gelen istekleri kabul et
                    var presentationUrl = builder.Configuration["AppSettings:PresentationUrl"] ?? "https://localhost:7037";

                    policy.WithOrigins(
                            presentationUrl,
                            "http://localhost:3000",
                            "https://localhost:3001",
                            "http://localhost:5243",  // ← EKLENEN
                            "https://localhost:8080"  // ← EKLENEN
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
                    c.RoutePrefix = string.Empty; // Swagger'ı root'da aç
                    c.DocumentTitle = "SGK Portal API Documentation";
                    c.DefaultModelsExpandDepth(-1); // Model örneklerini gizle
                });

                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            // Middleware pipeline
            app.UseHttpsRedirection();

            // CORS middleware
            app.UseCors("SGKPortalPolicy");

            // Authentication & Authorization (ileride eklenecek)
            // app.UseAuthentication();
            app.UseAuthorization();

            // Health check endpoint
            app.MapHealthChecks("/health");

            // API Controllers
            app.MapControllers();

            // ═══════════════════════════════════════════════════════
            // 📊 API INFO ENDPOINT
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
            // 🗄️ DATABASE MIGRATION - YENİ EKLENEN
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
                }
            }

            // ═══════════════════════════════════════════════════════
            // 🚀 APPLICATION START
            // ═══════════════════════════════════════════════════════
            Console.WriteLine("\n╔════════════════════════════════════════════════════════╗");
            Console.WriteLine("║         SGK PORTAL API BAŞLATILIYOR...                 ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════╝");
            Console.WriteLine($"🌐 Ortam: {app.Environment.EnvironmentName}");
            Console.WriteLine($"🔒 HTTPS URL: {httpsUrl}");
            Console.WriteLine($"🌍 HTTP URL: {httpUrl}");
            Console.WriteLine($"📖 Swagger: {httpsUrl}");
            Console.WriteLine($"❤️  Health: {httpsUrl}/health");
            Console.WriteLine("╚════════════════════════════════════════════════════════╝\n");

            app.Run();
        }
    }
}