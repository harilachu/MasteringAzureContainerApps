using ERP.Common.Core;
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

            return services;
        }
    }
}
