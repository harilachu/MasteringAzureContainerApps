using System;
using System.Collections.Generic;
using System.Text;
using ERP.Common.Core;
using ERP.Common.Domain;
using ERP.PerfReview.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace ERP.PerfReview.Infrastructure.Services
{
    public class BonusService : IBonusService
    {
        private readonly ILogger<BonusService> _logger;
        private readonly IReviewRepository _reviewRepository;
        private readonly IProjectsRepository _projectsRepository;

        public BonusService(ILogger<BonusService> logger, IReviewRepository reviewRepository, IProjectsRepository projectsRepository)

        {
            _logger = logger;
            _reviewRepository = reviewRepository;
            _projectsRepository = projectsRepository;
        }

        public async Task<decimal> CalculateBonusAsync(Employee employee, decimal? avgScore, CancellationToken cancellationToken)
        {
            var avgScoreRetrieved = (avgScore != null)? avgScore : 0m;
            if (avgScore == null)
            {
                avgScoreRetrieved = await GetAverageScoreAsync(employee, cancellationToken);
            }

            decimal bonusPercent = avgScoreRetrieved switch
            {
                >= Constants.AVERAGE_SCORE_LEVEL2 => Constants.BONUS_PERCENTAGE_LEVEL2, //avg score 4.6 and above
                >= Constants.AVERAGE_SCORE_LEVEL1 => Constants.BONUS_PERCENTAGE_LEVEL1, //avg score 4.0 to 4.5
                _ => 0m
            };

            try
            {
                var hasOnTimeProject = await _projectsRepository.HasProjectCompletedOnTime(employee.EmpId, cancellationToken);
                if (bonusPercent > 0 && hasOnTimeProject)
                {
                    bonusPercent += 0.05m;
                }

                return Math.Round(employee.AnnualIncome * bonusPercent, 2);
            }
            catch (ArgumentOutOfRangeException argEx)
            {
                _logger.LogError(argEx, "Error calculating bonus for EmployeeId: {EmployeeId}", employee.EmpId);
                return 0m;
            }
            catch (OverflowException ofEx)
            {
                _logger.LogError(ofEx, "Overflow error calculating bonus for EmployeeId: {EmployeeId}", employee.EmpId);
                return 0m;
            }
        }

        public async Task<decimal> GetAverageScoreAsync(Employee employee, CancellationToken cancellationToken)
        {
            if (employee == null)
            {
                _logger.LogError(new ArgumentNullException("employee"), "Employee cannot be null to calculate the average score");
                return 0m;
            }

            try
            {
                var oneYearAgo = DateTime.Now.AddYears(-1);
                var averageScore = await this._reviewRepository.ComputeAverageScore(employee.EmpId, cancellationToken) ?? 0;

                return Math.Round(averageScore, 2);
            }
            catch (ArgumentOutOfRangeException argEx)
            {
                _logger.LogError(argEx, "Error calculating Avg Score for EmployeeId: {EmployeeId}", employee.EmpId);
                return 0m;
            }
            catch (OverflowException ofEx)
            {
                _logger.LogError(ofEx, "Overflow error calculating Avg Score for EmployeeId: {EmployeeId}", employee.EmpId);
                return 0m;
            }
        }
    }
}
