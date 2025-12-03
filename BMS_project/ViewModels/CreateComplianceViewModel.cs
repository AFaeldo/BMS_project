using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BMS_project.ViewModels
{
    public class CreateComplianceViewModel
    {
        [Required]
        [Display(Name = "Barangay")]
        public int SelectedBarangayId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Document Type")]
        public string DocumentType { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; }

        // Dropdown list
        public IEnumerable<SelectListItem>? BarangayList { get; set; }
    }
}
