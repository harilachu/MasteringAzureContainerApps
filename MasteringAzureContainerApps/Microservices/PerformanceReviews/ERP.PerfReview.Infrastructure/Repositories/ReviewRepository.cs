using AutoMapper;
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
    public class ReviewRepository : IReviewRepository
    {
        private readonly IBaseCosmosRepository<PerformanceReview> cosmosRepository;
        private readonly ILogger<ReviewRepository> logger;
        private readonly IMapper mapper;

        public ReviewRepository(IBaseCosmosRepository<PerformanceReview> cosmosRepository, ILogger<ReviewRepository> logger, IMapper mapper)
        {
            this.cosmosRepository = cosmosRepository;
            this.logger = logger;
            this.mapper = mapper;
        }

        public async Task<decimal?> ComputeAverageScore(string empId, CancellationToken cancellationToken)
        {
            var queryDefinition = new QueryDefinition(InfraConstants.GET_PERF_REVIEW_SCORE_BY_EMPID_QUERY);
            if (!string.IsNullOrEmpty(empId))
            {
                queryDefinition.WithParameter("@empId", empId);
            }

            var result =await cosmosRepository.GetItemsByQueryAsync<PerformanceReview>(queryDefinition, cancellationToken);
            if(result!=null && result.Value!=null && result.Value.Count() > 0)
            {
                var reviews = result.Value.ToList();
                return reviews.Average(r => (decimal?) r.Score);
            }

            return 0;
        }
    }
}
