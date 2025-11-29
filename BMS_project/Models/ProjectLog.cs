using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMS_project.Models
{
    [Table("project_log")]
    public class ProjectLog
    {
        [Key]
        [Column("Log_ID")]
        public int Log_ID { get; set; }

        [Column("Project_ID")]
        public int? Project_ID { get; set; }

        [ForeignKey("Project_ID")]
        public Project Project { get; set; }

        [Column("User_ID")]
        public int? User_ID { get; set; }

        [ForeignKey("User_ID")]
        public User User { get; set; }

        [Column("Status")]
        public string Status { get; set; } = "Pending";

        [Column("Changed_On")]
        public DateTime? Changed_On { get; set; } = DateTime.Now;

        [Column("Remarks")]
        public string Remarks { get; set; }
    }
}
