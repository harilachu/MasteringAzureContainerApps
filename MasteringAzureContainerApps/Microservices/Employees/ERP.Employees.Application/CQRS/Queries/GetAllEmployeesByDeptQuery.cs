using AutoMapper;
using ERP.Common.Core;
using ERP.Common.DTO;
using ERP.Common.Interfaces;
using ERP.Employees.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Synaptrix;
using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.Employees.Application.CQRS.Queries
{
    public record GetAllEmployeesByDeptResult(List<EmployeeDto> Employee);
    public record GetAllEmployeesByDeptQuery(string Department) : IQuery<Result<GetAllEmployeesByDeptResult>>;
    public class GetAllEmployeesByDeptQueryHandler : IRequestHandler<GetAllEmployeesByDeptQuery, Result<GetAllEmployeesByDeptResult>>
    {
        private readonly ILogger<GetAllEmployeesByDeptQueryHandler> logger;
        private readonly IEmployeeRepository employeeRepository;
        private readonly IMapper mapper;

        public GetAllEmployeesByDeptQueryHandler(ILogger<GetAllEmployeesByDeptQueryHandler> logger, IEmployeeRepository employeeRepository, IMapper mapper)
        {
            this.logger = logger;
            this.employeeRepository = employeeRepository;
            this.mapper = mapper;
        }

        public async ValueTask<Result<GetAllEmployeesByDeptResult>> Handle(GetAllEmployeesByDeptQuery request, CancellationToken cancellationToken)
        {
            var result = await employeeRepository.GetEmployeesByQueryAsync( request.Department, "", cancellationToken);

            if(result.IsSuccess && result.Value != null && result.Value.Count > 0)
            {
                var employees = result.Value; // Assuming EmpId is unique and returns only one employee
                var employeeDtos = mapper.Map<List<EmployeeDto>>(employees);
                return Result<GetAllEmployeesByDeptResult>.Success(new GetAllEmployeesByDeptResult(employeeDtos));
            }
            if(result.IsSuccess && result.Value != null && result.Value.Count == 0)
            {
                logger.LogWarning("No Employees found in department {Department} or Department not Found.", request.Department);
                return Result<GetAllEmployeesByDeptResult>.Failure(new Error("NotFound", $"No Employees found in department {request.Department} or Department not Found."));
            }
            
            logger.LogError("Failed to get employees with and department {Department}. Error: {Error}", request.Department, result.Error);
            return Result<GetAllEmployeesByDeptResult>.Failure(result.Error);
        }
    }
}
