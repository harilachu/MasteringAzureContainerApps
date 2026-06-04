using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.Infrastructure.Core
{
    public static class InfraConstants
    {
        public const string GET_EMPLOYEE_BY_EMPID_QUERY = "SELECT * FROM Employees p WHERE p.Department = @partitionKey and p.EmpId = @empId";
        public const string GET_EMPLOYEES_BY_DEPT_QUERY = "SELECT * FROM Employees p WHERE p.Department = @partitionKey";
        public const string GET_RECENT_PROJECTS_BY_EMPID_QUERY = "SELECT * FROM Projects p WHERE p.EmpId = @empId and (DateTimeToTimestamp(p.CompletionDate)) >= (GetCurrentTimestamp()) - (365*24*60*60*1000)";
        public const string GET_PERF_REVIEW_SCORE_BY_EMPID_QUERY = "SELECT * FROM PerformanceReviews p WHERE p.EmpId = @empId and (DateTimeToTimestamp(p.ReviewDate)) >= (GetCurrentTimestamp()) - (365*24*60*60*1000)";
        public const string HAS_PROJECT_COMPLETED_ON_TIME_QUERY = "SELECT * FROM Projects p WHERE p.EmpId = @empId and (DateTimeToTimestamp(p.CompletionDate)) >= (GetCurrentTimestamp()) - (365*24*60*60*1000) and p.IsCompletedOnTime = true";


    }
}
