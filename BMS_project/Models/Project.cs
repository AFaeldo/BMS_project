using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMS_project.Models
{
    [Table("project")]
    public class Project
    {
        [Key]
        [Column("Project_ID")]
        public int Project_ID { get; set; }

        [Column("User_ID")]
        public int? User_ID { get; set; }

        [ForeignKey("User_ID")]
        public User User { get; set; }

        [Column("Term_ID")]
        public int? Term_ID { get; set; }

        [Required]
        [Column("Project_Title")]
        [StringLength(255)]
        public string Project_Title { get; set; } = string.Empty;

        [Column("Project_Description")]
        public string Project_Description { get; set; } = string.Empty;

        [Column("Estimated_Cost")]
        public decimal Estimated_Cost { get; set; }

        [Column("Date_Submitted")]
        public DateTime? Date_Submitted { get; set; }

        [Column("Project_Status")]
        public string Project_Status { get; set; } = "Pending"; // Enum stored as string

        [Column("Start_Date")]
        public DateTime? Start_Date { get; set; }

        [Column("End_Date")]
        public DateTime? End_Date { get; set; }

        [Column("IsArchived")]
        public bool IsArchived { get; set; } = false;

        // Navigation properties
        public ICollection<ProjectAllocation> Allocations { get; set; } = new List<ProjectAllocation>();
        public ICollection<ProjectLog> Logs { get; set; } = new List<ProjectLog>();
        public ICollection<FileUpload> Files { get; set; } = new List<FileUpload>();
    }
}