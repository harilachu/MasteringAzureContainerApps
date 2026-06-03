using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ERP.Common.DTO
{
    public class CreateEmployeeDto
    {
        [Required]
        public string EmpId { get; set; }

        public string EmpName { get; set; }

        [Required]
        public string Department { get; set; }

        public string State { get; set; }
        public string ManagerId { get; set; }

        public int AnnualIncome { get; set; }

        public int JoiningYear { get; set; }

    }
}
