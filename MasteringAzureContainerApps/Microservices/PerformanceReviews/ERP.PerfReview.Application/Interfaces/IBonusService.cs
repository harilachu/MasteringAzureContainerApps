using ERP.Common.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.PerfReview.Application.Interfaces
{
    public interface IBonusService
    {
        Task<decimal> GetAverageScoreAsync(Employee employee, CancellationToken cancellationToken);
        Task<decimal> CalculateBonusAsync(Employee employee, decimal? avgScore, CancellationToken cancellationToken);
    }
}
