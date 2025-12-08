using System;

namespace BMS_project.ViewModels
{
    public class ProjectApprovalListViewModel
    {
        public int Project_ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Barangay { get; set; }
        public decimal Amount { get; set; }
        public DateTime? Date { get; set; }
        public string Status { get; set; }

        public DateTime DateSubmitted { get; set; }
        
        public List<ProjectDocumentViewModel> Documents { get; set; } = new List<ProjectDocumentViewModel>();
    }

    public class ProjectDocumentViewModel
    {
        public int DocumentId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public int FileId { get; set; }
        public string? Description { get; set; }
        public DateTime DateAdded { get; set; }
    }
}
