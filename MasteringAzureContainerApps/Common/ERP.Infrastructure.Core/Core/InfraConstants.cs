using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.Infrastructure.Core
{
    public static class InfraConstants
    {
        public const string GET_EMPLOYEE_BY_EMPID_QUERY = "SELECT * FROM Employees p WHERE p.Department = @partitionKey and p.EmpId = @empId";
        public const string GET_EMPLOYEES_BY_DEPT_QUERY = "SELECT * FROM Employees p WHERE p.Department = @partitionKey";

    }
}
