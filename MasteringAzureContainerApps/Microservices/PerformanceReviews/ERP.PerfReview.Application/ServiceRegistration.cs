using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.PerfReview.Application
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg =>
            {//optional config
            }, typeof(MappingProfile).Assembly);

            //Dapr client registration for inter-service communication
            services.AddDaprClient();

            services.AddHttpClient("employee-app", client =>
            {
                // Base address can be set to the employee dapr service URL
                client.BaseAddress = new Uri("http://localhost:3500/v1.0/invoke/employee-app/method/");
            });
            return services;
        }
    }
}
