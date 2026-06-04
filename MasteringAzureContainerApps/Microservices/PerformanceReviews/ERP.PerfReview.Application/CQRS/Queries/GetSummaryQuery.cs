using AutoMapper;
using ERP.Common.Core;
using ERP.Common.Domain;
using ERP.Common.DTO;
using ERP.Common.Interfaces;
using ERP.PerfReview.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Synaptrix;
using System;
using System.Collections.Generic;
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

        public GetSummaryQueryHandler(ILogger<GetSummaryQueryHandler> logger, IProjectsRepository projectsRepository, IEmployeeRepository employeeRepository, IBonusService bonusService, IMapper mapper)
        {
            this.logger = logger;
            this.projectsRepository = projectsRepository;
            this.employeeRepository = employeeRepository;
            this.bonusService = bonusService;
            this.mapper = mapper;
        }

        public async ValueTask<Result<GetSummaryResult>> Handle(GetSummaryQuery request, CancellationToken cancellationToken)
        {
            var result = await employeeRepository.GetEmployeesByQueryAsync(request.Department, null, cancellationToken);
            if (!result.IsSuccess || result.Value == null)
            {
                logger.LogError("Failed to get employee details for employee within department {Department}. Error: {Error}", request.Department, result.Error);
                return Result<GetSummaryResult>.Failure(result.Error);
            }
            else if (result.Value != null && result.Value.Count == 0)
            {
                logger.LogInformation("No employee details found for employee within department {Department}.", request.Department);
                return Result<GetSummaryResult>.Success(new GetSummaryResult(new List<EmployeeSummaryDto>()));
            }

            var summaries = new List<EmployeeSummaryDto>();
            var employees = result.Value;

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
