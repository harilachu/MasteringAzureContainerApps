using AutoMapper;
using ERP.Common.Core;
using ERP.Common.Domain;
using ERP.PerfReview.Application.Interfaces;
using ERP.Infrastructure.Core;
using ERP.Infrastructure.Core.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace ERP.PerfReview.Infrastructure.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly IBaseCosmosRepository<Employee> cosmosRepository;
        private readonly ILogger<EmployeeRepository> logger;
        private readonly IMapper mapper;

        public EmployeeRepository(IBaseCosmosRepository<Employee> cosmosRepository, ILogger<EmployeeRepository> logger, IMapper mapper)
        {
            this.cosmosRepository = cosmosRepository;
            this.logger = logger;
            this.mapper = mapper;
        }
        public async Task<Result<Employee>> AddEmployeeAsync(Employee employee, CancellationToken cancellationToken)
        {
            return await cosmosRepository.AddItemAsync(employee, employee.Department, cancellationToken);
        }

        public async Task<Result<Employee>> DeleteEmployeeAsync(string id, string department, CancellationToken cancellationToken)
        {
            return await cosmosRepository.DeleteItemAsync(id, department, cancellationToken);
        }

        public async Task<Result<Employee>> GetEmployeeAsync(string id, string department, CancellationToken cancellationToken)
        {
            return await cosmosRepository.GetItemAsync<Employee>(id, department, cancellationToken);
        }

        public async Task<Result<List<Employee>>> GetEmployeesByQueryAsync(string department, string empId = "", CancellationToken cancellationToken = default)
        {
            var queryDefinition = new QueryDefinition(InfraConstants.GET_EMPLOYEE_BY_EMPID_QUERY);
            if(!string.IsNullOrEmpty(empId))
            {
                queryDefinition.WithParameter("@empId", empId);
            }
            if(!string.IsNullOrEmpty(department) && string.IsNullOrEmpty(empId))
            {
                queryDefinition = new QueryDefinition(InfraConstants.GET_EMPLOYEES_BY_DEPT_QUERY);
                queryDefinition.WithParameter("@partitionKey", department);
            }
            else if(!string.IsNullOrEmpty(department))
            {
                queryDefinition.WithParameter("@partitionKey", department);
            }

            return await cosmosRepository.GetItemsByQueryAsync<Employee>(queryDefinition, cancellationToken);
        }

        public async Task<Result<Employee>> UpdateEmployeeAsync(Employee employee, string department, CancellationToken cancellationToken)
        {
            return await cosmosRepository.UpdateItemAsync(employee, employee.Department, cancellationToken);
        }
    }
}
