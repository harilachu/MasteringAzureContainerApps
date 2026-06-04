using AutoMapper;
using ERP.Common.Core;
using ERP.Common.Domain;
using ERP.Infrastructure.Core;
using ERP.Infrastructure.Core.Interfaces;
using ERP.PerfReview.Application.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.PerfReview.Infrastructure.Repositories
{
    public class ProjectsRepository : IProjectsRepository
    {
        private readonly IBaseCosmosRepository<Project> cosmosRepository;
        private readonly ILogger<ProjectsRepository> logger;
        private readonly IMapper mapper;

        public ProjectsRepository(IBaseCosmosRepository<Project> cosmosRepository, ILogger<ProjectsRepository> logger, IMapper mapper)
        {
            this.cosmosRepository = cosmosRepository;
            this.logger = logger;
            this.mapper = mapper;
        }

        public async Task<Result<List<Project>>> QueryRecentProjects(string empId, CancellationToken cancellationToken)
        {
            var queryDefinition = new QueryDefinition(InfraConstants.GET_RECENT_PROJECTS_BY_EMPID_QUERY);
            if (!string.IsNullOrEmpty(empId))
            {
                queryDefinition.WithParameter("@empId", empId);
            }

            return await cosmosRepository.GetItemsByQueryAsync<Project>(queryDefinition, cancellationToken);
        }

        public async Task<bool> HasProjectCompletedOnTime(string empId, CancellationToken cancellationToken)
        {
            var queryDefinition = new QueryDefinition(InfraConstants.HAS_PROJECT_COMPLETED_ON_TIME_QUERY);
            if (!string.IsNullOrEmpty(empId))
            {
                queryDefinition.WithParameter("@empId", empId);
            }

            var result = await cosmosRepository.GetItemsByQueryAsync<Project>(queryDefinition, cancellationToken);
            if (result != null && result.Value != null && result.Value.Count() > 0)
            {
                return true;
            }

            return false;
        }

        public async Task<int> QueryProjectsCount(string empId, CancellationToken cancellationToken)
        {
            var queryDefinition = new QueryDefinition(InfraConstants.GET_RECENT_PROJECTS_BY_EMPID_QUERY);
            if (!string.IsNullOrEmpty(empId))
            {
                queryDefinition.WithParameter("@empId", empId);
            }

            var result = await cosmosRepository.GetItemsByQueryAsync<Project>(queryDefinition, cancellationToken);
            if (result != null && result.Value != null && result.Value.Count() > 0)
            {
                return result.Value.Count();
            }

            return 0;
        }
    }
}
