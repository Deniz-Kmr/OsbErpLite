using System.Security.Claims;
using System.Text.Json;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using OsbErpLite.Api.Common;

namespace OsbErpLite.Api.Extensions;

public static class RateLimitingServiceExtensions
{
    public const string AuthPolicyName = "auth";
    public const string PasswordChangePolicyName = "password-change";
    public const string SapSyncPolicyName = "sap-sync";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static IServiceCollection AddApiRateLimiting(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var isEnabled = configuration.GetValue("RateLimiting:Enabled", true);

        if (!isEnabled)
            return services;

        var authPermitLimit = GetRequiredPositiveInt(configuration, "RateLimiting:Auth:PermitLimit");
        var authWindowSeconds = GetRequiredPositiveInt(configuration, "RateLimiting:Auth:WindowSeconds");

        var passwordPermitLimit = GetRequiredPositiveInt(configuration, "RateLimiting:PasswordChange:PermitLimit");
        var passwordWindowSeconds = GetRequiredPositiveInt(configuration, "RateLimiting:PasswordChange:WindowSeconds");

        var sapPermitLimit = GetRequiredPositiveInt(configuration, "RateLimiting:SapSync:PermitLimit");
        var sapWindowSeconds = GetRequiredPositiveInt(configuration, "RateLimiting:SapSync:WindowSeconds");

        var globalPermitLimit = GetRequiredPositiveInt(configuration, "RateLimiting:Global:PermitLimit");
        var globalWindowSeconds = GetRequiredPositiveInt(configuration, "RateLimiting:Global:WindowSeconds");

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.OnRejected = async (context, cancellationToken) =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<RateLimiterOptions>>();
                var endpoint = context.HttpContext.Request.Path;
                var ip = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                var userId = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                logger.LogWarning(
                    "Rate limit asildi. IP: {ClientIp}, UserId: {UserId}, Endpoint: {Endpoint}",
                    ip, userId ?? "anonymous", endpoint);

                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.HttpContext.Response.ContentType = "application/json";

                var response = ApiResponse<object>.Fail(
                    "Çok fazla istek gönderildi, lütfen bir süre sonra tekrar deneyin.");

                await context.HttpContext.Response.WriteAsync(
                    JsonSerializer.Serialize(response, JsonOptions), cancellationToken);
            };

            // IP bazli — login, register, refresh-token
            options.AddPolicy(AuthPolicyName, httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: GetClientIp(httpContext),
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = authPermitLimit,
                        Window = TimeSpan.FromSeconds(authWindowSeconds),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0,
                        AutoReplenishment = true
                    }));

            // UserId bazli — sifre degistirme islemleri
            options.AddPolicy(PasswordChangePolicyName, httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: GetUserIdOrIp(httpContext),
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = passwordPermitLimit,
                        Window = TimeSpan.FromSeconds(passwordWindowSeconds),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0,
                        AutoReplenishment = true
                    }));

            // IP bazli — SAP sync (maliyetli dis servis cagrisi)
            options.AddPolicy(SapSyncPolicyName, httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: GetClientIp(httpContext),
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = sapPermitLimit,
                        Window = TimeSpan.FromSeconds(sapWindowSeconds),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0,
                        AutoReplenishment = true
                    }));

            // Global limiter — tum endpoint'ler icin IP bazli
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: GetClientIp(context),
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = globalPermitLimit,
                        Window = TimeSpan.FromSeconds(globalWindowSeconds),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0,
                        AutoReplenishment = true
                    }));
        });

        return services;
    }

    public static IApplicationBuilder UseApiRateLimiting(
        this IApplicationBuilder app,
        IConfiguration configuration)
    {
        var isEnabled = configuration.GetValue("RateLimiting:Enabled", true);

        if (!isEnabled)
            return app;

        app.UseRateLimiter();

        return app;
    }

    private static int GetRequiredPositiveInt(IConfiguration configuration, string key)
    {
        var value = configuration.GetValue<int>(key);

        if (value <= 0)
            throw new InvalidOperationException(
                $"'{key}' must be greater than 0 when rate limiting is enabled.");

        return value;
    }

    private static string GetClientIp(HttpContext httpContext)
    {
        return httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }

    private static string GetUserIdOrIp(HttpContext httpContext)
    {
        var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        return userId ?? GetClientIp(httpContext);
    }
}
