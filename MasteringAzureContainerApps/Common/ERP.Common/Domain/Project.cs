using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ERP.Common.Domain
{
    public class Project
    {
        public string id { get; set; }

        [Required]
        public string ProjectId { get; set; }
        [Required]
        public string ProjectName { get; set; }
        [Required]
        public string EmpId { get; set; }

        public DateTimeOffset AssignedDate { get; set; }
        public DateTimeOffset? CompletionDate { get; set; }
        public bool IsCompletedOnTime { get; set; }
    }
}
