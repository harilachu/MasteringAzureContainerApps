using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.Common.DTO
{
    public class EmployeeSummaryDto
    {
        public string EmployeeId { get; set; }
        public string FullName { get; set; }
        public decimal AverageScore { get; set; }
        public int ProjectsCompleted { get; set; }
        public decimal BonusAmount { get; set; }
    }
}
