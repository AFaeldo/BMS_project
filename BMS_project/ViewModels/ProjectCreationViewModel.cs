using System;
using System.ComponentModel.DataAnnotations;

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
    }
}
