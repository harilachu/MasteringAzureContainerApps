using AutoMapper;
using ERP.Common.Core;
using ERP.Common.Domain;
using ERP.Common.Interfaces;
using ERP.Employees.Application.CQRS.Queries;
using ERP.Employees.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Synaptrix;
using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.Employees.Application.CQRS.Commands
{
    public record DeleteEmployeeResult(string EmpId, string Department);
    public record DeleteEmployeeCommand(string EmpId, string Department) : ICommand<Result<DeleteEmployeeResult>>;
    public class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand, Result<DeleteEmployeeResult>>
    {
        private readonly ILogger<DeleteEmployeeCommandHandler> logger;
        private readonly IEmployeeRepository employeeRepository;
        private readonly IMapper mapper;

        public DeleteEmployeeCommandHandler(ILogger<DeleteEmployeeCommandHandler> logger, IEmployeeRepository employeeRepository, IMapper mapper)
        {
            this.logger = logger;
            this.employeeRepository = employeeRepository;
            this.mapper = mapper;
        }

        public async ValueTask<Result<DeleteEmployeeResult>> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var employeeResult = await employeeRepository.GetEmployeesByQueryAsync(request.Department, request.EmpId, cancellationToken);
                if (employeeResult == null || !employeeResult.IsSuccess || employeeResult.Value == null || employeeResult.Value.Count == 0)
                {
                    logger.LogWarning("Employee with id {EmpId} not found in the Department: {Department}", request.EmpId, request.Department);
                    return Result<DeleteEmployeeResult>.Failure(new Error("NotFound", "Employee not found"));
                }

                var deletedResult = await employeeRepository.DeleteEmployeeAsync(employeeResult.Value[0].id, request.Department, cancellationToken);
                if (!deletedResult.IsSuccess)
                {
                    logger.LogError("Failed to delete employee with id {EmpId}. Error: {Error}", request.EmpId, deletedResult.Error);
                    return Result<DeleteEmployeeResult>.Failure(deletedResult.Error);
                }

                var result = new DeleteEmployeeResult(request.EmpId, request.Department);
                return Result<DeleteEmployeeResult>.Success(result);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting employee");
                return Result<DeleteEmployeeResult>.Failure(new Error("CommandFailed", "Failed to delete employee"));
            }
        }
    }
}
