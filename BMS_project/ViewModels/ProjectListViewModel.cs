using System;
using System.Collections.Generic;

namespace BMS_project.ViewModels
{
    public class ProjectListViewModel
    {
        public int Project_ID { get; set; }
        public string Project_Title { get; set; }
        public string Project_Description { get; set; }
        public string Project_Status { get; set; }
        public decimal Allocated_Budget { get; set; }
        public DateTime? Date_Submitted { get; set; }
        public DateTime? Start_Date { get; set; }
        public DateTime? End_Date { get; set; }
        public int? Term_ID { get; set; }

        // Barangay SK submitted proposal documents
        public List<AnnexDocumentViewModel> ProposalDocuments { get; set; } = new List<AnnexDocumentViewModel>();

        // SK Federation uploaded annex documents (Annex I/H)
        public List<AnnexDocumentViewModel> FederationAnnexDocuments { get; set; } = new List<AnnexDocumentViewModel>();
    }
}
