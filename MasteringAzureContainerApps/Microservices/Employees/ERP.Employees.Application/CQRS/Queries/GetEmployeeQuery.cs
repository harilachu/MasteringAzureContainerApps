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
    public record GetEmployeeResult(EmployeeDto Employee);
    public record GetEmployeeQuery(string Id, string Department) : IQuery<Result<GetEmployeeResult>>;
    public class GetEmployeeQueryHandler : IRequestHandler<GetEmployeeQuery, Result<GetEmployeeResult>>
    {
        private readonly ILogger<GetEmployeeQueryHandler> logger;
        private readonly IEmployeeRepository employeeRepository;
        private readonly IMapper mapper;

        public GetEmployeeQueryHandler(ILogger<GetEmployeeQueryHandler> logger, IEmployeeRepository employeeRepository, IMapper mapper)
        {
            this.logger = logger;
            this.employeeRepository = employeeRepository;
            this.mapper = mapper;
        }

        public async ValueTask<Result<GetEmployeeResult>> Handle(GetEmployeeQuery request, CancellationToken cancellationToken)
        {
            var result = await employeeRepository.GetEmployeeAsync(request.Id, request.Department, cancellationToken);

            if (!result.IsSuccess || result.Value == null) {
                logger.LogError("Failed to get employee with id {EmployeeId} and department {Department}. Error: {Error}", request.Id, request.Department, result.Error);
                return Result<GetEmployeeResult>.Failure(result.Error);
            }

            var employee = result.Value;
            var employeeDto = mapper.Map<EmployeeDto>(employee);
            return Result<GetEmployeeResult>.Success(new GetEmployeeResult(employeeDto));
        }
    }
}
