using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Infrastructure.Identity;
using OsbErpLite.Infrastructure.Persistence.Context;
using OsbErpLite.Infrastructure.Persistence.Repositories;

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

        services.AddSingleton<IJwtTokenService>(
            new JwtTokenService(configuration, jwtSecretKey));

        return services;
    }
}
