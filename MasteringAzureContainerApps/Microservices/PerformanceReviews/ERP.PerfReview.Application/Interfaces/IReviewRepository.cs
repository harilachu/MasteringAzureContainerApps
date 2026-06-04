using ERP.Common.Core;
using ERP.Common.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.PerfReview.Application.Interfaces
{
    public interface IReviewRepository
    {
        Task<decimal?> ComputeAverageScore(int employeeId, DateTime oneYearAgo, CancellationToken cancellationToken);
        Task<bool> HasProjectCompletedOnTime(int employeeId, DateTime oneYearAgo, CancellationToken cancellationToken);
        Task<int> QueryProjectsCount(int employeeId, CancellationToken cancellationToken);
    }
}
