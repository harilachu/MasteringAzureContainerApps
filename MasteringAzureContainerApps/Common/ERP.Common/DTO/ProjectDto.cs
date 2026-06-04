using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.Common.DTO
{
    public class ProjectDto
    {
        public string ProjectName { get; set; }
        public string CompletionDate { get; set; }
        public bool IsCompletedOnTime { get; set; }
    }
}
