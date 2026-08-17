namespace OsbErpLite.Api.Extensions;

public static class CorsServiceExtensions
{
    public const string FrontendPolicyName = "AllowFrontend";

    public static IServiceCollection AddApiCors(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var allowedOrigins = configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>() ?? [];

        services.AddCors(options =>
        {
            options.AddPolicy(FrontendPolicyName, policy =>
            {
                policy
                    .WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .WithMethods("GET", "POST", "PUT", "DELETE")
                    .AllowCredentials();
            });
        });

        return services;
    }
}
