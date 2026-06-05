
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
    public record GetSummaryResult(List<EmployeeSummaryDto> EmployeeSummaryInfo);
    public record GetSummaryQuery(string Department) : IQuery<Result<GetSummaryResult>>;
    public class GetSummaryQueryHandler : IRequestHandler<GetSummaryQuery, Result<GetSummaryResult>>
    {
        private readonly ILogger<GetSummaryQueryHandler> logger;
        private readonly IProjectsRepository projectsRepository;
        private readonly IEmployeeRepository employeeRepository;
        private readonly IBonusService bonusService;
        private readonly IMapper mapper;
        private readonly DaprClient daprClient;
        private readonly IHttpClientFactory httpClientFactory;

        public GetSummaryQueryHandler(ILogger<GetSummaryQueryHandler> logger, IProjectsRepository projectsRepository, IEmployeeRepository employeeRepository, 
            IBonusService bonusService, IMapper mapper, DaprClient daprClient, IHttpClientFactory httpClientFactory)
        {
            this.logger = logger;
            this.projectsRepository = projectsRepository;
            this.employeeRepository = employeeRepository;
            this.bonusService = bonusService;
            this.mapper = mapper;
            this.daprClient = daprClient;
            this.httpClientFactory = httpClientFactory;
        }

        public async ValueTask<Result<GetSummaryResult>> Handle(GetSummaryQuery request, CancellationToken cancellationToken)
        {
            var employeeDto = new List<EmployeeDto>();
            try
            {
#if DAPRCLIENT
                employeeDto = await daprClient.InvokeMethodAsync<List<EmployeeDto>>(HttpMethod.Get, "employee-app", $"Employees/All?department={request.Department}", cancellationToken);
#else
                var httpclient = httpClientFactory.CreateClient("employee-app");
                httpclient.DefaultRequestHeaders.Add("X-Request-ID", Guid.NewGuid().ToString());
                employeeDto = await httpclient.GetFromJsonAsync<List<EmployeeDto>>($"Employees/All?department={request.Department}", cancellationToken);
#endif
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while invoking employee dapr service for employees within department {Department}.",  request.Department);
                return Result<GetSummaryResult>.Failure(new Error("EmployeeDaprServiceError", $"Failed to retrieve employee details for employees in department {request.Department}."));
            }

            var summaries = new List<EmployeeSummaryDto>();
            var employees = mapper.Map<List<Employee>>(employeeDto);

            foreach (var e in employees)
            {
                var summary = new EmployeeSummaryDto
                {
                    EmployeeId = e.EmpId,
                    FullName = e.EmpName,
                    AverageScore = await bonusService.GetAverageScoreAsync(e, cancellationToken),
                    ProjectsCompleted = await projectsRepository.QueryProjectsCount(e.EmpId, cancellationToken),
                    BonusAmount = await bonusService.CalculateBonusAsync(e, null, cancellationToken)
                };

                summaries.Add(summary);
            }

            return Result<GetSummaryResult>.Success(new GetSummaryResult(summaries));
        }
    }
}
