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
    public record GetEmployeeByEmpIdResult(EmployeeDto Employee);
    public record GetEmployeeByEmpIdQuery(string EmpId, string Department) : IQuery<Result<GetEmployeeByEmpIdResult>>;
    public class GetEmployeeByEmpIdQueryHandler : IRequestHandler<GetEmployeeByEmpIdQuery, Result<GetEmployeeByEmpIdResult>>
    {
        private readonly ILogger<GetEmployeeByEmpIdQueryHandler> logger;
        private readonly IEmployeeRepository employeeRepository;
        private readonly IMapper mapper;

        public GetEmployeeByEmpIdQueryHandler(ILogger<GetEmployeeByEmpIdQueryHandler> logger, IEmployeeRepository employeeRepository, IMapper mapper)
        {
            this.logger = logger;
            this.employeeRepository = employeeRepository;
            this.mapper = mapper;
        }

        public async ValueTask<Result<GetEmployeeByEmpIdResult>> Handle(GetEmployeeByEmpIdQuery request, CancellationToken cancellationToken)
        {
            var result = await employeeRepository.GetEmployeesByQueryAsync(request.Department, request.EmpId, cancellationToken);

            if(result.IsSuccess && result.Value != null && result.Value.Count > 0)
            {
                var employee = result.Value[0]; // Assuming EmpId is unique and returns only one employee
                var employeeDto = mapper.Map<EmployeeDto>(employee);
                return Result<GetEmployeeByEmpIdResult>.Success(new GetEmployeeByEmpIdResult(employeeDto));
            }

            
            logger.LogError("Failed to get employee with EmpId {EmployeeId} and department {Department}. Error: {Error}", request.EmpId, request.Department, result.Error);
            return Result<GetEmployeeByEmpIdResult>.Failure(result.Error);
        }
    }
}
