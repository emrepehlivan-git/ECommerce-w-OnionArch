using ECommerce.Application.Common.Interfaces;
using ECommerce.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services)
    {
        services.AddScoped<IJwtService, JwtService>();

        return services;
    }
}