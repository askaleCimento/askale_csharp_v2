using AskalePortal.API.Extensions;
using AskalePortal.API.Infrastructure.Errors;
using AskalePortal.API.Infrastructure.Serialization;
using AskalePortal.API.Mapper;
using AskalePortal.API.Security.Auth;
using AskalePortal.API.Security.Auth.Cleanup;
using AskalePortal.BLL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Modüller
builder.Services.AddEducationModule(
    builder.Configuration,
    builder.Environment);

// Ortak servisler
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddProblemDetails();
builder.Services.AddScoped<DetachedEntityResultFilter>();
builder.Services.AddScoped<PaginationResultFilter>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// JWT ayarları
builder.Services
    .AddOptions<JwtOptions>()
    .Bind(builder.Configuration.GetSection(JwtOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddScoped<IAuthService, AuthService>();

// Refresh token cleanup ayarları
builder.Services
    .AddOptions<RefreshTokenCleanupOptions>()
    .Bind(
        builder.Configuration.GetSection(
            RefreshTokenCleanupOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

// Cleanup kapalıysa hosted service hiç oluşturulmaz.
var refreshTokenCleanupEnabled =
    builder.Configuration.GetValue<bool>(
        $"{RefreshTokenCleanupOptions.SectionName}:Enabled");

if (refreshTokenCleanupEnabled)
{
    builder.Services.AddHostedService<RefreshTokenCleanupService>();
}

// Controller ve JSON ayarları
builder.Services
    .AddControllers(options => options.Filters.Add<DetachedEntityResultFilter>())
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            ReferenceHandler.IgnoreCycles;
    })
    .AddNewtonsoftJson();

// Model validation cevapları
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = ApiValidation.ToErrors(context.ModelState);

        var response = ApiErrorWriter.Create(
            context.HttpContext,
            StatusCodes.Status400BadRequest,
            "VALIDATION_ERROR",
            "Gönderilen bilgiler geçersiz.",
            errors);

        return new BadRequestObjectResult(response);
    };
});

// Dil ve bölge ayarları
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var cultureInfo = new CultureInfo("en-US");

    CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
    CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

    options.DefaultRequestCulture =
        new RequestCulture(cultureInfo);

    options.SupportedCultures =
        new List<CultureInfo> { cultureInfo };

    options.SupportedUICultures =
        new List<CultureInfo> { cultureInfo };
});
builder.Services.AddScoped<ISftpServer, SftpServer>();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        foreach (var modelState in context.ModelState)
        {
            foreach (var error in modelState.Value.Errors)
            {
                Console.WriteLine(
                    $"MODEL BINDING ERROR | {modelState.Key} | " +
                    $"{error.ErrorMessage} | " +
                    $"{error.Exception?.Message}");
            }
        }

        var errors = ApiValidation.ToErrors(context.ModelState);

        var response = ApiErrorWriter.Create(
            context.HttpContext,
            StatusCodes.Status400BadRequest,
            "VALIDATION_ERROR",
            "Gönderilen bilgiler geçersiz.",
            errors);

        return new BadRequestObjectResult(response);
    };
});
// Reverse proxy/IIS arkasında gerçek protokolün (X-Forwarded-Proto)
// Request.IsHttps değerine yansımasını sağlar. Refresh cookie Secure/SameSite
// ayarları bu bilgiye göre oluşturulur.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

    // IIS/reverse proxy adresi ortama göre değişebildiği için forwarded header
    // işleme proxy listesiyle sınırlandırılmaz. Dış erişimde proxy'nin bu
    // header'ları temizleyip yeniden yazdığı doğrulanmalıdır.
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// CORS
const string corsPolicyName = "CorsPolicy";

var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicyName, policy =>
    {
        if (allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins);
        }
        else
        {
            // Geriye dönük geliştirme davranışı. Canlı ortamda AllowedOrigins
            // mutlaka tanımlanmalıdır.
            policy.SetIsOriginAllowed(_ => true);
        }

        // Credential içeren CORS isteklerinde AllowAnyOrigin kullanılamaz.
        // WithOrigins/SetIsOriginAllowed gerçek Origin değerini response'a
        // yansıtır ve tarayıcının HttpOnly refresh cookie'yi kabul etmesini sağlar.
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Swagger

// Authentication
builder.Services
    .AddAuthentication(
        JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer =
                    builder.Configuration["Token:Issuer"],

                ValidAudience =
                    builder.Configuration["Token:Audience"],

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            builder.Configuration[
                                "Token:SecurityKey"]!)),

                ClockSkew = TimeSpan.Zero,

                NameClaimType =
                    System.Security.Claims.ClaimTypes.Name,

                RoleClaimType =
                    System.Security.Claims.ClaimTypes.Role
            };

        options.Events = new JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                context.HandleResponse();

                await ApiErrorWriter.WriteAsync(
                    context.HttpContext,
                    StatusCodes.Status401Unauthorized,
                    "AUTH_UNAUTHORIZED",
                    "Oturum geçersiz veya süresi dolmuş. Yeniden giriş yapın.");
            },

            OnForbidden = async context =>
            {
                await ApiErrorWriter.WriteAsync(
                    context.HttpContext,
                    StatusCodes.Status403Forbidden,
                    "AUTH_FORBIDDEN",
                    "Bu işlem için yetkiniz bulunmuyor.");
            }
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddAutoMapper(
    typeof(MapperProfile).Assembly);

var app = builder.Build();

app.UseRequestLocalization();



app.UseExceptionHandler();

app.UseStatusCodePages(async statusCodeContext =>
{
    var httpContext = statusCodeContext.HttpContext;

    if (!httpContext.Request.Path.StartsWithSegments("/api") ||
        httpContext.Response.HasStarted ||
        !string.IsNullOrWhiteSpace(
            httpContext.Response.ContentType))
    {
        return;
    }

    var (code, message) =
        httpContext.Response.StatusCode switch
        {
            StatusCodes.Status404NotFound =>
                (
                    "RESOURCE_NOT_FOUND",
                    "İstenen kaynak bulunamadı."
                ),

            StatusCodes.Status405MethodNotAllowed =>
                (
                    "METHOD_NOT_ALLOWED",
                    "Bu kaynak için HTTP metodu desteklenmiyor."
                ),

            _ =>
                (
                    "HTTP_ERROR",
                    "İstek tamamlanamadı."
                )
        };

    await ApiErrorWriter.WriteAsync(
        httpContext,
        httpContext.Response.StatusCode,
        code,
        message);
});

// Cookie üretilmeden ve HTTPS yönlendirmesi yapılmadan önce proxy
// protokol/header bilgileri uygulanmalıdır.
app.UseForwardedHeaders();

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(corsPolicyName);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
