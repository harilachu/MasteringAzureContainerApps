#define DAPRCLIENT

using AutoMapper;
using Dapr.Client;
using ERP.Common.Core;
using ERP.Common.Domain;
using ERP.Common.DTO;
using ERP.Common.Interfaces;
using ERP.PerfReview.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Synaptrix;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;


namespace ERP.PerfReview.Application.CQRS.Queries
{
    public record GetBonusResult(BonusDto BonusInfo);
    public record GetBonusQuery(string EmpId, string Department) : IQuery<Result<GetBonusResult>>;
    public class GetBonusQueryHandler : IRequestHandler<GetBonusQuery, Result<GetBonusResult>>
    {
        private readonly ILogger<GetBonusQueryHandler> logger;
        private readonly IEmployeeRepository employeeRepository;
        private readonly IBonusService bonusService;
        private readonly IMapper mapper;
        private readonly DaprClient daprClient;
        private readonly IHttpClientFactory httpClientFactory;

        public GetBonusQueryHandler(ILogger<GetBonusQueryHandler> logger, IEmployeeRepository employeeRepository, IBonusService bonusService, IMapper mapper, DaprClient daprClient, IHttpClientFactory httpClientFactory)
        {
            this.logger = logger;
            this.employeeRepository = employeeRepository;
            this.bonusService = bonusService;
            this.mapper = mapper;
            this.daprClient = daprClient;
            this.httpClientFactory = httpClientFactory;
        }

        public async ValueTask<Result<GetBonusResult>> Handle(GetBonusQuery request, CancellationToken cancellationToken)
        {
            var employeeDto = new EmployeeDto();
            try
            {
#if DAPRCLIENT
                employeeDto = await daprClient.InvokeMethodAsync<EmployeeDto>(HttpMethod.Get, "employee-app", $"Employees?empId={request.EmpId}&department={request.Department}", cancellationToken);
#else
                var httpclient = httpClientFactory.CreateClient("employee-app");
                httpclient.DefaultRequestHeaders.Add("X-Request-ID", Guid.NewGuid().ToString());
                employeeDto = await httpclient.GetFromJsonAsync<EmployeeDto>($"Employees?empId={request.EmpId}&department={request.Department}", cancellationToken);
#endif
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while invoking employee dapr service for employee with id {EmployeeId} in department {Department}.", request.EmpId, request.Department);
                return Result<GetBonusResult>.Failure(new Error("EmployeeDaprServiceError", $"Failed to retrieve employee details for employee with id {request.EmpId} in department {request.Department}."));
            }

            var employee = mapper.Map<Employee>(employeeDto);
            var avgScore = await bonusService.GetAverageScoreAsync(employee, cancellationToken);
            var bonusDto = new BonusDto
            {
                FullName = employee.EmpName,
                AverageScore = avgScore,
                BonusAmount = await bonusService.CalculateBonusAsync(employee, avgScore, cancellationToken)
            };

            return Result<GetBonusResult>.Success(new GetBonusResult(bonusDto));
        }
    }
}
