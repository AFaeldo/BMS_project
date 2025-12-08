using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace BMS_project.ViewModels
{
    public class ComplianceViewModel
    {
        public int Compliance_ID { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        public int? BarangayId { get; set; }

        // Display properties
        public string? Status { get; set; }

        public DateTime? Date_Submitted { get; set; }
        public string? SubmissionFilePath { get; set; }
        public int? SubmissionFileId { get; set; }

        // Template Info
        public string? TemplateFileName { get; set; }
        public int? TemplateFileId { get; set; }
    }
}
