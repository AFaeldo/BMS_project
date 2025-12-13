using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMS_project.Models
{
    [Table("compliance")]
    public class Compliance
    {
        [Key]
        [Column("com_id")]
        public int Compliance_ID { get; set; }

        [Column("Barangay_id")]
        public int? Barangay_ID { get; set; }

        [ForeignKey("Barangay_ID")]
        public Barangay Barangay { get; set; }

        [Required]
        [Column("Title")]
        [StringLength(255)]
        public string Title { get; set; } = string.Empty;

        [Column("Type")]
        [StringLength(100)]
        public string Type { get; set; } = string.Empty;

        [Column("Annex_Type")]
        [StringLength(50)]
        public string? Annex_Type { get; set; }

        [Column("Status")]
        [StringLength(50)]
        public string Status { get; set; } = "Not Submitted";

        [Column("due_date")]
        public DateTime Due_Date { get; set; }
        [Column("Date_Submitted")]
        public DateTime? Date_Submitted { get; set; }

        [Column("File_ID")]
        public int? File_ID { get; set; }

        [ForeignKey("File_ID")]
        public FileUpload SubmissionFile { get; set; }

        [Column("Term_ID")]
        public int Term_ID { get; set; }

        [ForeignKey("Term_ID")]
        public KabataanTermPeriod KabataanTermPeriod { get; set; }

        [Column("IsArchived")]
        public bool IsArchived { get; set; } = false;

        [Column("TemplateFile_ID")]
        public int? TemplateFile_ID { get; set; }

        [ForeignKey("TemplateFile_ID")]
        public FileUpload? TemplateFile { get; set; }

        public ICollection<ComplianceDocument> Documents { get; set; } = new List<ComplianceDocument>();
    }
}
