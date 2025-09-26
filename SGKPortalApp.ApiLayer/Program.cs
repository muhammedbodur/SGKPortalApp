using Microsoft.OpenApi.Models;

namespace SGKPortalApp.ApiLayer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Port ayarlarını appsettings'den al
            var httpsUrl = builder.Configuration["AppSettings:Urls:HttpsUrl"] ?? "https://localhost:7021";
            var httpUrl = builder.Configuration["AppSettings:Urls:HttpUrl"] ?? "http://localhost:5272";

            // URL konfigürasyonu
            builder.WebHost.UseUrls(httpsUrl, httpUrl);

            // Add services to the container
            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

            // CORS configuration
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("SGKPortalPolicy", policy =>
                {
                    // Presentation layer'dan gelen istekleri kabul et
                    var presentationUrl = builder.Configuration["AppSettings:PresentationUrl"] ?? "https://localhost:7037";

                    policy.WithOrigins(presentationUrl, "http://localhost:3000", "https://localhost:3001")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            // JSON serialization options
            builder.Services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                options.SerializerOptions.WriteIndented = true;
            });

            // Health checks
            builder.Services.AddHealthChecks();

            // Logging configuration
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            if (builder.Environment.IsDevelopment())
            {
                builder.Logging.SetMinimumLevel(LogLevel.Debug);
            }

            var app = builder.Build();

            // Configure the HTTP request pipeline
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

            // API Info endpoint
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
                        "/api/banko",
                        "/api/kanal",
                        "/api/sira",
                        "/api/auth"
                    }
                }
            }).WithTags("Info");

            Console.WriteLine("SGK Portal API başlatılıyor...");
            Console.WriteLine($"Ortam: {app.Environment.EnvironmentName}");
            Console.WriteLine($"API HTTPS URL: {httpsUrl}");
            Console.WriteLine($"API HTTP URL: {httpUrl}");
            Console.WriteLine($"Swagger Dokümantasyon: {httpsUrl}");
            Console.WriteLine($"Health Check: {httpsUrl}/health");

            app.Run();
        }
    }
}