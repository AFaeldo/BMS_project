using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMS_project.Models
{
    [Table("compliance_document")]
    public class ComplianceDocument
    {
        [Key]
        [Column("Document_ID")]
        public int Document_ID { get; set; }

        [Column("Compliance_ID")]
        public int Compliance_ID { get; set; }

        [ForeignKey("Compliance_ID")]
        public Compliance Compliance { get; set; }

        [Column("File_ID")]
        public int File_ID { get; set; }

        [ForeignKey("File_ID")]
        public FileUpload File { get; set; }

        [Column("Status")]
        [StringLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected

        [Column("Remarks")]
        public string? Remarks { get; set; }

        [Column("Date_Submitted")]
        public DateTime Date_Submitted { get; set; } = DateTime.Now;
    }
}
