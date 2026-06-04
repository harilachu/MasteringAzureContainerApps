using ERP.Common.Core;
using ERP.Common.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.PerfReview.Application.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<Result<Employee>> GetEmployeeAsync(string id, string department, CancellationToken cancellationToken);
        Task<Result<List<Employee>>> GetEmployeesByQueryAsync(string department, string empId = "", CancellationToken cancellationToken = default);
        Task<Result<Employee>> AddEmployeeAsync(Employee employee, CancellationToken cancellationToken);
        Task<Result<Employee>> UpdateEmployeeAsync(Employee employee, string department, CancellationToken cancellationToken);
        Task<Result<Employee>> DeleteEmployeeAsync(string id, string department, CancellationToken cancellationToken);
    }
}
