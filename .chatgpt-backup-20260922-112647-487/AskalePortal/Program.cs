using AskalePortal.API.Features.ChatHistory;
using AskalePortal.API.Extensions;
using AskalePortal.API.Infrastructure.Errors;
using AskalePortal.API.Infrastructure.Serialization;
using AskalePortal.API.Mapper;
using AskalePortal.API.Security.Auth;
using AskalePortal.API.Security.Auth.PasswordRecovery;
using AskalePortal.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using AskalePortal.API.Security.Auth.Cleanup;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
// Optional local secret; environment and command-line settings take precedence.
builder.Configuration.AddJsonFile("appsettings.OpenAI.local.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables().AddCommandLine(args);
builder.Services.Configure<AskalePortal.API.Services.ChatGpt.OpenAiChatOptions>(
    builder.Configuration.GetSection("OpenAI"));
builder.Services.AddHttpClient<AskalePortal.API.Services.ChatGpt.OpenAiResponsesService>(
    client => client.Timeout = TimeSpan.FromSeconds(90));
builder.Services.AddChatHistory(builder.Configuration, builder.Environment);

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
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});
// Reverse proxy / IIS üzerinden gelen protokol bilgisini kullan.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto;

    // IIS, Nginx veya başka bir reverse proxy arkasında çalışırken
    // proxy adresleri ortam bazında sınırlandırılabiliyorsa
    // KnownProxies / KnownNetworks kullanılmalıdır.
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// JWT ayarları
builder.Services
    .AddOptions<JwtOptions>()
    .Bind(builder.Configuration.GetSection(JwtOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<PasswordRecoveryService>();
builder.Services.AddScoped<PasswordRecoveryEmailFactory>();
builder.Services.AddOptions<PasswordRecoveryOptions>()
    .Bind(builder.Configuration.GetSection("PasswordRecovery"));
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("chat-gpt", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.User.FindFirst("userId")?.Value ?? context.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
            _ => new FixedWindowRateLimiterOptions {
                PermitLimit = 6, Window = TimeSpan.FromMinutes(1), QueueLimit = 0
            }));
    // Shared per-instance limit also bounds anonymous attempts with random usernames/IPs.
    options.AddFixedWindowLimiter("password-recovery", limiter =>
    {
        limiter.PermitLimit = 30;
        limiter.Window = TimeSpan.FromMinutes(1);
        limiter.QueueLimit = 0;
    });
    options.OnRejected = async (context, ct) =>
    {
        if (context.HttpContext.Request.Path.StartsWithSegments("/api/chatgpt"))
        {
            context.HttpContext.Response.Headers.RetryAfter = "60";
            await ApiErrorWriter.WriteAsync(context.HttpContext, 429, "CHAT_RATE_LIMITED",
                "Çok fazla mesaj gönderildi. Bir dakika sonra tekrar deneyin.", cancellationToken: ct);
            return;
        }
        context.HttpContext.Response.Headers.RetryAfter = "60";
        await ApiErrorWriter.WriteAsync(context.HttpContext, 429, "AUTH_RECOVERY_RATE_LIMITED",
            "Çok fazla deneme yapıldı. Lütfen bir dakika sonra tekrar deneyin.");
    };
});

// Refresh token cleanup ayarları
builder.Services
    .AddOptions<RefreshTokenCleanupOptions>()
    .Bind(
        builder.Configuration.GetSection(
            RefreshTokenCleanupOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

var refreshTokenCleanupEnabled =
    builder.Configuration.GetValue<bool>(
        $"{RefreshTokenCleanupOptions.SectionName}:Enabled");

if (refreshTokenCleanupEnabled)
{
    builder.Services.AddHostedService<RefreshTokenCleanupService>();
}

// Controller ve JSON ayarları
builder.Services
    .AddControllers(options =>
    {
        options.Filters.Add<PaginationResultFilter>();
        options.Filters.Add<DetachedEntityResultFilter>();
    })
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
        new List<CultureInfo>
        {
            cultureInfo
        };

    options.SupportedUICultures =
        new List<CultureInfo>
        {
            cultureInfo
        };
});

// CORS
const string corsPolicyName = "CorsPolicy";

var allowedOrigins =
    builder.Configuration
        .GetSection("Cors:AllowedOrigins")
        .Get<string[]>()
    ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicyName, policy =>
    {
        if (allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins);
        }
        else if (builder.Environment.IsDevelopment())
        {
            // Yalnızca geliştirme ortamı için.
            policy.SetIsOriginAllowed(_ => true);
        }
        else
        {
            throw new InvalidOperationException(
                "Canlı ortamda Cors:AllowedOrigins tanımlanmalıdır.");
        }

        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Swagger ayarlarınız varsa bu alanda kalmalı.
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

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
            OnTokenValidated = async context =>
            {
                var idText = context.Principal?.FindFirst("userId")?.Value;
                var version = context.Principal?.FindFirst("cv")?.Value;
                if (!int.TryParse(idText, out var userId) || string.IsNullOrWhiteSpace(version))
                {
                    context.Fail("Credential version missing.");
                    return;
                }
                var db = context.HttpContext.RequestServices.GetRequiredService<DBDataContext>();
                var credential = await db.AdminUser.AsNoTracking().Where(x => x.Id == userId && x.enabled)
                    .Select(x => x.password).SingleOrDefaultAsync(context.HttpContext.RequestAborted);
                if (credential is null || version != CredentialVersion.Create(userId, credential,
                    builder.Configuration["Token:SecurityKey"]!))
                    context.Fail("Credentials changed.");
            },
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

// Swagger middleware ayarlarınız varsa burada kalmalı.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

// Proxy bilgisinin HTTPS yönlendirmesinden önce uygulanması gerekir.
app.UseForwardedHeaders();

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

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(corsPolicyName);
app.UseAuthentication();
app.UseRateLimiter();
app.UseAuthorization();

app.MapControllers();

app.Run();

