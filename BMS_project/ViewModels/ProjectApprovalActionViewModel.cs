using System.ComponentModel.DataAnnotations;

namespace BMS_project.ViewModels
{
    public class ProjectApprovalActionViewModel
    {
        [Required]
        public int Project_ID { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal? Approved_Amount { get; set; }

        public string Remarks { get; set; }
    }
}
