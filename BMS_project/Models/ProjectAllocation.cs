using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMS_project.Models
{
    [Table("project_allocation")]
    public class ProjectAllocation
    {
        [Key]
        [Column("Allocation_ID")]
        public int Allocation_ID { get; set; }

        [Column("Budget_ID")]
        public int Budget_ID { get; set; }

        [ForeignKey("Budget_ID")]
        public Budget Budget { get; set; }

        [Column("Project_ID")]
        public int Project_ID { get; set; }

        [ForeignKey("Project_ID")]
        public Project Project { get; set; }

        [Column("Amount_Allocated")]
        public decimal Amount_Allocated { get; set; }
    }
}
