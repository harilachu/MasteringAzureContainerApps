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
    public record GetProjectHistoryResult(List<ProjectDto> Projects);
    public record GetProjectHistoryQuery(string EmpId) : IQuery<Result<GetProjectHistoryResult>>;
    public class GetProjectHistoryQueryHandler : IRequestHandler<GetProjectHistoryQuery, Result<GetProjectHistoryResult>>
    {
        private readonly ILogger<GetProjectHistoryQueryHandler> logger;
        private readonly IProjectsRepository projectsRepository;
        private readonly IMapper mapper;

        public GetProjectHistoryQueryHandler(ILogger<GetProjectHistoryQueryHandler> logger, IProjectsRepository projectsRepository, IMapper mapper)
        {
            this.logger = logger;
            this.projectsRepository = projectsRepository;
            this.mapper = mapper;
        }

        public async ValueTask<Result<GetProjectHistoryResult>> Handle(GetProjectHistoryQuery request, CancellationToken cancellationToken)
        {
            var result = await projectsRepository.QueryRecentProjects(request.EmpId, cancellationToken);

            if (!result.IsSuccess || result.Value == null ) {
                logger.LogError("Failed to get project history for employee with id {EmployeeId}. Error: {Error}", request.EmpId, result.Error);
                return Result<GetProjectHistoryResult>.Failure(result.Error);
            }
            else if(result.IsSuccess && result.Value!=null && result.Value.Count == 0)
            {
                logger.LogInformation("No project history found for employee with id {EmployeeId}.", request.EmpId);
                return Result<GetProjectHistoryResult>.Success(new GetProjectHistoryResult(new List<ProjectDto>()));
            }

            var projects = result.Value;
            var projectsDto = projects.Select(p => new ProjectDto
            {
                ProjectName = p.ProjectName,
                CompletionDate = p.CompletionDate?.ToString(Constants.DATE_FORMAT) ?? Constants.NOT_AVAILABLE,
                IsCompletedOnTime = p.IsCompletedOnTime
            }).ToList();

            return Result<GetProjectHistoryResult>.Success(new GetProjectHistoryResult(projectsDto));
        }
    }
}
