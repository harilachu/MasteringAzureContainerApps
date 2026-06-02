using ERP.Common.Core;
using ERP.Common.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.Employees.Application.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<Result<Employee>> GetEmployeeAsync(string id, string department, CancellationToken cancellationToken);
        Task<Result<List<Employee>>> GetEmployeesByQueryAsync(string department, string empId = "", CancellationToken cancellationToken = default);
        Task<Result> AddEmployeeAsync(Employee employee, CancellationToken cancellationToken);
        Task<Result> UpdateEmployeeAsync(string id, Employee employee, CancellationToken cancellationToken);
        Task<Result> DeleteEmployeeAsync(string id, string department, CancellationToken cancellationToken);
    }
}
