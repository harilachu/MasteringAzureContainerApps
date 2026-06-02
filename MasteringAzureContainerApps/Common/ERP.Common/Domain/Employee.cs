using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.Common.Domain
{
    public class Employee
    {
        public string id { get; set; }

        public string EmpId { get; set; }

        public string EmpName { get; set; }
        public string Department { get; set; }

        public string State { get; set; }
        public string ManagerId { get; set; }

        public int AnnualIncome { get; set; }

        public int JoiningYear { get; set; }
    }
}
