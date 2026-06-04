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


    }
}
