using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMS_project.Models
{
    [Table("project_document")]
    public class ProjectDocument
    {
        [Key]
        [Column("Document_ID")]
        public int Document_ID { get; set; }

        [Column("Project_ID")]
        public int Project_ID { get; set; }

        [ForeignKey("Project_ID")]
        public Project Project { get; set; }

        [Column("File_ID")]
        public int File_ID { get; set; }

        [ForeignKey("File_ID")]
        public FileUpload File { get; set; }

        [Column("Date_Added")]
        public DateTime Date_Added { get; set; } = DateTime.Now;
        
        [Column("Description")]
        public string? Description { get; set; }
    }
}
