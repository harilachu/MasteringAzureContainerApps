using AutoMapper;
using ERP.Common.Core;
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
    public record GetBonusResult(BonusDto BonusInfo);
    public record GetBonusQuery(string EmpId, string Department) : IQuery<Result<GetBonusResult>>;
    public class GetBonusQueryHandler : IRequestHandler<GetBonusQuery, Result<GetBonusResult>>
    {
        private readonly ILogger<GetBonusQueryHandler> logger;
        private readonly IProjectsRepository projectsRepository;
        private readonly IEmployeeRepository employeeRepository;
        private readonly IBonusService bonusService;
        private readonly IMapper mapper;

        public GetBonusQueryHandler(ILogger<GetBonusQueryHandler> logger, IProjectsRepository projectsRepository, IEmployeeRepository employeeRepository, IBonusService bonusService, IMapper mapper)
        {
            this.logger = logger;
            this.projectsRepository = projectsRepository;
            this.employeeRepository = employeeRepository;
            this.bonusService = bonusService;
            this.mapper = mapper;
        }

        public async ValueTask<Result<GetBonusResult>> Handle(GetBonusQuery request, CancellationToken cancellationToken)
        {
            var result = await employeeRepository.GetEmployeesByQueryAsync(request.Department, request.EmpId, cancellationToken);
            if (!result.IsSuccess || result.Value == null)
            {
                logger.LogError("Failed to get employee details for employee with id {EmployeeId} in department {Department}. Error: {Error}", request.EmpId, request.Department, result.Error);
                return Result<GetBonusResult>.Failure(result.Error);
            }
            else if (result.Value != null && result.Value.Count == 0)
            {
                logger.LogInformation("No employee details found for employee with id {EmployeeId} in department {Department}.", request.EmpId, request.Department);
                return Result<GetBonusResult>.Success(new GetBonusResult(new BonusDto()));
            }

            var employee = result.Value.FirstOrDefault();
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
