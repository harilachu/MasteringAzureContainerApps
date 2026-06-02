using Azure.Identity;
using ERP.Common.Core;
using ERP.Common.Domain;
using ERP.Employees.Application.Interfaces;
using ERP.Employees.Infrastructure.Repositories;
using ERP.Infrastructure.Core;
using ERP.Infrastructure.Core.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ERP.Employees.Infrastructure.ServiceDependencies
{
    public static class CosmosServiceRegistration
    {
        public static IServiceCollection AddCosmosService(this IServiceCollection services, AppConfig appConfig)
        {
            services.AddSingleton<CosmosClient>(sp =>
            {
                var appConfig = sp.GetRequiredService<IOptions<AppConfig>>().Value;
                return InitCosmosClientAsync(appConfig);
            });

            return services;
        }

        private static CosmosClient InitCosmosClientAsync(AppConfig appConfig)
        {
            //var ManagedIdentityClientId = appConfig.ManagedIdentity;
            //var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions
            //{
            //    ManagedIdentityClientId = ManagedIdentityClientId
            //});

            //Use for Local run using Cosmos DB Emulator
            DefaultAzureCredential credential = new();

            List<string> preferredRegions = new List<string> { "East US", "West US" };
            var options = new CosmosClientOptions
            {
                ApplicationPreferredRegions = preferredRegions,
                ConnectionMode = ConnectionMode.Gateway,
                AllowBulkExecution = true,
                RequestTimeout = TimeSpan.FromSeconds(30),
                MaxRetryAttemptsOnRateLimitedRequests = 5,
                MaxRetryWaitTimeOnRateLimitedRequests = TimeSpan.FromSeconds(10)
            };

            if (appConfig.Environment == "Development")
            {
                return new CosmosClient(appConfig.Cosmos.Endpoint);
            }
            else
            {
                return new CosmosClient(appConfig.Cosmos.Endpoint, credential, options);
            }
        }

        public static IServiceCollection AddCosmosRepositories(this IServiceCollection services)
        {
            services.AddSingleton<IBaseCosmosRepository<Employee>, BaseCosmosRepository<Employee>>();

            return services;
        }

        public static IServiceCollection AddApplicationRepositories(this IServiceCollection services)
        {
            services.AddSingleton<IEmployeeRepository, EmployeeRepository>();

            return services;
        }
    }
}
