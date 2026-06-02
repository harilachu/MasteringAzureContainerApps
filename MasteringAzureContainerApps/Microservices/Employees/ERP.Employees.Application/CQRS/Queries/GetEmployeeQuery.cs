using ERP.Common.Core;
using ERP.Common.DTO;
using ERP.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Synaptrix;
using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.Employees.Application.CQRS.Queries
{
    public record GetEmployeeResult(EmployeeDto Employee);
    public record GetEmployeeQuery(int EmpId) : IQuery<Result<GetEmployeeResult>>;
    public class GetEmployeeQueryHandler : IRequestHandler<GetEmployeeQuery, Result<GetEmployeeResult>>
    {
        private readonly ILogger<GetEmployeeQueryHandler> logger;

        public GetEmployeeQueryHandler(ILogger<GetEmployeeQueryHandler> logger)
        {
            this.logger = logger;
        }

        public async ValueTask<Result<GetEmployeeResult>> Handle(GetEmployeeQuery request, CancellationToken cancellationToken)
        {
            
            return Result<GetEmployeeResult>.Success(new GetEmployeeResult(new EmployeeDto
            {
                EmpId = request.EmpId,
                EmpName = "John Doe",
            }));
        }
    }
}
