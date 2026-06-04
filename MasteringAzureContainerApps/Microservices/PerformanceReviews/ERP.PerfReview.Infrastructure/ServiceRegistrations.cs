using ERP.Common.Core;
using ERP.PerfReview.Application.Interfaces;
using ERP.PerfReview.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ERP.PerfReview.Infrastructure.ServiceDependencies
{
    public static class ServiceRegistrations
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, AppConfig appConfig)
        {
            services.AddCosmosService(appConfig)
            .AddCosmosRepositories()
            .AddApplicationRepositories();

            services.AddSingleton<IBonusService, BonusService>();
            return services;
        }
    }
}
