using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.Common.Core
{
    public class AppConfig
    {
        public string Environment { get; set; }
        public string ManagedIdentity { get; set; }
        public CosmosConfig Cosmos { get; set; }

        public string EmployeeDaprAppId { get; set; }

    }   

    public class CosmosConfig
    {
        public string Endpoint { get; set; }
        public string DatabaseName { get; set; }
        public string EmployeesContainerName { get; set; }
        public string PerformanceReviewsContainerName { get; set; }
        public string ProjectsContainerName { get; set; }

    }
}
