using AutoMapper;
using ERP.Common.Domain;
using ERP.Infrastructure.Core.Interfaces;
using ERP.PerfReview.Application.Interfaces;
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

        public async Task<decimal?> ComputeAverageScore(int employeeId, DateTime oneYearAgo, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> HasProjectCompletedOnTime(int employeeId, DateTime oneYearAgo, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<int> QueryProjectsCount(int employeeId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
