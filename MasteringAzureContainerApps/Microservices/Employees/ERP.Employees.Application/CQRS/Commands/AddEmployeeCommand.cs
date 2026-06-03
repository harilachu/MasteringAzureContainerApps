using AutoMapper;
using ERP.Common.Core;
using ERP.Common.Domain;
using ERP.Common.DTO;
using ERP.Common.Interfaces;
using ERP.Employees.Application.CQRS.Queries;
using ERP.Employees.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Synaptrix;

namespace ERP.Employees.Application.CQRS.Commands
{
    public record AddEmployeeResult(string EmpId, string Name);
    public record AddEmployeeCommand(CreateEmployeeDto EmployeeDto) : ICommand<Result<AddEmployeeResult>>;
    public class AddEmployeeCommandHandler : IRequestHandler<AddEmployeeCommand, Result<AddEmployeeResult>>
    {
        private readonly ILogger<AddEmployeeCommandHandler> logger;
        private readonly IEmployeeRepository employeeRepository;
        private readonly IMapper mapper;

        public AddEmployeeCommandHandler(ILogger<AddEmployeeCommandHandler> logger, IEmployeeRepository employeeRepository, IMapper mapper)
        {
            this.logger = logger;
            this.employeeRepository = employeeRepository;
            this.mapper = mapper;
        }

        public async ValueTask<Result<AddEmployeeResult>> Handle(AddEmployeeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var employeeResult = await employeeRepository.GetEmployeesByQueryAsync(request.EmployeeDto.Department, request.EmployeeDto.EmpId, cancellationToken);
                if (employeeResult != null && employeeResult.IsSuccess && employeeResult.Value != null && employeeResult.Value.Count == 1)
                {
                    logger.LogWarning("Employee with id {EmpId} is already found in the Department: {Department}", request.EmployeeDto.EmpId, request.EmployeeDto.Department);
                    return Result<AddEmployeeResult>.Failure(new Error("Found", "Employee already found"));
                }

                var employee = mapper.Map<Employee>(request.EmployeeDto);
                employee.id = Guid.NewGuid().ToString();
                var addedResult = await employeeRepository.AddEmployeeAsync(employee, cancellationToken);
                if (!addedResult.IsSuccess)
                {
                    logger.LogError("Failed to add employee with id {EmpId}. Error: {Error}", request.EmployeeDto.EmpId, addedResult.Error);
                    return Result<AddEmployeeResult>.Failure(addedResult.Error);
                }

                var result = new AddEmployeeResult(addedResult.Value.EmpId, addedResult.Value.EmpName);
                return Result<AddEmployeeResult>.Success(result);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding employee");
                return Result<AddEmployeeResult>.Failure(new Error("CommandFailed", "Failed to add employee"));
            }
        }
    }
}
