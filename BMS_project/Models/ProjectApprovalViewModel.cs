using Microsoft.AspNetCore.Mvc;

namespace BMS_project.Models
{
    public class ProjectApprovalViewModel : Controller
    {
        public int Project_ID { get; set; }
        public string Project_Title { get; set; }
        public string Project_Description { get; set; }
        public string Barangay_Name { get; set; }
        public DateTime? Date_Submitted { get; set; }
        public string Project_Status { get; set; }
    }
}
