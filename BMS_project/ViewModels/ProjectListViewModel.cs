using System;

namespace BMS_project.ViewModels
{
    public class ProjectListViewModel
    {
        public int Project_ID { get; set; }
        public string Project_Title { get; set; }
        public string Project_Description { get; set; }
        public DateTime? Start_Date { get; set; }
        public DateTime? End_Date { get; set; }
        public DateTime? Date_Submitted { get; set; }
        public string Project_Status { get; set; }
        public decimal Allocated_Budget { get; set; }
    }
}
