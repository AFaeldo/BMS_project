using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace BMS_project.ViewModels
{
    public class ProjectCreationViewModel
    {
        [Required]
        public string Project_Title { get; set; }

        public string Project_Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Allocated_Amount { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Start_Date { get; set; }

        [DataType(DataType.Date)]
        public DateTime? End_Date { get; set; }

        // Project Proposal Files (Required - goes through original process)
        [Required(ErrorMessage = "Please upload at least one project proposal document (PDF).")]
        public List<IFormFile> ProjectProposalFiles { get; set; } = new List<IFormFile>();
        
        // Annex Files (Optional - goes to Project Documents)
        public List<IFormFile>? AnnexFiles { get; set; } = new List<IFormFile>();
        
        // Annex Types corresponding to each annex file
        public List<string>? AnnexTypes { get; set; } = new List<string>();
        
        public string? DocumentName { get; set; }
    }
}
