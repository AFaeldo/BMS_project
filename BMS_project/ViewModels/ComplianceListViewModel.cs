using System;
using System.ComponentModel.DataAnnotations;

namespace BMS_project.ViewModels
{
    public class ComplianceListViewModel
    {
        public int Compliance_ID { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? Date_Submitted { get; set; }
        public string? BarangayName { get; set; }
        public int? Barangay_ID { get; set; }

        public int? SubmissionFileId { get; set; }
    }
}
