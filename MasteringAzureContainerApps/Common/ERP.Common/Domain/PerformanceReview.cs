using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ERP.Common.Domain
{
    public class PerformanceReview
    {
        public string id { get; set; }

        public string EmpId { get; set; }

        public DateTime ReviewDate { get; set; }

        [Range(1, 5)]
        public int Score { get; set; }
    }
}
