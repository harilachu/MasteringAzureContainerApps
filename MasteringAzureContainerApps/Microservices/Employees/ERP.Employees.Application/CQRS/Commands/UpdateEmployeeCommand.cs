using AutoMapper;
using ERP.Common.Core;
using ERP.Common.Domain;
using ERP.Common.DTO;
using ERP.Common.Interfaces;
using ERP.Employees.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Synaptrix;
using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.Employees.Application.CQRS.Commands
{
    public record UpdateEmployeeResult(string EmpId, string Name);
    public record UpdateEmployeeCommand(CreateEmployeeDto EmployeeDto) : ICommand<Result<UpdateEmployeeResult>>;
    public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, Result<UpdateEmployeeResult>>
    {
        private readonly ILogger<UpdateEmployeeCommandHandler> logger;
        private readonly IEmployeeRepository employeeRepository;
        private readonly IMapper mapper;

        public UpdateEmployeeCommandHandler(ILogger<UpdateEmployeeCommandHandler> logger, IEmployeeRepository employeeRepository, IMapper mapper)
        {
            this.logger = logger;
            this.employeeRepository = employeeRepository;
            this.mapper = mapper;
        }
        public async ValueTask<Result<UpdateEmployeeResult>> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var employeeResult = await employeeRepository.GetEmployeesByQueryAsync(request.EmployeeDto.Department, request.EmployeeDto.EmpId, cancellationToken);
                if (employeeResult == null || !employeeResult.IsSuccess || employeeResult.Value == null || employeeResult.Value.Count == 0)
                {
                    logger.LogWarning("Employee with id {EmpId} not found in the Department: {Department}", request.EmployeeDto.EmpId, request.EmployeeDto.Department);
                    return Result<UpdateEmployeeResult>.Failure(new Error("NotFound", "Employee not found"));
                }

                var employee = mapper.Map<Employee>(request.EmployeeDto);
                employee.id = employeeResult.Value[0].id; // Ensure we set the existing employee's id for the update operation
                var updateResult = await employeeRepository.UpdateEmployeeAsync(employee, employeeResult.Value[0].Department, cancellationToken);
                if (!updateResult.IsSuccess)
                {
                    logger.LogError("Failed to update employee with id {EmpId}. Error: {Error}", request.EmployeeDto.EmpId, updateResult.Error);
                    return Result<UpdateEmployeeResult>.Failure(updateResult.Error);
                }

                var result = new UpdateEmployeeResult(updateResult.Value.EmpId, updateResult.Value.EmpName);
                return Result<UpdateEmployeeResult>.Success(result);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating employee");
                return Result<UpdateEmployeeResult>.Failure(new Error("CommandFailed", "Failed to update employee"));
            }
        }
    }
}
