using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Infrastructure.ExternalServices.Sap;
using OsbErpLite.Infrastructure.Identity;
using OsbErpLite.Infrastructure.Persistence.Context;
using OsbErpLite.Infrastructure.Persistence.Repositories;
using Polly;

namespace OsbErpLite.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        string connectionString,
        IConfiguration configuration,
        string jwtSecretKey)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IBusinessPartnerRepository, BusinessPartnerRepository>();
        services.AddScoped<IWarehouseRepository, WarehouseRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IStockMovementRepository, StockMovementRepository>();
        services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
        services.AddScoped<ISalesOrderRepository, SalesOrderRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IPositionRepository, PositionRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<ILeaveRequestRepository, LeaveRequestRepository>();

        services.AddSingleton<IJwtTokenService>(
            new JwtTokenService(configuration, jwtSecretKey));

        var sapBaseUrl = Environment.GetEnvironmentVariable("SAP_API_BASE_URL")
            ?? throw new InvalidOperationException("SAP_API_BASE_URL environment variable is not set.");
        var sapApiKey = Environment.GetEnvironmentVariable("SAP_API_KEY")
            ?? throw new InvalidOperationException("SAP_API_KEY environment variable is not set.");

        services.AddHttpClient<ISapBusinessPartnerClient, SapBusinessPartnerClient>(client =>
        {
            client.BaseAddress = new Uri(sapBaseUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("APIKey", sapApiKey);
            client.Timeout = TimeSpan.FromSeconds(30);
        })
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
        })
        .AddTransientHttpErrorPolicy(policy =>
            policy.WaitAndRetryAsync(3, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));

        return services;
    }
}
