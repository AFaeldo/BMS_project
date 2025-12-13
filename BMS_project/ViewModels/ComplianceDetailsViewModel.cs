using System;
using System.Collections.Generic;

namespace BMS_project.ViewModels
{
    public class ComplianceDetailsViewModel
    {
        public int ComplianceId { get; set; }
        public string Title { get; set; } // Added Title
        public string BarangayName { get; set; }
        public string ComplianceType { get; set; }
        public string? AnnexType { get; set; } // Added AnnexType
        public DateTime DueDate { get; set; }
        public string ComplianceStatus { get; set; }
        public List<SubmittedDocumentViewModel> Documents { get; set; } = new List<SubmittedDocumentViewModel>();
    }

    public class SubmittedDocumentViewModel
    {
        public int DocumentId { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public string Status { get; set; } // "Pending", "Approved", "Rejected"
        public string Remarks { get; set; }
        public DateTime DateSubmitted { get; set; }
    }
}
