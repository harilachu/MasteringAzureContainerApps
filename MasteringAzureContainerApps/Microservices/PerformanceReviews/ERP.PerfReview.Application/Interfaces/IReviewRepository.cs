using ERP.Common.Core;
using ERP.Common.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.PerfReview.Application.Interfaces
{
    public interface IReviewRepository
    {
        Task<decimal?> ComputeAverageScore(string empId, CancellationToken cancellationToken);
    }
}
