using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Health;

namespace YarpGatewayService
{
    public static class ReverseProxyConfigurationExtensions
    {
        public static void AddReverseProxyForYarp(this IServiceCollection services)
        {
            //Set the Environment variables in the YARP Container App for the downstream APIs of the microservices
            string employeeAppBaseAddress = Environment.GetEnvironmentVariable("EMPLOYEE_APP_BASE_ADDRESS") ?? "http://localhost:5252";
            string reviewAppBaseAddress = Environment.GetEnvironmentVariable("REVIEW_APP_BASE_ADDRESS") ?? "http://localhost:5052";


            services.AddReverseProxy()
             .LoadFromMemory(
                routes: new[]
                {
                    //Add Route Config for every controller in the downstream API.
                    new RouteConfig
                    {
                        RouteId = "employee-Route",
                        ClusterId = "employee-Cluster",
                        Match = new RouteMatch
                        {
                            Path = "/api/Employees/{**catch-all}"
                        },
                        AuthorizationPolicy = "JWTAuthPolicy",
                        Transforms = new []
                        {
                            new Dictionary<string, string>
                            {
                                { "PathRemovePrefix", "/api" }
                            }
                        }
                    },
                    new RouteConfig
                    {
                        RouteId = "review-Route",
                        ClusterId = "review-Cluster",
                        Match = new RouteMatch
                        {
                            Path = "/api/Review/{**catch-all}"
                        },
                        AuthorizationPolicy = "JWTAuthPolicy",
                        Transforms = new []
                        {
                            new Dictionary<string, string>
                            {
                                { "PathRemovePrefix", "/api" }
                            }
                        }
                    }
                },
                clusters: new[]
                {
                    new ClusterConfig
                    {
                        ClusterId = "employee-Cluster",
                        Destinations = new Dictionary<string, DestinationConfig>
                        {
                            { "employee-App-Destination", new DestinationConfig { Address = employeeAppBaseAddress } }
                        },
                        HealthCheck = new HealthCheckConfig
                        {
                            Active = new ActiveHealthCheckConfig
                            {
                                Enabled = true,
                                Interval = TimeSpan.FromSeconds(15),
                                Timeout = TimeSpan.FromSeconds(10),
                                Path = "/health", //Health check endpoint in the downstream API
                                Policy = HealthCheckConstants.ActivePolicy.ConsecutiveFailures //built-in policy that marks the destination as unhealthy after a specified number of consecutive failures and healthy after a specified number of consecutive successes.
                            },
                            Passive = new PassiveHealthCheckConfig
                            {
                                Enabled = true,
                                ReactivationPeriod = TimeSpan.FromSeconds(10),
                                Policy = HealthCheckConstants.PassivePolicy.TransportFailureRate //built-in policy that marks the destination as unhealthy if the rate of failed requests exceeds a specified threshold and healthy if the rate of successful requests exceeds a specified threshold.
                            }
                        },
                        Metadata = new Dictionary<string, string>
                        {
                            { ConsecutiveFailuresHealthPolicyOptions.ThresholdMetadataName, "5" }, //Custom metadata to set the threshold for consecutive failures in the active health check policy.
                            { TransportFailureRateHealthPolicyOptions.FailureRateLimitMetadataName, "0.3" } //Custom metadata to set the failure rate limit for the passive health check policy.
                        }
                    },
                     new ClusterConfig
                    {
                        ClusterId = "review-Cluster",
                        Destinations = new Dictionary<string, DestinationConfig>
                        {
                            { "review-App-Destination", new DestinationConfig { Address = reviewAppBaseAddress } }
                        },
                        HealthCheck = new HealthCheckConfig
                        {
                            Active = new ActiveHealthCheckConfig
                            {
                                Enabled = true,
                                Interval = TimeSpan.FromSeconds(15),
                                Timeout = TimeSpan.FromSeconds(10),
                                Path = "/health", //Health check endpoint in the downstream API
                                Policy = HealthCheckConstants.ActivePolicy.ConsecutiveFailures //built-in policy that marks the destination as unhealthy after a specified number of consecutive failures and healthy after a specified number of consecutive successes.
                            },
                            Passive = new PassiveHealthCheckConfig
                            {
                                Enabled = true,
                                ReactivationPeriod = TimeSpan.FromSeconds(10),
                                Policy = HealthCheckConstants.PassivePolicy.TransportFailureRate //built-in policy that marks the destination as unhealthy if the rate of failed requests exceeds a specified threshold and healthy if the rate of successful requests exceeds a specified threshold.
                            }
                        },
                        Metadata = new Dictionary<string, string>
                        {
                            { ConsecutiveFailuresHealthPolicyOptions.ThresholdMetadataName, "5" }, //Custom metadata to set the threshold for consecutive failures in the active health check policy.
                            { TransportFailureRateHealthPolicyOptions.FailureRateLimitMetadataName, "0.3" } //Custom metadata to set the failure rate limit for the passive health check policy.
                        }
                    }
                });
        }
    }
}
