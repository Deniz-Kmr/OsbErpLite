using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Infrastructure.Persistence.Context;
using OsbErpLite.Infrastructure.Persistence.Repositories;

namespace OsbErpLite.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IBusinessPartnerRepository, BusinessPartnerRepository>();

        return services;
    }
}
